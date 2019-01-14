using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
namespace RTI
{
    public class InspectorManager : MonoBehaviour
    {
        /// <summary>
        /// 检索器所处的根节点
        /// </summary>
        public RectTransform root;
        /// <summary>
        /// 检索器的配置
        /// </summary>
        public InspectorAsset asset;
        private static InspectorManager instance;
        /// <summary>
        /// InspectorBehaviour的唯一实例
        /// </summary>
        /// <value></value>
        public static InspectorManager Instance
        {
            get
            {
                if (!instance)
                {
                    instance = FindObjectOfType<InspectorManager>();
                    if (!instance)
                    {
                        throw new Exception("没有在场景中找到InspectorManager实例！");
                    }
                    DontDestroyOnLoad(instance.gameObject);
                }
                return instance;
            }
        }
        /// <summary>
        /// 所有检查到的MemberAttribute类型，会在Initialize时重新检查
        /// </summary>
        private List<Type> MemberAttributeTypes = new List<Type>();
        /// <summary>
        /// 所有记录在册的检索器预置
        /// </summary>
        public List<GameObject> InpectorPrefabList { get { return asset.memberInpectorPrefabList; } }
        /// <summary>
        /// 已用过的检索器预置缓存
        /// </summary>
        protected Dictionary<string, GameObject> UsedMemberInspectorPrefabs = new Dictionary<string, GameObject>();
        /// <summary>
        /// 与某个检索器关键值存在绑定关系的类型
        /// </summary>
        /// <value></value>
        protected Dictionary<Type, string> BindedTypes
        {
            get; private set;
        }
        /// <summary>
        /// 没有被bind的类型缓存
        /// </summary>
        protected HashSet<Type> UnBindedTypesCache;
        /// <summary>
        /// 检索识别过滤器。
        /// InspectorManager在判断是否要识别并检索某个Member时，会从前向后逐个执行每个过滤器，
        /// 如果过滤器最终没有得到合法的检索信息，则表明该Member不可被检索。
        /// </summary>
        /// <param name="context">执行检索的检索管理器</param>
        /// <param name="host">宿主对象</param>
        /// <param name="memberInfo">要尝试检索的类成员信息</param>
        /// <param name="inspectInfo">上一个过滤器得到的检索信息，同时也会传给下一个过滤器</param>
        public delegate void InspectInfoFilter(InspectorManager context, object host, MemberInfo memberInfo, ref InspectInfo inspectInfo);
        /// <summary>
        /// 检索识别过滤器列表。
        /// InspectorManager在判断是否要识别并检索某个Member时，会从前向后逐个执行每个过滤器，
        /// 如果过滤器最终没有得到合法的检索信息，则表明该Member不可被检索。
        /// 过滤器列表是静态的，你可以使用[UnityEngine.RuntimeInitializeOnLoadMethodAttribute]标记来在游戏开始时注册新的过滤器
        /// </summary>
        /// <typeparam name="InspectInfoFilter"></typeparam>
        /// <returns></returns>
        public static List<InspectInfoFilter> InspectInfoFilters { get; } = new List<InspectInfoFilter>();

        static InspectorManager()
        {
            //注册一个使用Attribute的检索识别过滤器
            InspectInfoFilter AttributeFilter = (InspectorManager context, object host, MemberInfo memberInfo, ref InspectInfo inspectInfo) =>
            {
                //若inspectInfo尚未定义
                if (inspectInfo == null)
                {
                    var memberAttribute = memberInfo.GetCustomAttribute<MemberAttribute>(true);
                    if (memberAttribute != null)
                    {
                        inspectInfo = memberAttribute.inspectInfo;
                    }
                }
            };
            InspectInfoFilters.Add(AttributeFilter);
        }
        /// <summary>
        /// 为该key在注册表中所有检索器预制体中寻找一个最合适的检索器预置。
        /// > 若未找到，则返回null
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected List<GameObject> GetOriginMemberInsectorPrefabsByKey(string key)
        {
            var ret = new List<GameObject>();
            foreach (var prefab in InpectorPrefabList)
            {
                //若该prefab中的memberInspector类型的key满足条件...
                var memberInspector = prefab.GetComponent<MemberInspector>();
                if (memberInspector && memberInspector.IsFit(key))
                {
                    ret.Add(prefab);
                }
            }
            return ret;
        }
        /// <summary>
        /// 通过key获取一个可用的检索器预置，这个预置包含一个符合条件的MemberInspector
        /// 将优先通过缓存来查找
        /// > 若未找到，则返回null
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public GameObject GetMemberInspectorPrefabByKey(string key)
        {
            GameObject ret;
            //在已用预置中寻找
            UsedMemberInspectorPrefabs.TryGetValue(key, out ret);
            if (ret == null)
            {
                //若没有在已用预置中找到，则尝试从所有注册的预制体中寻找
                ret = GetOriginMemberInsectorPrefabsByKey(key).LastOrDefault();
                if (ret)
                {
                    UsedMemberInspectorPrefabs.Add(key, ret);
                }
            }
            return ret;
        }
        /// <summary>
        /// 初始化运行时检索器。
        /// 在初始化过程中，检索器会完成以下任务：
        /// * 重新检查所有已定义的MemberAttribute及其派生类型
        /// * 重新检查并移除所有不合法的检索器预置
        /// * 重置对所有已使用检索关键词-检索器预置的相关缓存
        /// * 重新检查所有存在的Binder并将检索器关键词与相应的类型进行重新绑定
        /// </summary>
        public virtual void Initialize()
        {
            //清除缓存
            this.MemberAttributeTypes.Clear();
            if (this.UsedMemberInspectorPrefabs == null)
            {
                this.UsedMemberInspectorPrefabs = new Dictionary<string, GameObject>();
            }
            else
            {
                this.UsedMemberInspectorPrefabs.Clear();
            }

            //清除已Bind类型的记录
            if (this.BindedTypes == null)
            {
                this.BindedTypes = new Dictionary<Type, string>();
            }
            else
            {
                this.BindedTypes.Clear();
            }
            //清除未Bind类型的缓存
            if (this.UnBindedTypesCache == null)
            {
                this.UnBindedTypesCache = new HashSet<Type>();
            }
            else
            {
                this.UnBindedTypesCache.Clear();
            }

            //获取所有类型
            var types = Utils.GetAllTypes();
            //记录所有MemberAttribute类型
            foreach (var type in types)
            {
                if (typeof(MemberAttribute).IsAssignableFrom(type))
                {
                    //如果是MemberAttribute或其派生
                    this.MemberAttributeTypes.Add(type);
                }

            }
            //将检索器关键词-类型进行绑定
            foreach (var type in types)
            {
                this.TryBind(type);
            }
        }
        /// <summary>
        /// 检索该游戏对象，返回一个携带了DrawerBehaviour的UI游戏对象
        /// </summary>
        /// <param name="target"></param>
        public virtual GameObject Inspect(object target, string name)
        {
            //根据根检索器预置创建一个根检索器
            var rootInspectorObject = Instantiate(this.asset.rootInspectorPrefab, this.root);
            var rootInspector = rootInspectorObject.GetComponent<InspectorBehaviour>();
            var hostType = target.GetType();
            rootInspector.InspectName = name;
            //检查该object的每一个member
            foreach (var memberInfo in hostType.GetMembers())
            {
                //检查并尝试bind该member
                var memberDrawer = TryInspect(target, memberInfo);
                if (memberDrawer)
                {
                    //如果bind成功
                    rootInspector.AddChild(memberDrawer);
                }
            }
            return rootInspectorObject;
        }
        /// <summary>
        /// 尝试从类型-检索关键词 的绑定表中寻找到适合该类型的关键词。
        /// 若该类型不直接具有某关键词，会迭代遍历其基类，直到找到或是迭代结束为止
        /// 如果未能找到，则返回null
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string GetBindedKey(Type type)
        {
            string ret = null;
            var initType = type;
            for (; type != null && !UnBindedTypesCache.Contains(type); type = type.BaseType)
            {
                //循环遍历type及其每一个基类，直到基类不存在为止
                this.BindedTypes.TryGetValue(type, out ret);
                if (ret != null) break;
            }
            if (ret == null)
            {
                this.UnBindedTypesCache.Add(initType);
            }
            return ret;
        }
        /// <summary>
        /// 尝试进行类型-检索器关键值绑定
        /// </summary>
        public void TryBind(Type type)
        {
            foreach (var member in type.GetMembers())
            {
                BindAttribute bindAttribute = Attribute.GetCustomAttribute(member, typeof(BindAttribute)) as BindAttribute;
                if (bindAttribute != null && this.asset.activeBindNameList.Contains(bindAttribute.name))
                {
                    //如果BindAttribute被定义，并且被使用
                    try
                    {
                        var value = (member as FieldInfo).GetValue(type);
                        var binders = (IEnumerable<Binder>)value;
                        //执行bind
                        foreach (var binder in binders)
                        {
                            foreach (var t in binder.types)
                            {
                                //将binder中输入的每一个Type与对应的Key记录下来
                                this.BindedTypes.Add(t, binder.key);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        //类型转换失败
                        Interf.Instance.Print("Failed to cast the BindList {0} as IEnumerable<BInder>!", member);
                    }
                }
            }
        }

        /// <summary>
        /// 尝试检索某个Member
        /// </summary>
        /// <param name="host"></param>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        public InspectorBehaviour TryInspect(object host, MemberInfo memberInfo)
        {
            InspectInfo inspectInfo = null;
            foreach (var inspectInfoFilter in InspectInfoFilters)
            {
                inspectInfoFilter(this, host, memberInfo, ref inspectInfo);
            }
            if (inspectInfo != null)
            {
                //识别成功，得到了该类成员的检索信息
                return Inspect(host, memberInfo, inspectInfo);
            }
            else
            {
                //识别失败
                return null;
            }
        }
        public MemberInspector Inspect(object host, MemberInfo memberInfo, InspectInfo inspectInfo)
        {
            MemberInspector memberInspector = null;
            if (inspectInfo != null)
            //如果识别成功
            {
                var prefab = GetMemberInspectorPrefabByKey(inspectInfo.Key);
                if (prefab)
                {
                    //生成memberInspector，并完成绑定工作
                    var memberDrawerObject = Instantiate(prefab);
                    memberInspector = memberDrawerObject.GetComponent<MemberInspector>();
                    if (!memberInspector)
                    {
                        throw new Exception("在目标检索器预制体中找不到合法的MemberInspector组件！");
                    }
                    memberInspector.Host = host;
                    memberInspector.inspectInfo = inspectInfo;
                    memberInspector.InspectName = memberInfo.Name;
                    inspectInfo.member = memberInfo;
                    Interf.Instance.Print("bind Member of {0} successfully from Type {1} !", memberInfo.ToString(), memberInfo.DeclaringType.ToString());
                }
                else
                {
                    //未找到合适的prefab，输出失败信息
                    Interf.Instance.Print("Failed to find the valid prefab to inspect the member by key of [{0}]", inspectInfo.Key);
                }
            }
            return memberInspector;
        }
    }
}
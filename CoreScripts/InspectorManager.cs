using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
namespace RTI
{
    public class InspectorManager : MonoBehaviour
    {
        public InspectorBehaviour rootInspector;
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
        /// 所有检查到的 DrawerBehaviour 类型，会在Initialize时重新检查
        /// </summary>
        private List<Type> DrawerBehaviourTypes = new List<Type>();
        /// <summary>
        /// 所有记录在册的检索器预置
        /// </summary>
        public List<GameObject> allDrawerPrefabList;
        /// <summary>
        /// 所有当前可用的检索器预置
        /// </summary>
        protected Dictionary<string, GameObject> UsedDrawerPrefabs = new Dictionary<string, GameObject>();
        /// <summary>
        /// 已经被检索过的类型缓存
        /// </summary>
        /// <value></value>
        protected Dictionary<Type, GameObject> InspectedTypes
        {
            get; private set;
        }
        public List<Type> GetDrawerTypesByKey(string key)
        {
            var ret = new List<Type>();
            foreach (var drawerBehaviourType in DrawerBehaviourTypes)
            {
                //获取在该 Drawerbehaviour 上标记的 BindAttribute
                var binder = Attribute.GetCustomAttribute(drawerBehaviourType, typeof(DrawerAttribute)) as DrawerAttribute;
                if (binder != null)
                {
                    //如果关键字和该drawerbehaviour相符合
                    if (binder.IsFit(key))
                    {
                        ret.Add(drawerBehaviourType);
                    }
                }
            }
            return ret;
        }
        /// <summary>
        /// 为该key在记录的所有检索器预制体中寻找一个最合适的检索器预置。
        /// > 若未找到，则返回null
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected GameObject FindDrawerPrefabFromListForKey(string key)
        {
            var ret = new List<GameObject>();
            //获取所有相符合的Drawer类型
            var drawerTypes = GetDrawerTypesByKey(key);
            foreach (var prefab in allDrawerPrefabList)
            {
                //若该prefab中的DrawerBehaviour类型的key满足条件...
                var drawer = prefab.GetComponent<DrawerBehaviour>();
                if (drawer)
                {
                    if (drawerTypes.Contains(drawer.GetType()))
                    {
                        ret.Add(prefab);
                    }
                }
            }
            if (ret.Count > 0)
            {
                //返回最新的预置
                return ret[ret.Count - 1];
            }
            else
            {
                //若未找到，则返回null
                return null;
            }
        }
        /// <summary>
        /// 通过key获取一个可用的检索器预置，这个预置包含一个符合条件的DrawerBehaviour
        /// > 若未找到，则返回null
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public GameObject GetValidDrawerPrefabByKey(string key)
        {
            GameObject ret;
            UsedDrawerPrefabs.TryGetValue(key, out ret);
            if (ret == null)
            {
                //若没有在已用预置中找到，则尝试从记录的预制体中寻找
                ret = FindDrawerPrefabFromListForKey(key);
                if (ret)
                {
                    UsedDrawerPrefabs.Add(key, ret);
                }
            }
            return ret;
        }
        /// <summary>
        /// 初始化运行时检索器。
        /// 在初始化过程中，检索器会完成以下任务：
        /// * 检查并记录所有已定义的MemberAttribute及其派生类型
        /// * 检查并记录所有已定义的DrawerBehaviour派生类型
        /// * 移除所有不合法的检索器预置
        /// * 重置对所有已检索类型的缓存
        /// </summary>
        public virtual void Initialize()
        {
            var types = Utils.GetAllTypes();
            this.MemberAttributeTypes.Clear();
            this.DrawerBehaviourTypes.Clear();
            foreach (var type in types)
            {
                if (typeof(MemberAttribute).IsAssignableFrom(type))
                {
                    //如果是MemberAttribute或其派生
                    this.MemberAttributeTypes.Add(type);
                }
                else if (typeof(DrawerBehaviour).IsAssignableFrom(type))
                {
                    //如果是UIBehaivour或其派生
                    this.DrawerBehaviourTypes.Add(type);
                }
            }
            foreach (var prefab in this.allDrawerPrefabList.AsReadOnly())
            {
                //若预制体中不包含DrawerBehaviour，则将其移除
                if (!prefab.GetComponent<DrawerBehaviour>()) { this.allDrawerPrefabList.Remove(prefab); }
            }
        }
        /// <summary>
        /// 检索该游戏对象，返回一个携带了DrawerBehaviour的UI游戏对象
        /// </summary>
        /// <param name="target"></param>
        public virtual void Inspect(object target)
        {
            var hostType = target.GetType();

            //检查该object的每一个member
            foreach (var memberInfo in hostType.GetMembers())
            {
                //检查并尝试bind该member
                var memberDrawer = TryBind(target, memberInfo);
                if (memberDrawer)
                {
                    //如果bind成功
                    this.rootInspector.AddChild(memberDrawer);
                }
            }
        }
        public GameObject TryBind(object host, MemberInfo memberInfo)
        {
            //获取该member上面标注的MemberAttribute
            var memberAttribute = memberInfo.GetCustomAttribute<MemberAttribute>(true);
            if (memberAttribute != null)
            //如果标记存在
            {
                var prefab = GetValidDrawerPrefabByKey(memberAttribute.Key);
                if (prefab)
                {
                    //生成drawer，并完成绑定工作
                    var memberDrawerObject = Instantiate(prefab);
                    var drawer = memberDrawerObject.GetComponent<DrawerBehaviour>();
                    drawer.Host = host;
                    drawer.memberAttribute = memberAttribute;
                    memberAttribute.member = memberInfo;
                    Interf.Instance.Print("bind Member of {0} successfully from Type {1} !", memberInfo.ToString(), memberInfo.DeclaringType.ToString());
                    return memberDrawerObject;
                }
                else
                {
                    //未找到合适的prefab，输出失败信息
                    Interf.Instance.Print("Failed to find the valid prefab to draw the member by key of [{0}]", memberAttribute.Key);
                }
            }
            return null;
        }
        public void Bind(object host, MemberInfo memberInfo, MemberAttribute memberAttribute, DrawerBehaviour drawer)
        {
            var hostType = memberInfo.ReflectedType;
            // 绑定该Field
            memberAttribute.member = memberInfo;
            //输出相应消息
            Interf.Instance.Print("binding field {0} of type {1} in way of {2}", memberInfo.Name, hostType.Name, memberAttribute.Key);
        }
        /// <summary>
        /// 从fieldAttributeType列表中寻找到合适该Field的FieldAtribute，并执行绑定
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="memberInfo"></param>
        /// <param name="memberAttributeTypes"></param>
        /// <returns></returns>
        public static MemberAttribute BindFrom(DrawerAttribute binder, MemberInfo memberInfo, List<Type> memberAttributeTypes)
        {
            //检查所有标注了 MemberAttribute派生标记的 MemberInfo
            foreach (var memberAttributeType in memberAttributeTypes)
            {
                var memberAttribute = Attribute.GetCustomAttribute(memberInfo, memberAttributeType) as MemberAttribute;
                //如果该MemberAttribute被定义，并且类型相符合
                if (memberAttribute != null && binder.IsFit(memberAttribute.Key))
                {
                    //执行Bind，并返回该attribute
                    //Bind(binder, memberInfo, memberAttribute);
                    return memberAttribute;
                }
            }
            return null;
        }
    }
}
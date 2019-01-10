using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
namespace RTI
{
    public class InspectorBehaviour : MonoBehaviour
    {
        List<Type> FieldAttributeTypes = new List<Type>();
        List<Type> UIBehaviourTypes = new List<Type>();
        /// <summary>
        /// 所有记录在册的检索器预置
        /// </summary>
        public List<GameObject> RTIUIPrefabs;
        /// <summary>
        /// 初始化运行时检索器。
        /// 在初始化过程中，检索器会完成以下任务：
        /// * 检查并记录所有已定义的FieldAttribute及其派生类型
        /// * 检查并记录所有已定义并在RTUIPrefabs中存在的UIBehaviour派生类型
        /// </summary>
        public virtual void Initialize()
        {
            var types = Utils.GetAllTypes();
            this.FieldAttributeTypes.Clear();
            this.UIBehaviourTypes.Clear();
            foreach (var type in types)
            {
                if (typeof(FieldAttribute).IsAssignableFrom(type))
                {
                    //如果是FieldAttribute或其派生
                    this.FieldAttributeTypes.Add(type);
                }
                else if (typeof(UIBehaviour).IsAssignableFrom(type))
                {
                    //如果是UIBehaivour或其派生
                    this.UIBehaviourTypes.Add(type);
                    //检查是否在prefabs中存在
                }
            }
        }
        /// <summary>
        /// 检索该游戏对象，返回一个携带了RTUIBehaviour的UI游戏对象
        /// </summary>
        /// <param name="target"></param>
        public virtual void Inspect(object target)
        {
            //fixme 尝试bind每一个UIPrefab
            foreach (var uiBehaviourType in this.UIBehaviourTypes)
            {
                //检查并尝试bind
            }
        }
    }
}
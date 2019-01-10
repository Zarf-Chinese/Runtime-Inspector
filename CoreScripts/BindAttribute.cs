using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
namespace RTI
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    sealed public class BindAttribute : Attribute
    {
        public string Type { get; private set; }
        public bool IsFit(FieldAttribute fieldAttribute)
        {
            return this.Type == fieldAttribute.Type;
        }
        public static void Bind(BindAttribute binder, FieldInfo fieldInfo, FieldAttribute fieldAttribute)
        {

            var hostType = fieldInfo.ReflectedType;
            // 绑定该Field
            fieldAttribute.hostType = hostType;
            fieldAttribute.field = fieldInfo;
            //输出相应消息
            Interf.Instance.Print("binding field {0} of type {1} in way of {2}", fieldInfo.Name, hostType.Name, binder.Type);
        }
        /// <summary>
        /// 从fieldAttributeType列表中寻找到合适该Field的FieldAtribute，并执行绑定
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="fieldInfo"></param>
        /// <param name="fieldAttributeTypes"></param>
        /// <returns></returns>
        public static FieldAttribute BindFrom(BindAttribute binder, FieldInfo fieldInfo, List<Type> fieldAttributeTypes)
        {
            //检查所有标注了 fieldAttribute派生标记的 Field
            foreach (var fieldAttributeType in fieldAttributeTypes)
            {
                var fieldAttribute = Attribute.GetCustomAttribute(fieldInfo, fieldAttributeType) as FieldAttribute;
                //如果该FieldAttribute被定义，并且类型相符合
                if (fieldAttribute != null && binder.IsFit(fieldAttribute))
                {
                    //执行Bind，并返回该attribute
                    Bind(binder, fieldInfo, fieldAttribute);
                    return fieldAttribute;
                }
            }
            return null;
        }
        public BindAttribute(string type) { this.Type = type; }
    }
}

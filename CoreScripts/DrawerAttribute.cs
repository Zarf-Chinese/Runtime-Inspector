using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
namespace RTI
{
    /// <summary>
    /// 用于标记一个属性检索器行为的类型上。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    sealed public class DrawerAttribute : Attribute
    {
        public List<string> Keys { get; private set; }
        public bool IsFit(string key)
        {
            return this.Keys.Contains(key);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="keys">需要至少一个Key</param>
        /// <typeparam name="string"></typeparam>
        /// <returns></returns>
        public DrawerAttribute(params string[] keys)
        {
            if (keys.Length > 0)
            {
                this.Keys = new List<string>(keys);
            }
            else
            {
                throw new Exception("至少需要一个Key来初始化 DrawerAttribute!");
            }
        }
    }
}

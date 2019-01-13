using System;
using System.Collections.Generic;

namespace RTI
{
    public class Binder
    {
        public Binder(string key, Type type, params Type[] otherTypes)
        {
            var types = new HashSet<Type>();
            types.Add(type);
            foreach (var t in otherTypes) { types.Add(t); }
            this.types = types;
            this.key = key;
        }
        /// <summary>
        /// 要绑定到的类型
        /// </summary>
        public HashSet<Type> types;
        /// <summary>
        /// 这些类型要对应的检索器关键词
        /// </summary>
        public string key;
    }
}
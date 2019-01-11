using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
namespace RTI
{
    public abstract class MemberAttribute : Attribute
    {
        public MemberInfo member;
        public abstract Type GetMemberType();
        public abstract object GetMemberData(object host);
        public abstract void SetMemberData(object host, object value);
        public string Key { get; private set; }
        public MemberAttribute(string key) { this.Key = key; }
    }
}

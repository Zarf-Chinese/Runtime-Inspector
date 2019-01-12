using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
namespace RTI
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class FieldAttribute : MemberAttribute
    {

        public FieldAttribute(string type) : base(type)
        {
        }

        public override Type GetMemberType()
        {
            return (member as FieldInfo).FieldType;
        }

        public override object GetMemberData(object host)
        {
            var ret = (member as FieldInfo).GetValue(host);
            return ret;
        }
        public override void SetMemberData(object host, object value)
        {
            (member as FieldInfo).SetValue(host, value);
        }
    }
}

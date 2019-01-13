using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
namespace RTI
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class PropertyAttribute : MemberAttribute
    {

        public PropertyAttribute(string key) : base(key)
        {
        }

        public override Type GetMemberType()
        {
            return (member as PropertyInfo).PropertyType;
        }

        public override object GetMemberData(object host)
        {
            var ret = (member as PropertyInfo).GetValue(host);
            return ret;
        }
        public override void SetMemberData(object host, object value)
        {
            (member as PropertyInfo).SetValue(host, value);
        }
    }
}

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

        public PropertyAttribute(string key)
        {
            this.inspectInfo = new PropertyInspectInfo();
            inspectInfo.Key = key;
        }

    }
}

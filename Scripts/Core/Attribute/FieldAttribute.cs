using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RTI
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class FieldAttribute : MemberAttribute
    {
        public FieldAttribute(string key)
        {
            this.inspectInfo = new FieldInspectInfo();
            this.inspectInfo.Key = key;
        }
    }
}

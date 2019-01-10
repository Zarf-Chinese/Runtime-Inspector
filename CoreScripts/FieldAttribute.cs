using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
namespace RTI
{
    [AttributeUsage(AttributeTargets.Field, Inherited = true)]
    public class FieldAttribute : Attribute
    {
        public FieldInfo field;
        public System.Type hostType;
        public virtual object GetFieldData(object host)
        {
            return field.GetValue(host);
        }
        public virtual void SetFieldData(object host, object value)
        {
            field.SetValue(host, value);
        }
        public string Type { get; private set; }
        public FieldAttribute(string type) { this.Type = type; }
    }
}

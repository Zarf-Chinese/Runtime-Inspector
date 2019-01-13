using System;
using System.Reflection;
namespace RTI
{
    /// <summary>
    /// 检索信息。
    /// 提供了检索某个成员变量所需要的信息。
    /// </summary>
    public abstract class InspectInfo
    {
        public virtual MemberInfo member { get; set; }
        public abstract Type MemberType { get; }
        public abstract object GetMemberData(object host);
        public abstract void SetMemberData(object host, object value);
        public virtual string Key { get; set; }
    }
}
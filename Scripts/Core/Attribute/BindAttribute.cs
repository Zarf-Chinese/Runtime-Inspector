using System.Reflection;
using System;

namespace RTI
{
    /// <summary>
    /// 绑定标记
    /// >在InspectorManager初始化时，
    /// 会在Bind脚本中寻找所有标记了BindAttribute并继承IEnumerable接口的静态Field，
    /// InspectorManager会从该Field中读取预绑定信息，将关键值Key与相应类型（包括其基类）做绑定。
    /// 但是通过类型绑定来进行检索的类成员仅限于Field与Property
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class BindAttribute : Attribute
    {
        public string name;
        /// <summary>
        /// 绑定标记，**需要绑定在一个公共的Field中**
        /// >在InspectorManager初始化时，
        /// 会在Bind脚本中寻找所有标记了BindAttribute并继承IEnumerable接口的静态Field，
        /// InspectorManager会从该Field中读取预绑定信息，将关键值Key与相应类型（包括其基类）做绑定。
        /// 但是通过类型绑定来进行检索的类成员仅限于Field与Property
        /// </summary>
        /// <param name="name">BindAttribute的名字。如果要使用这个BindAttribute，则需要把这个名字放入InspectorAsset的启用列表中</param>
        public BindAttribute(string name) { this.name = name; }
    }
}
using System;
namespace RTI
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RegistFilterAttribute : Attribute
    {
        public string filterName;
        public int index;
        /// <summary>
        /// 用于标注一个过滤器的注册函数。
        /// 该函数需要是一个静态的公共函数。
        /// 该函数应该返回一个过滤器 (InspectInfoFilter)
        /// </summary>
        /// <param name="filterName">过滤器名称，暂时无用</param>
        /// <param name="index">过滤器生效的优先级，index越低，过滤器越先执行</param>
        public RegistFilterAttribute(string filterName, int index)
        {
            this.filterName = filterName;
            this.index = index;
        }
    }
}
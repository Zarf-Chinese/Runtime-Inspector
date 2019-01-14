using System;
namespace RTI
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RegistFilterAttribute : Attribute
    {
        public string filterName;
        public int index;
        public RegistFilterAttribute(string filterName, int index)
        {
            this.filterName = filterName;
            this.index = index;
        }
    }
}
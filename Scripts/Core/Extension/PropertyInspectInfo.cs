using System;
using System.Reflection;
namespace RTI
{
    public class PropertyInspectInfo : InspectInfo
    {
        public override Type MemberType
        {
            get => (member as PropertyInfo).PropertyType;
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
        [RTI.RegistFilter("Bind", 5)]
        public static InspectorManager.InspectInfoFilter RegistFilter()
        {
            //注册一个通过类型绑定来实现的检索识别过滤器
            InspectorManager.InspectInfoFilter BindFilter = (InspectorManager context, object host, MemberInfo memberInfo, ref InspectInfo inspectInfo) =>
            {
                if (!context.asset.Flags.Contains(InspectFlags.DisableBindProperty)
                    && !context.asset.Flags.Contains(InspectFlags.DisableBind)
                    && inspectInfo == null)
                {
                    if (memberInfo.MemberType == MemberTypes.Property)
                    {
                        //如果类成员是 property
                        var propertyInfo = memberInfo as PropertyInfo;
                        string key = context.GetBindedKey(propertyInfo.PropertyType);
                        if (key != null)
                        {
                            //找到了key
                            inspectInfo = new PropertyInspectInfo();
                            inspectInfo.Key = key;
                        }
                    }
                }
                return false;
            };
            return (BindFilter);
        }
    }
}

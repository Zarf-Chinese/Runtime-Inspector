using System;
using System.Reflection;
namespace RTI
{
    public class FieldInspectInfo : InspectInfo
    {
        public override Type MemberType
        {
            get => (member as FieldInfo).FieldType;
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
        [UnityEngine.RuntimeInitializeOnLoadMethodAttribute]
        static void RegistFilter()
        {
            //注册一个通过类型绑定来实现的检索识别过滤器
            InspectorManager.InspectInfoFilter BindFilter = (InspectorManager context, object host, MemberInfo memberInfo, ref InspectInfo inspectInfo) =>
            {
                if (inspectInfo == null)
                {
                    //如果类成员是 field
                    if (memberInfo.MemberType == MemberTypes.Field)
                    {
                        var fieldInfo = memberInfo as FieldInfo;
                        //检查该成员的类型上是否有绑定标记
                        string key = context.GetBindedKey(fieldInfo.FieldType);
                        if (key != null)
                        {
                            //找到了key
                            inspectInfo = new FieldInspectInfo();
                            inspectInfo.Key = key;
                        }
                    }
                }
            };
            InspectorManager.InspectInfoFilters.Add(BindFilter);
        }
    }
}

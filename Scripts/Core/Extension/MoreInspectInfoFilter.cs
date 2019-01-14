using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
namespace RTI
{
    public class MoreInspectInfoFilter
    {
        [RegistFilter("Public", 2)]
        public static InspectorManager.InspectInfoFilter RegistPublicOnlyFilter()
        {
            //注册一个检索识别过滤器，只允许公共成员被识别
            InspectorManager.InspectInfoFilter PublicOnlyFilter = (InspectorManager context, object host, MemberInfo memberInfo, ref InspectInfo inspectInfo) =>
            {
                if (context.asset.Flags.Contains(InspectFlags.PublicOnly) && inspectInfo != null)
                {
                    if (host.GetType().GetMember(memberInfo.Name, BindingFlags.Public) == null)
                    {
                        //如果不是public
                        return true;
                    }
                }
                return false;
            };
            return PublicOnlyFilter;
        }
    }
}
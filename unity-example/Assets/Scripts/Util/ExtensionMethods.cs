using System;
using System.Linq;
using AWSSignatureV4_S3_Sample.Util;

public static class ExtensionMethods
{
    public static string GetQueryString(this object obj)
    {
        var properties = from p in obj.GetType().GetProperties()
                         where p.GetValue(obj, null) != null
                         select p.Name + "=" + HttpHelpers.UrlEncode(p.GetValue(obj, null).ToString());

        return String.Join("&", properties.ToArray());
    }
}

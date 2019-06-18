using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKMsgParse
{
    public class Cache
    {
        public static Dictionary<string, Cache> results = new Dictionary<string, Cache>();
        public DateTime CreationTime;
        public object Result;
        public Type Type;
        public static void SetRes(string text, Type type, object res)
        {
            if (results.ContainsKey(type.ToString() + text))
                results[type.ToString() + text] = new Cache(type, res);
            else
                results.Add(type.ToString() + text, new Cache(type, res));
        }
        public static object GetRes(string text, Type type, TimeSpan Age)
        {
            if (results.ContainsKey(type.ToString() + text))
            {
                return DateTime.Now.Subtract(results[type.ToString() + text].CreationTime).TotalMinutes < Age.TotalMinutes ? results[type.ToString() + text].Result : null;
            }
            else
                return null;
        }
        public Cache(Type Type, object result)
        {
            Result = result;
            CreationTime = DateTime.Now;
        }
    }
}
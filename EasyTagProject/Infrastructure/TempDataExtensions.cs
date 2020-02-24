using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTagProject.Infrastructure
{
    public static class TempDataExtensions
    {
        public static void SetObject(this ITempDataDictionary tempData, string key, object value)
        {
            tempData[key] = JsonConvert.SerializeObject(value);
        }

        public static T GetObject<T>(this ITempDataDictionary tempData, string key)
        {
            string value = (string)tempData[key];

            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }

        public static T Peek<T>(this ITempDataDictionary tempData, string key)// where T : class
        {
            string value = tempData.Peek(key).ToString();
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }
}

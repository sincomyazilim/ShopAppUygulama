using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopApp.webui.Extensions
{
    public static class TempDataExtensions
    {//buda 613 vıdeoda anlatılanlar yapıldı.. mesajı patıla seklınde tanımladık
        public static void Put<T>(this ITempDataDictionary tempData,string key,T value) where T:class
        {
            tempData[key] = JsonConvert.SerializeObject(value);
        }
        public static T Get<T>(this ITempDataDictionary tempData, string key) where T : class
        {
            object o;
            tempData.TryGetValue(key, out o);
            return o == null ? null : JsonConvert.DeserializeObject<T>((string)o);//o objet oldugu ıcın onu strnge donusturp gonderıyor ((string)o) anlamı odur

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SoundControlServer
{
    public static class JSON
    {
        public static string serialize(object aObject)
        {
            if (aObject != null)
            {
                string serializedObject = JsonConvert.SerializeObject(aObject, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                return serializedObject;
            }

            return string.Empty;
        }

        //public static Container deserialize(string aSerializedObject)
        //{
        //    if (!string.IsNullOrWhiteSpace(aSerializedObject))
        //    {
        //        Container obj = JsonConvert.DeserializeObject<Container>(aSerializedObject);

        //        return obj;
        //    }

        //    return null;
        //}
    }
}

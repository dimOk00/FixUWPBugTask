using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundation.Core.IO
{
    public class JsonPathConverter : JsonConverter
    {
        private string _folderPath;
        private string _pathHeader;

        public JsonPathConverter(string folderPath)
        {
            _folderPath = folderPath;
            _pathHeader = $"file:///{folderPath.Replace('\\', '/')}";
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Uri);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (!(reader.Value is string str) || string.IsNullOrEmpty(str))
                return null;

            if (str.Contains("@@RELATIVE@@"))
            {
                str = str.Replace("@@RELATIVE@@", _pathHeader);
            }

            return new Uri(str);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}

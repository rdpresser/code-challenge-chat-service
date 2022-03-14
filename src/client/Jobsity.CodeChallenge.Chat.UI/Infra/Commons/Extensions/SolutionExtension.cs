using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Linq;

namespace Jobsity.CodeChallenge.Chat.UI.Infra.Commons.Extensions
{
    public static class SolutionExtension
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
        {
            if (source == null)
            {
                return true;
            }

            return !source.Any();
        }

        public static JsonSerializerSettings JsonSettings
        {
            get
            {
                return new JsonSerializerSettings
                {
                    Formatting = Formatting.None,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                    FloatFormatHandling = FloatFormatHandling.DefaultValue,
                    FloatParseHandling = FloatParseHandling.Decimal,
                    DateFormatHandling = DateFormatHandling.IsoDateFormat,
                    Converters = new[] { new IsoDateTimeConverter { DateTimeStyles = System.Globalization.DateTimeStyles.AssumeLocal } }
                };
            }
        }

        public static string ToJson(this object objToJson, bool useSettings = false)
        {
            if (useSettings)
            {
                return JsonConvert.SerializeObject(objToJson, JsonSettings);
            }

            return JsonConvert.SerializeObject(objToJson);
        }

        public static T ToObject<T>(this string stringToObject, bool useSettings = false)
        {
            if (useSettings)
            {
                return JsonConvert.DeserializeObject<T>(stringToObject, JsonSettings);
            }

            return JsonConvert.DeserializeObject<T>(stringToObject);
        }
    }
}
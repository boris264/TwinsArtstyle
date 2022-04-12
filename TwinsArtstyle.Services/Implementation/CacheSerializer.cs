using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwinsArtstyle.Services.Helpers;
using TwinsArtstyle.Services.Interfaces;

namespace TwinsArtstyle.Services.Implementation
{
    public class CacheSerializer : ICacheSerializer
    {
        private readonly Encoding _encoding;

        public CacheSerializer(Encoding? encoding)
        {
            if(encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            else
            {
                _encoding = encoding;
            }
        }

        public T DeserializeFromByteArray<T>(byte[] source) where T : class
        {
           if(source != null)
            {
                return JsonHelper.Deserialize<T>(_encoding.GetString(source));
            }

            return default(T);
        }

        public byte[] SerializeToByteArray<T>(T objectToSerialize) where T : class
        {
            return _encoding.GetBytes(JsonHelper.Serialize(objectToSerialize));
        }

        public T DeserializeFromByteArray<T>(byte[] source, Encoding encoding) where T : class
        {
            if(source != null)
            {
                return JsonHelper.Deserialize<T>(encoding.GetString(source));
            }

            return default(T);
        }

        public byte[] SerializeToByteArray<T>(T objectToSerialize, Encoding encoding) where T : class
        {
            return encoding.GetBytes(JsonHelper.Serialize(objectToSerialize));
        }
    }
}

using System.Text;

namespace TwinsArtstyle.Services.Interfaces
{
    public interface ICacheSerializer
    {
        T DeserializeFromByteArray<T>(byte[] source) where T : class;

        byte[] SerializeToByteArray<T>(T objectToSerialize) where T : class;

        T DeserializeFromByteArray<T>(byte[] source, Encoding encoding) where T : class;

        byte[] SerializeToByteArray<T>(T objectToSerialize, Encoding encoding) where T : class;
    }
}

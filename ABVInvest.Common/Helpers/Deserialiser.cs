using ABVInvest.Common.Constants;
using System.Xml.Serialization;

namespace ABVInvest.Common.Helpers
{
    public class Deserialiser : IDeserialiser
    {
        public async Task<IEnumerable<T>> DeserialiseXmlFile<T>(string fileName)
        {
            var serializer = new XmlSerializer(typeof(T), new XmlRootAttribute(ShortConstants.Common.XmlRootAttr));
            await using var stream = new FileStream(fileName, FileMode.Create);

            return serializer.Deserialize(stream) as T[] ?? [];
        }
    }
}

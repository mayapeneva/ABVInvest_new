using ABVInvest.Common.Constants;
using Microsoft.AspNetCore.Components.Forms;
using System.Xml.Serialization;

namespace ABVInvest.Common.Helpers
{
    public class Deserialiser : IDeserialiser
    {
        private const long MaxFileSize = 1024 * 15;

        public async Task<IEnumerable<T>> DeserialiseXmlFile<T>(string fileName, IBrowserFile file)
        {
            try
            {
                await SaveFile(fileName, file);

                var serializer = new XmlSerializer(typeof(T[]), new XmlRootAttribute(ShortConstants.Common.XmlRootAttr));
                var xmlFileContentString = File.ReadAllText(fileName);
                var deserializedArray = serializer.Deserialize(new StringReader(xmlFileContentString)) as T[];

                return deserializedArray ?? [];
            }
            catch (Exception)
            {
                // TODO: Log the exception
            }

            return [];
        }

        private static async Task SaveFile(string fileName, IBrowserFile file)
        {
            using FileStream fileStream = new(fileName, FileMode.Create);
            await file.OpenReadStream(MaxFileSize).CopyToAsync(fileStream);
        }
    }
}

using Microsoft.AspNetCore.Components.Forms;

namespace ABVInvest.Common.Helpers.Serialisation
{
    public interface IDeserialiser
    {
        Task<IEnumerable<T>> DeserialiseXmlFile<T>(string fileName, IBrowserFile file);
    }
}

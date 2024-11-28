using ABVInvest.Common.Constants;
using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;

namespace ABVInvest.Common.BindingModels
{
    public class FileInputModel
    {
        [Required(ErrorMessage = Messages.Common.RequiredField)]
        [DataType(DataType.Date, ErrorMessage = Messages.Common.DateError)]
        public DateOnly? Date { get; set; }

        [Required(ErrorMessage = Messages.Common.RequiredField)]
        public IReadOnlyList<IBrowserFile> XMLFiles { get; set; }
    }
}

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
        [DataType(DataType.Upload, ErrorMessage = Messages.Common.FileError)]
        public IBrowserFile? XMLFile { get; set; }
    }
}

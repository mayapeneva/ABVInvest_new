using System.ComponentModel.DataAnnotations;

namespace ABVInvest.Common.Validators
{
    public static class ModelValidator
    {
        public static bool IsValid(object obj)
        {
            var validationContext = new ValidationContext(obj);
            var validationResult = new List<ValidationResult>();
            return Validator.TryValidateObject(obj, validationContext, validationResult, true);
            // TODO: log the validation result
        }
    }
}

using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SerialInspector.Model
{
    internal class ListAttribute : ValidationAttribute
    {
        private object[] list;

        public ListAttribute(params object[] list)
        {
             this.list = list;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!list.Any(x => x.Equals(value)))
            {
                string elements = string.Join(", ", list);
                return new ValidationResult($"{validationContext.DisplayName} must be one of: {elements}");
            }

            return null;
        }
    }
}
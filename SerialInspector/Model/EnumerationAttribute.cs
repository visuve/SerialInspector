using System;
using System.ComponentModel.DataAnnotations;

namespace SerialInspector.Model
{
    internal class EnumerationAttribute : ValidationAttribute
    {
        private Type type;

        public EnumerationAttribute(Type type)
        {
            this.type = type;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            Array values = Enum.GetValues(type);

            foreach (object o in values)
            {
                if (o.Equals(value))
                {
                    return null;
                }
            }

            string elements = string.Join(", ", (int[])values);
            return new ValidationResult($"{validationContext.DisplayName} must be one of: {elements}");
        }
    }
}
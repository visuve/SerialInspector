using System;
using System.ComponentModel.DataAnnotations;
using System.IO.Ports;
using System.Linq;

namespace SerialInspector.Model
{
    internal class PortNameAttribute : ValidationAttribute
    {
        private static readonly bool IsUnitTest = 
            AppDomain.CurrentDomain.GetAssemblies().Any(
                a => a.FullName.StartsWith("Microsoft.VisualStudio.TestPlatform.TestFramework"));

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult($"Port name cannot be null!");
            }

            string[] ports = IsUnitTest ? new string[] { "COM1" } : SerialPort.GetPortNames();

            if (!ports.Any(x => x.Equals(value)))
            {
                return new ValidationResult($"Could not find a device in port: {value}.");
            }

            return null;
        }
    }
}
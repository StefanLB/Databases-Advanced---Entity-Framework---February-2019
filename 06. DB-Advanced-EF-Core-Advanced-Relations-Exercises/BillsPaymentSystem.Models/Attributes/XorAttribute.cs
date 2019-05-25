using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BillsPaymentSystem.Models.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class XorAttribute : ValidationAttribute
    {
        private readonly string targetAttribute;

        public XorAttribute(string targetAttribute)
        {
            this.targetAttribute = targetAttribute;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var property = validationContext.ObjectType
                .GetProperty(targetAttribute)
                .GetValue(validationContext.ObjectInstance);

            if ((property == null && value != null) || (property != null && value == null))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult("The two properties must have opposite values!");
        }
    }
}

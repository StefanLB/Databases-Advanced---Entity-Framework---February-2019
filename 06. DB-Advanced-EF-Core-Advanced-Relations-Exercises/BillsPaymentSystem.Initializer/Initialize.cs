using BillsPaymentSystem.Data;
using BillsPaymentSystem.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BillsPaymentSystem.Initializer
{
    public class Initialize
    {
        public static void Seed(BillsPaymentSystemContext context)
        {
            InsertValues(UsersInitializer.GetUsers(), context);
            InsertValues(CreditCardInitializer.GetCreditCards(), context);
            InsertValues(BankAccountInitializer.GetBankAccounts(), context);
            InsertValues(PaymentMethodInitializer.GetPaymentMethods(), context);
        }

        private static void InsertValues<T>(T[] entities, BillsPaymentSystemContext context)
            where T: class
        {
            for (int i = 0; i < entities.Length; i++)
            {
                if (IsValid(entities[i]))
                {
                    context.Add(entities[i]);
                }
            }

            context.SaveChanges();
        }

        private static bool IsValid<T>(T obj)
        {
            var validationContext = new ValidationContext(obj);
            var result = new List<ValidationResult>();

            return Validator.TryValidateObject(obj, validationContext, result, true);

        }
    }
}

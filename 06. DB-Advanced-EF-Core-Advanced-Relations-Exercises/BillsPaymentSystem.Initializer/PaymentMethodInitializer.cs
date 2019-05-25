using BillsPaymentSystem.Models;
using BillsPaymentSystem.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace BillsPaymentSystem.Initializer
{
    public class PaymentMethodInitializer
    {
        public static PaymentMethod[] GetPaymentMethods()
        {
            return new PaymentMethod[]
            {
                new PaymentMethod(){ Type = PaymentType.BankAccount, UserId = 1, BankAccountId = 2},
                new PaymentMethod(){ Type = PaymentType.BankAccount, UserId = 1, BankAccountId = 4},
                new PaymentMethod(){ Type = PaymentType.BankAccount, UserId = 1, BankAccountId = 6},
                new PaymentMethod(){ Type = PaymentType.CreditCard, UserId = 1, CreditCardId = 2},
                new PaymentMethod(){ Type = PaymentType.CreditCard, UserId = 1, CreditCardId = 4},
                new PaymentMethod(){ Type = PaymentType.BankAccount, UserId = 1, BankAccountId = 1},
                new PaymentMethod(){ Type = PaymentType.CreditCard, UserId = 4, CreditCardId = 3},
                new PaymentMethod(){ Type = PaymentType.CreditCard, UserId = 4, CreditCardId = 1},
                new PaymentMethod(){ Type = PaymentType.BankAccount, UserId = 7, BankAccountId = 5},
                new PaymentMethod(){ Type = PaymentType.CreditCard, UserId = 3, CreditCardId = 1},
                new PaymentMethod(){ Type = PaymentType.BankAccount, UserId = 3, BankAccountId = 3},
                new PaymentMethod(){ Type = PaymentType.CreditCard, UserId = 3, CreditCardId = 4},
                new PaymentMethod(){ Type = PaymentType.BankAccount, UserId = 3, BankAccountId = 5}
            };
        }
    }
}

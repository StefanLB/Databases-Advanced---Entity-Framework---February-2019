using BillsPaymentSystem.Models;
using System;

namespace BillsPaymentSystem.Initializer
{
    public class BankAccountInitializer
    {
        public static BankAccount[] GetBankAccounts()
        {
            return new BankAccount[]
            {
                new BankAccount(1000.00m,"test1","SWIFT1"),
                new BankAccount(10000.00m,"test2","SWIFT2"),
                new BankAccount(100000.00m,"test3","SWIFT3"),
                new BankAccount(1000000.00m,"test4","SWIFT4"),
                new BankAccount(10000000.00m,"test5","SWIFT5"),
                new BankAccount(100000000.00m,"test6","SWIFT6")
            };
        }
    }
}

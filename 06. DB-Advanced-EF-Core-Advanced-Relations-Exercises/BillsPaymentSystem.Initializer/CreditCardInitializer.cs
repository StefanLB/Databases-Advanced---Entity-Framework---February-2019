using BillsPaymentSystem.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BillsPaymentSystem.Initializer
{
    public class CreditCardInitializer
    {
        public static CreditCard[] GetCreditCards()
        {
            return new CreditCard[]
            {
                new CreditCard(500, 200, DateTime.Parse("01-01-2020")),
                new CreditCard(5000, 2000, DateTime.Parse("02-02-2021")),
                new CreditCard(50000, 20000, DateTime.Parse("03-03-2022")),
                new CreditCard(500000, 200000, DateTime.Parse("04-04-2023")),
                new CreditCard(50000000, 2000000, DateTime.Parse("05-05-2024")),
                new CreditCard(500000000, 20000000, DateTime.Parse("06-06-2025"))
            };
        }
    }
}

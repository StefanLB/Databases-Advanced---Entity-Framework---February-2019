using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BillsPaymentSystem.Models
{
    [Table("CreditCards")]
    public class CreditCard
    {
        public CreditCard(decimal limit, decimal moneyOwed, DateTime expirationDate)
        {
            this.Limit = limit;
            this.MoneyOwed = moneyOwed;
            this.ExpirationDate = expirationDate;
        }

        [Key]
        public int CreditCardId { get; private set; }

        public decimal Limit { get; private set; }

        public decimal MoneyOwed { get; private set; }

        [NotMapped]
        public decimal LimitLeft => this.Limit - this.MoneyOwed;

        public DateTime ExpirationDate { get; private set; }

        public PaymentMethod PaymentMethod { get; private set; }

        public void Deposit(decimal amount)
        {
            this.MoneyOwed -= amount;
        }

        public void Withdraw(decimal amount)
        {
            if (amount>this.LimitLeft)
            {
                throw new ArgumentException("Not enough money in the card!");
            }

            this.MoneyOwed += amount;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("--ID: " + this.CreditCardId);
            sb.AppendLine("---Limit: " + this.Limit);
            sb.AppendLine("---Money Owed: " + this.MoneyOwed);
            sb.AppendLine("---Limit Left: " + this.LimitLeft);
            sb.AppendLine("---Expiration Date: " + this.ExpirationDate);

            return sb.ToString().Trim();
        }

    }
}

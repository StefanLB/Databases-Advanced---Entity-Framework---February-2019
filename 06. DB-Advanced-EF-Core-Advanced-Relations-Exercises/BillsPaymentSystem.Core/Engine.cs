using BillsPaymentSystem.Core.Contracts;
using BillsPaymentSystem.Core.IO.Contracts;
using BillsPaymentSystem.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Text;

namespace BillsPaymentSystem.Core
{
    public class Engine : IEngine
    {
        private readonly IReader reader;
        private readonly IWriter writer;
        private readonly BillsPaymentSystemContext context;

        public Engine(IReader reader, IWriter writer, BillsPaymentSystemContext context)
        {
            this.reader = reader;
            this.writer = writer;
            this.context = context;
        }

        public void Run()
        {
            while (true)
            {
                try
                {
                    var result = string.Empty;
                    var command = this.reader.ReadLine().Split();

                    if (command.Length == 1 && command[0].ToLower() == "end")
                    {
                        Environment.Exit(0);
                    }
                    else if (command.Length == 1)
                    {
                        var userId = int.Parse(command[0]);

                        result = this.FindUser(userId);

                    }
                    else if (command.Length == 2)
                    {
                        var userId = int.Parse(command[0]);
                        var amount = decimal.Parse(command[1]);

                        result = this.PayBills(userId, amount);

                        this.context.SaveChanges();
                    }

                    this.writer.WriteLine(result);
                }
                catch (Exception)
                {
                    this.writer.WriteLine("Unrecognized operation. Please try again or enter \"END\" to Exit.");
                }
            }
        }

        private string PayBills(int userId, decimal amount)
        {
            var user = this.context.Users
                .Include(x => x.PaymentMethods)
                .ThenInclude(x => x.BankAccount)
                .Include(x => x.PaymentMethods)
                .ThenInclude(x => x.CreditCard)
                .FirstOrDefault(x => x.UserId == userId);

            var bankAccountTotalAmount = user.PaymentMethods
                .Where(x => x.BankAccount != null)
                .Sum(x => x.BankAccount.Balance);

            var creditCardTotalAmount = user.PaymentMethods
                .Where(x => x.CreditCard != null)
                .Sum(x => x.CreditCard.LimitLeft);

            decimal total = bankAccountTotalAmount + creditCardTotalAmount;

            StringBuilder sb = new StringBuilder();
            if (total<amount)
            {
                sb.AppendLine("Insufficient funds!");
            }
            else
            {
                var accounts = user.PaymentMethods
                    .Where(x => x.BankAccount != null)
                    .Select(x => x.BankAccount)
                    .OrderBy(x => x.BankAccountId)
                    .ToList();

                foreach (var acc in accounts)
                {
                    if (acc.Balance<amount)
                    {
                        amount -= acc.Balance;
                        acc.Withdraw(acc.Balance);
                    }
                    else
                    {
                        acc.Withdraw(amount);
                        amount = 0;
                        break;
                    }
                }

                var creditCards = user.PaymentMethods
                    .Where(x => x.CreditCard != null)
                    .Select(x => x.CreditCard)
                    .OrderBy(x => x.CreditCardId)
                    .ToList();

                foreach (var card in creditCards)
                {
                    if (card.LimitLeft < amount)
                    {
                        amount -= card.LimitLeft;
                        card.Withdraw(card.LimitLeft);
                    }
                    else
                    {
                        card.Withdraw(amount);
                        amount = 0;
                        break;
                    }
                }
            }
                return sb.ToString().Trim();
        }

        private string FindUser(int userId)
        {
            var userInfo = context.Users
                .Select(u => new
                {
                    Id = u.UserId,
                    Name = u.FirstName + " " + u.LastName,
                    BankAccounts = u.PaymentMethods.Where(x => x.BankAccount != null).Select(pm => pm.BankAccount).ToList(),
                    CreditCards = u.PaymentMethods.Where(x => x.CreditCard != null).Select(pm => pm.CreditCard).ToList()
                })
                .SingleOrDefault(u => u.Id == userId);

            StringBuilder sb = new StringBuilder();

            if (userInfo != null)
            {
                sb.AppendLine("User: " + userInfo.Name);

                sb.AppendLine("Bank Accounts:");

                foreach (var bankAcc in userInfo.BankAccounts)
                {
                    sb.AppendLine(bankAcc.ToString());
                }

                sb.AppendLine("Credit Cards:");
                foreach (var creditCard in userInfo.CreditCards)
                {
                    sb.AppendLine(creditCard.ToString());
                }
            }
            else
            {
                sb.AppendLine($"User with id {userId} not found!");
            }

            return sb.ToString().Trim();
        }
    }
}

using MyApp.Core.Commands.Contracts;
using MyApp.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace MyApp.Core.Commands
{
    public class SetBirthdayCommand : ICommand
    {
        private readonly MyAppContext context;

        public SetBirthdayCommand(MyAppContext context)
        {
            this.context = context;
        }

        public string Execute(string[] inputArgs)
        {
            int employeeId = int.Parse(inputArgs[0]);
            string dateString = inputArgs[1];
            string format = "dd-MM-yyyy";

            DateTime birthday = DateTime.ParseExact(dateString, format,
                CultureInfo.InvariantCulture);

            var employee = this.context.Employees.Find(employeeId);

            employee.Birthday = birthday;

            this.context.SaveChanges();

            return "Command completed successfully!";
        }
    }
}

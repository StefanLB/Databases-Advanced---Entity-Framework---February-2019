using AutoMapper;
using MyApp.Core.Commands.Contracts;
using MyApp.Core.ViewModels;
using MyApp.Data;
using MyApp.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyApp.Core.Commands
{
    public class EmployeePersonalInfoCommand : ICommand
    {
        private readonly MyAppContext context;
        private readonly Mapper mapper;

        public EmployeePersonalInfoCommand(MyAppContext context, Mapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public string Execute(params string[] args)
        {
            int employeeId = int.Parse(args[0]);

            Employee employee = this.context.Employees.Find(employeeId);

            var employeeInfo = this.mapper.CreateMappedObject<EmployeePersonalInfoDto>(employee);

            return $"ID: {employee.Id} - {employee.FirstName} {employee.LastName} - {employee.Salary:f2} {Environment.NewLine}" +
                $"Birthday: {employee.Birthday?.ToString("dd-MM-yyyy")} {Environment.NewLine}" +
                $"Address: { employee.Address}";
        }
    }
}

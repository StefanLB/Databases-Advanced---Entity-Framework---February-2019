﻿using Microsoft.EntityFrameworkCore;
using SoftUni.Data;
using SoftUni.Models;
using System;
using System.Linq;
using System.Text;

namespace SoftUni
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            using(var context = new SoftUniContext())
            {
                var result = RemoveTown(context); //Swap methods for each exercise
                Console.WriteLine(result);
            }
        }

        //Exercise 3
        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context.Employees
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.MiddleName,
                    e.JobTitle,
                    e.Salary,
                    e.EmployeeId
                })
                .OrderBy(x => x.EmployeeId)
                .ToList();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} {employee.MiddleName} {employee.JobTitle} {employee.Salary:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        //Exercise 4
        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context.Employees
                .Where(s => s.Salary > 50000)
                .Select(x => new
                {
                    FirstName = x.FirstName,
                    Salary = x.Salary
                })
                .OrderBy(f => f.FirstName)
                .ToList();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} - {employee.Salary:F2}");
            }

            return sb.ToString()
                .TrimEnd();
        }

        //Exercise 5
        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context.Employees
                .Where(d => d.Department.Name == "Research and Development")
                .Select(x => new
                {
                    x.FirstName,
                    x.LastName,
                    x.Department.Name,
                    x.Salary
                })
                .OrderBy(s => s.Salary)
                .ThenByDescending(f=>f.FirstName);

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} from Research and Development - ${employee.Salary:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        //Exercise 6
        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            var address = new Address
            {
                AddressText = "Vitoshka 15",
                TownId = 4
            };

            context.Addresses.Add(address); // can be skipped, EF will figure out to add the address to the addresses table

            var nakov = context.Employees.FirstOrDefault(x => x.LastName == "Nakov");

            nakov.Address = address;

            context.SaveChanges();

            var employeeAddresses = context.Employees
                .OrderByDescending(x => x.AddressId)
                .Select(a => a.Address.AddressText)
                .Take(10)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var employeeAddress in employeeAddresses)
            {
                sb.AppendLine($"{employeeAddress}");
            }

            return sb.ToString().TrimEnd();
        }

        //Exercise 7
        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            var employees = context.Employees
                .Where(p => p.EmployeesProjects.Any(s => s.Project.StartDate.Year >= 2001 && s.Project.StartDate.Year <= 2003))
                .Select(x => new
                {
                    EmployeeFullName = x.FirstName + " " + x.LastName,
                    ManagerFullName = x.Manager.FirstName + " " + x.Manager.LastName,
                    Projects = x.EmployeesProjects.Select(p => new
                    {
                        ProjectName = p.Project.Name,
                        StartDate = p.Project.StartDate,
                        EndDate = p.Project.EndDate
                    })
                })
                .Take(10)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.EmployeeFullName} - Manager: {employee.ManagerFullName}");

                foreach (var project in employee.Projects)
                {
                    var startDate = project.StartDate.ToString("M/d/yyyy h:mm:ss tt");

                    var endDate = project.EndDate.HasValue ?
                        project.EndDate.Value.ToString("M/d/yyyy h:mm:ss tt") : "not finished";


                    sb.AppendLine($"--{project.ProjectName} - {startDate} - {endDate}");
                }
            }

            return sb.ToString().TrimEnd();
        }

        //Exercise 8
        public static string GetAddressesByTown(SoftUniContext context)
        {
            var sb = new StringBuilder();

            context.Addresses
                  .Select(a => new
                  {
                      a.AddressText,
                      a.Town.Name,
                      EmployeesCount = a.Employees.Count
                  })
                  .Take(10)
                  .OrderByDescending(a => a.EmployeesCount)
                  .ThenBy(a => a.Name)
                  .ThenBy(a => a.AddressText)
                  .ToList()
                  .ForEach(a => sb.AppendLine($"{a.AddressText}, {a.Name} - {a.EmployeesCount} employees"));

            return sb.ToString().TrimEnd();
        }

        //Exercise 9
        public static string GetEmployee147(SoftUniContext context)
        {
            var employee = context.Employees
                .Select(e => new
                {
                    e.EmployeeId,
                    e.FirstName,
                    e.LastName,
                    e.JobTitle,
                    ProjectNames = e.EmployeesProjects.Select(p => p.Project.Name).ToList()
                })
                .SingleOrDefault(e => e.EmployeeId == 147);

            StringBuilder sb = new StringBuilder();

            sb.AppendLine(employee.FirstName + " " + employee.LastName + " - " + employee.JobTitle);

            foreach (var project in employee.ProjectNames.OrderBy(p=>p))
            {
                sb.AppendLine(project);
            }

            return sb.ToString().TrimEnd();

        }

        //Exercise 10
        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            var sb = new StringBuilder();

            var departments = context.Departments
                .Include(d => d.Employees)
                .Include(d => d.Manager)
                .Where(d => d.Employees.Count > 5)
                .OrderBy(d => d.Employees.Count)
                .ThenBy(d => d.Name)
                .ToList();

            foreach (var d in departments)
            {
                sb.AppendLine($"{d.Name} - {d.Manager}");

                foreach (var e in d.Employees.OrderBy(e=>e.FirstName).ThenBy(e=>e.LastName))
                {
                    sb.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle}");
                }
            }

            return sb.ToString().TrimEnd();

        }


        //Exercise 11
        public static string GetLatestProjects(SoftUniContext context)
        {
            var projects = context.Projects
                .OrderByDescending(p => p.StartDate.Ticks)
                .Select(p => new
                {
                    p.Name,
                    p.Description,
                    p.StartDate
                })
                .Take(10)
                .OrderBy(p => p.Name)
                .ToList();

            var sb = new StringBuilder();

            foreach (var p in projects)
            {
                sb.AppendLine(p.Name);
                sb.AppendLine(p.Description);
                sb.AppendLine(p.StartDate.ToString("M/d/yyyy h:mm:ss tt"));
            }

            return sb.ToString().TrimEnd();
        }

        //Exercise 12
        public static string IncreaseSalaries(SoftUniContext context)
        {
            var employees = context.Employees
                .Where(e => e.Department.Name == "Engineering" ||
                            e.Department.Name == "Tool Design" ||
                            e.Department.Name == "Marketing" ||
                            e.Department.Name == "Information Services")
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToList();

            var sb = new StringBuilder();

            foreach (var e in employees)
            {
                e.Salary *= 1.12m;
                sb.AppendLine($"{e.FirstName} {e.LastName} (${e.Salary:F2})");
            }

            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        //Exercise 13
        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            var employees = context.Employees
                .Where(e => EF.Functions.Like(e.FirstName, "Sa%"))
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.JobTitle,
                    e.Salary
                })
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var emp in employees)
            {
                sb.AppendLine($"{emp.FirstName} {emp.LastName} - {emp.JobTitle} - (${emp.Salary:F2})");
            }

            return sb.ToString().TrimEnd();
        }

        //Exercise 14
        public static string DeleteProjectById(SoftUniContext context)
        {
            var empProjects = context.EmployeesProjects.Where(p => p.ProjectId == 2).ToList();
            var project = context.Projects.SingleOrDefault(p=>p.ProjectId == 2);

            context.EmployeesProjects.RemoveRange(empProjects);
            context.Projects.Remove(project);

            context.SaveChanges();

            var sb = new StringBuilder();

            context.Projects
                .Take(10)
                .Select(p => p.Name)
                .ToList()
                .ForEach(p => sb.AppendLine(p));

            return sb.ToString().TrimEnd();
        }

        //Exercise 15
        public static string RemoveTown(SoftUniContext context)
        {
            var town = context.Towns
                .Include(x => x.Addresses)
                .FirstOrDefault(x=>x.Name=="Seattle");

            var townId = town.TownId;
            var addressesCount = town.Addresses.Count;

            context.Employees
                .Where(e => e.Address.TownId == townId)
                .ToList()
                .ForEach(e => e.AddressId = null);

            context.Addresses.RemoveRange(town.Addresses);
            context.Towns.Remove(town);
            context.SaveChanges();

            return $"{addressesCount} addresses in {town.Name} were deleted";
        }
    }
}

namespace SoftJail.DataProcessor
{

    using Data;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using AutoMapper;
    using Data.Models;
    using Data.Models.Enums;
    using ImportDto;
    using Newtonsoft.Json;
    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

    public class Deserializer
    {
        private const string INVALID_DATA_MESSAGE = "Invalid Data";
        private const string DATETIME_FORMAT = "dd/MM/yyyy";

        public static string ImportDepartmentsCells(SoftJailDbContext context, string jsonString)
        {
            var departmentDtos = (DepartmentDto[])JsonConvert.DeserializeObject(jsonString, typeof(DepartmentDto[]));

            var validDepartments = new List<Department>();
            var result = new StringBuilder();

            foreach (var departmentDto in departmentDtos)
            {
                if (!IsValid(departmentDto))
                {
                    result.AppendLine(INVALID_DATA_MESSAGE);
                    continue;
                }

                var allCellsAreValid = departmentDto.Cells.All(IsValid);

                if (!allCellsAreValid)
                {
                    result.AppendLine(INVALID_DATA_MESSAGE);
                    continue;
                }

                var validDepartment = Mapper.Map<Department>(departmentDto);
                validDepartments.Add(validDepartment);
                result.AppendLine($"Imported {validDepartment.Name} with {validDepartment.Cells.Count} cells");
            }
            context.Departments.AddRange(validDepartments);
            context.SaveChanges();

            return result.ToString().Trim();
        }

        public static string ImportPrisonersMails(SoftJailDbContext context, string jsonString)
        {
            var prisonerDtos = (PrisonerDto[])JsonConvert.DeserializeObject(jsonString, typeof(PrisonerDto[]));

            var validPrisoners = new List<Prisoner>();
            var result = new StringBuilder();

            foreach (var prisonerDto in prisonerDtos)
            {
                if (!IsValid(prisonerDto))
                {
                    result.AppendLine(INVALID_DATA_MESSAGE);
                    continue;
                }

                var allMailsAreValid = prisonerDto.Mails.All(IsValid);

                if (!allMailsAreValid)
                {
                    result.AppendLine(INVALID_DATA_MESSAGE);
                    continue;
                }

                var incarcerationDate = DateTime.ParseExact(prisonerDto.IncarcerationDate, DATETIME_FORMAT, CultureInfo.InvariantCulture);
                DateTime? releaseDate = null;

                if (prisonerDto.ReleaseDate != null)
                {
                    releaseDate = DateTime.ParseExact(prisonerDto.ReleaseDate, DATETIME_FORMAT, CultureInfo.InvariantCulture);
                }

                if (releaseDate < incarcerationDate)
                {
                    result.AppendLine(INVALID_DATA_MESSAGE);
                    continue;
                }

                var validPrisoner = new Prisoner
                {
                    FullName = prisonerDto.FullName,
                    Nickname = prisonerDto.Nickname,
                    Age = prisonerDto.Age,
                    IncarcerationDate = incarcerationDate,
                    ReleaseDate = releaseDate,
                    Bail = prisonerDto.Bail,
                    CellId = prisonerDto.CellId,
                    Mails = Mapper.Map<Mail[]>(prisonerDto.Mails)
                };

                validPrisoners.Add(validPrisoner);
                result.AppendLine($"Imported {validPrisoner.FullName} {validPrisoner.Age} years old");
            }


            context.Prisoners.AddRange(validPrisoners);
            context.SaveChanges();
            return result.ToString().Trim();
        }

        public static string ImportOfficersPrisoners(SoftJailDbContext context, string xmlString)
        {
            var serializer = new XmlSerializer(typeof(OfficerDto[]), new XmlRootAttribute("Officers"));
            var officerDtos = (OfficerDto[])serializer.Deserialize(new StringReader(xmlString));

            var result = new StringBuilder();
            var validOfficers = new List<Officer>();

            foreach (var officerDto in officerDtos)
            {
                if (!IsValid(officerDto))
                {
                    result.AppendLine(INVALID_DATA_MESSAGE);
                    continue;
                }

                var validPosition = Enum.TryParse<Position>(officerDto.Position, out var position);
                var validWeapon = Enum.TryParse<Weapon>(officerDto.Weapon, out var weapon);

                if (!validPosition || !validWeapon)
                {
                    result.AppendLine(INVALID_DATA_MESSAGE);
                    continue;
                }
                
                var validOfficer = new Officer
                {
                    FullName = officerDto.FullName,
                    Salary = officerDto.Salary,
                    Position = position,
                    Weapon = weapon,
                    DepartmentId = officerDto.DepartmentId,
                };

                var officerPrisoners = officerDto.Prisoners
                    .Select(p => new OfficerPrisoner
                    {
                        PrisonerId = p.Id,
                        Officer = validOfficer
                    })
                    .ToArray();

                validOfficer.OfficerPrisoners = officerPrisoners;
                validOfficers.Add(validOfficer);

                result.AppendLine($"Imported {validOfficer.FullName} ({validOfficer.OfficerPrisoners.Count} prisoners)");
            }

            context.AddRange(validOfficers);
            context.SaveChanges();

            return result.ToString().Trim();
        }

        private static bool IsValid(object obj)
        {
            var validationContext = new ValidationContext(obj);
            var validationResults = new List<ValidationResult>();

            var result = Validator.TryValidateObject(obj, validationContext, validationResults, true);
            return result;
        }
    }
}
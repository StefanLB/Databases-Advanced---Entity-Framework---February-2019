namespace SoftJail.DataProcessor
{
    using Data;
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using ExportDto;
    using Newtonsoft.Json;
    using Formatting = Newtonsoft.Json.Formatting;

    public class Serializer
    {
        public static string ExportPrisonersByCells(SoftJailDbContext context, int[] ids)
        {
            var listOfIds = ids.ToList();
            var prisoners = context.Prisoners
                .Where(p => listOfIds.Contains(p.Id))
                .Select(p => new
                {
                    Id = p.Id,
                    Name = p.FullName,
                    CellNumber = p.Cell.CellNumber,
                    Officers = p.PrisonerOfficers
                        .Select(po => new
                        {
                            OfficerName = po.Officer.FullName,
                            Department = po.Officer.Department.Name
                        })
                        .OrderBy(o => o.OfficerName)
                        .ToArray(),
                    TotalOfficerSalary = p.PrisonerOfficers.Sum(po => po.Officer.Salary)
                })
                .OrderBy(p => p.Name)
                .ThenBy(p => p.Id)
                .ToArray();

            var result = JsonConvert.SerializeObject(prisoners, Formatting.Indented);
            return result;
        }

        public static string ExportPrisonersInbox(SoftJailDbContext context, string prisonersNames)
        {
            var names = prisonersNames.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);

            var prisoners = context.Prisoners
                .Where(p => names.Contains(p.FullName))
                .Select(p => new 
                {
                    Id = p.Id,
                    Name = p.FullName,
                    IncarcerationDate = p.IncarcerationDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    EncryptedMessages = p.Mails
                        .Select(m => new 
                        {
                            Description = m.Description
                        })
                        .ToArray()
                })
                .OrderBy(p => p.Name)
                .ThenBy(p => p.Id)
                .ToArray()
                .Select(x => new PrisonerDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    IncarcerationDate = x.IncarcerationDate,
                    EncryptedMessages = x.EncryptedMessages
                        .Select(m => new EncryptedMessageDto
                        {
                            Description = new string(m.Description.Reverse().ToArray())
                        })
                        .ToArray()
                })
                .ToArray();

            var serializer = new XmlSerializer(typeof(PrisonerDto[]), new XmlRootAttribute("Prisoners"));
            var sb = new StringBuilder();
            var namespaces = new XmlSerializerNamespaces(new []{XmlQualifiedName.Empty});
            serializer.Serialize(new StringWriter(sb), prisoners, namespaces);

            return sb.ToString().Trim();
        }
    }
}
namespace SoftJail.DataProcessor.ImportDto
{
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;

    [XmlType("Prisoner")]
    public class OfficerPrisonerDto
    {
        [Required]
        [XmlAttribute("id")]
        public int Id { get; set; }
    }
}
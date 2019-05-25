namespace SoftJail.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Prisoner
    {
        private ICollection<Mail> mails;
        private ICollection<OfficerPrisoner> prisonerOfficers;

        public Prisoner()
        {
            this.mails = new HashSet<Mail>();
            this.prisonerOfficers = new HashSet<OfficerPrisoner>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 3)]
        public string FullName { get; set; }

        [Required]
        [RegularExpression(@"The [A-Z][a-z]*")]
        public string Nickname { get; set; }

        [Required]
        [Range(18, 65)]
        public int Age { get; set; }

        [Required]
        public DateTime IncarcerationDate { get; set; }

        public DateTime? ReleaseDate { get; set; }

        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal? Bail { get; set; }

        public int? CellId { get; set; }
        public Cell Cell { get; set; }

        public virtual  ICollection<Mail> Mails
        {
            get => this.mails;
            set => this.mails = value;
        }

        public virtual ICollection<OfficerPrisoner> PrisonerOfficers
        {
            get => this.prisonerOfficers;
            set => this.prisonerOfficers = value;
        }
    }
}
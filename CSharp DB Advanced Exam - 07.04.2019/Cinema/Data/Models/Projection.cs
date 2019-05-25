using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Cinema.Data.Models
{
    public class Projection
    {
        public Projection()
        {
            Tickets = new HashSet<Ticket>();
        }

        [Key]
        public int Id { get; set; }

        //TODO: May need to add "required" attribute
        public int MovieId { get; set; }
        public Movie Movie { get; set; }

        //TODO: May need to add "required" attribute
        public int HallId { get; set; }
        public Hall Hall { get; set; }

        //TODO: May need to add "required" attribute
        public DateTime DateTime { get; set; }

        public ICollection<Ticket> Tickets { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Cinema.Data.Models
{
    public class Seat
    {
        [Key]
        public int Id { get; set; }

        //TODO: May need to add "required" attribute
        public int HallId { get; set; }
        public Hall Hall { get; set; }
    }
}

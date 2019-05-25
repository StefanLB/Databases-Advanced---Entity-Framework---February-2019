﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Cinema.Data.Models
{
    public class Ticket
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
        public decimal Price { get; set; }

        //TODO: May need to add "required" attribute
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        //TODO: May need to add "required" attribute
        public int ProjectionId { get; set; }
        public Projection Projection { get; set; }

    }
}

﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SoftJail.Data.Models
{
    public class Mail
    {
        public int Id { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Sender { get; set; }

        [Required]
        public string Address { get; set; }

        public int PrisonerId { get; set; }
        public Prisoner Prisoner { get; set; }
    }
}
//•	Id – integer, Primary Key
//•	Description– text (required)
//•	Sender – text(required)
//•	Address – text, consisting only of letters, spaces and numbers, which ends with “ str.” (required) (Example: “62 Muir Hill str.“)
//•	PrisonerId - integer, foreign key(required)
//•	Prisoner – the mail's Prisoner (required)

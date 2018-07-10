﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VehicleCostsMonitor.Models
{
    public class Manufacturer
    {
        public Manufacturer()
        {
            this.Models = new HashSet<Model>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public IEnumerable<Model> Models { get; set; }
    }
}
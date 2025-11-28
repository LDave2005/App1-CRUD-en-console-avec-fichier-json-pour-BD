using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1.Modeles
{
    public class Product
    {
        public int id { get; set; }
        public string? nom { get; set; }
        public string? description { get; set; }
        public decimal prix { get; set; }
        public int stock { get; set; }
    }
}

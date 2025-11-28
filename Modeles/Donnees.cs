using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1.Modeles
{
    public class Donnees
    {
        public List<User> users { get; set; } = new();
        public List<Client> clients { get; set; } = new();
        public List<Product> products { get; set; } = new();
    }
}

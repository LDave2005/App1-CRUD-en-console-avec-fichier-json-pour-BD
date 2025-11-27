using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1
{
    public class Client
    {
        public int id { get; set; }
        public string? nom { get; set; }
        public int numeroTel { get; set; }
    }

    public class Product
    {
        public int id { get; set; }
        public string? nom { get; set; }
        public string? description { get; set; }
        public decimal prix { get; set; }
        public int stock { get; set; }
    }

    public class User
    {
        public int id { get; set; }
        public string? nom { get; set; }
        public string? email { get; set; }
        public string? motDePasseHash { get; set; }
        public string statut { get; set; } = "actif"; // "actif" ou "inactif"
        public string role { get; set; } = "user"; //user ou admin
        public DateTime createdAt { get; set; } = DateTime.UtcNow;
    }

    public class Donnees
    {
        public List<User> users { get; set; } = new();
        public List<Client> clients { get; set; } = new();
        public List<Product> products { get; set; } = new();
    }
}

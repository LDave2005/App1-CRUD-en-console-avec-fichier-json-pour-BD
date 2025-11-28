using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1.Modeles
{
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
}

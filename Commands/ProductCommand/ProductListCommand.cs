using App1.Core;
using App1.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1.Commands.ProductCommand
{
    public class ProductListCommand : ISousCommand
    {
        public string Name => "list";
        public List<Parametre> parametres { get; } = new List<Parametre>();
        public void Execute(Dictionary<string, string> args)
        {
            ProductView product = new ProductView();
            Console.ForegroundColor = ConsoleColor.Yellow;
            product.AfficherProduitsTable();
            Console.ForegroundColor= ConsoleColor.White;
        }
    }
}

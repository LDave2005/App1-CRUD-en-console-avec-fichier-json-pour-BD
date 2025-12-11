using App1.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1.Commands.ProductCommand
{
    public class ProductCommand : ICommand2
    {
        public string Name => "product";
        public Dictionary<string, ISousCommand> SousCommands { get; } = new Dictionary<string, ISousCommand>();
        public ProductCommand()
        {
            SousCommands["list"] = new ProductListCommand();
            SousCommands["create"] = new ProductAddCommand();
            SousCommands["modify"] = new ProductModifyCommand();
            SousCommands["delete"] = new ProductDeleteCommand();
        }
        public void Execute(Dictionary<string,string> args)
        {

        }
        
    }
}

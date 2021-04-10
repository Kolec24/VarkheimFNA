using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Varkheim
{
    static class Program
    {
        static void Main(string[] args)
        {
            using (VGame game = new VGame())
            {
                game.Run();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JABBERNAUT
{
    abstract class Game
    {
        public Game()
        {

        }
        abstract public string GameLoop(string arg)
        {
            return string.Empty;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake
{
    [Serializable]
   public class Options
    {

        public int windowWidth = 100; 
        public int windowHeight = 40; 
            public ConsoleKey playerOneUpKey = ConsoleKey.W;
            public ConsoleKey playerOneDownKey = ConsoleKey.S;
            public ConsoleKey playerOneRightKey = ConsoleKey.D;
            public ConsoleKey playerOneLeftKey = ConsoleKey.A;
            public ConsoleKey playerTwoUpKey = ConsoleKey.UpArrow;
            public ConsoleKey playerTwoDownKey = ConsoleKey.DownArrow;
            public ConsoleKey playerTwoRightKey = ConsoleKey.RightArrow;
            public ConsoleKey playerTwoLeftKey = ConsoleKey.LeftArrow;



    }
}

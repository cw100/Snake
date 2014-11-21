using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake
{
    class Object
    {
        public bool active;
        public int[,] position= new int[1,2];
         public void Initialize(Random randomizer, int gridwidth, int gridheight)
        {
            active = true;
            position[0,0]= randomizer.Next(gridwidth);
            position[0, 1] = randomizer.Next(gridheight);
        }
         public void Initialize(int x, int y)
         {
             active = true;
             position[0, 0] = x;
             position[0, 1] = y;
         }


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake
{
    class Pickup
    {
        public bool active;
        public int[,] position= new int[1,2];
         public void Initialize(Random randomizer, int gridwidth, int gridheight)
        {
            active = true;
            position[0,0]= randomizer.Next(gridwidth);
            position[0, 1] = randomizer.Next(gridheight);
        }


    }
}

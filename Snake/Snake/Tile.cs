using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake
{
    class Tile
    {
        public string gridIcon = " ";
        public bool containsSnake;

        public void Update()
        {
            if(containsSnake)
            {
                gridIcon = "0";

            }
            else
            {
                gridIcon = " ";
            }
        }

     

    }
}

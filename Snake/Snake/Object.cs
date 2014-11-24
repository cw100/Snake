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
        int x, y;
        public void Initialize(Random randomizer, int gridwidth, int gridheight, List<Object> walls)
        {
            active = true;

            do
            {
                x = randomizer.Next(gridwidth);
                y = randomizer.Next(gridheight);
            }
            while (CheckForWall(x, y, walls));

            position[0,0]= x;
            position[0, 1] = y;
        }
         public void Initialize(int x, int y)
         {
             active = true;
             position[0, 0] = x;
             position[0, 1] = y;
         }
        public bool CheckForWall(int x, int y, List<Object> walls)
         {
             try
             {
                 foreach (Object wall in walls)
                 {
                     if (wall.position[0, 0] == x && wall.position[0, 1] == y)
                     {
                         return true;
                     }
                 }
             }
             catch { }
             return false;
         }

    }
}

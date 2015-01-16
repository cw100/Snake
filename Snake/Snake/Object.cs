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

        //Sets default values for pickup objects
        public void Initialize(Random randomizer, int gridwidth, int gridheight, List<Object> walls)
        {
            active = true;
            
            //Checks to prevent being placed on a wall
            do
            {
                //Randomly chooses new position
                x = randomizer.Next(gridwidth);
                y = randomizer.Next(gridheight);
            }
            while (CheckForWall(x, y, walls));

            position[0,0]= x;
            position[0, 1] = y;
        }

        //Wall default values
         public void Initialize(int x, int y)
         {
             active = true;
             position[0, 0] = x;
             position[0, 1] = y;
         }


        public bool CheckForWall(int x, int y, List<Object> walls)
         {
             if (walls.Count != 0)
             {
                 foreach (Object wall in walls)
                 {
                     if (wall.position[0, 0] == x && wall.position[0, 1] == y)//If the position is in a wall, return true
                     {
                         return true;
                     }
                 }
             }
             return false;
         }

    }
}

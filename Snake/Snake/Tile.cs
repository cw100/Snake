using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake
{[Serializable]
    class Tile
    {
    public bool multiplayergamerunning;
        public List<int> multiplayerscores = new List<int>();
        public string gridIcon = " ";
        public int headNumber;
        public ConsoleColor colour = ConsoleColor.Gray;
        public string defaultGridIcon = " ";
        public bool containsHead;
        public bool containsBody;
        public bool containsPickup;

        public bool containsWall;
        
        public bool didContainWall;
        public bool didContainHead;
        public bool didContainBody;
        public bool didContainPickup;
        public bool hasChanged = true;
        public bool updated = false;
        public enum Direction
        {
            Up,
            Down,
            Left,
            Right
        }
        public Direction currentDirection= Direction.Right;
        public void TestChange()
        {

            if (didContainHead != containsHead || didContainPickup != containsPickup || didContainBody != containsBody || didContainWall != containsWall)
            {
                hasChanged = true;
                
            }
         
           
            
       }
        public void Update()
        {
            TestChange();

            if (containsHead)
            {
                colour = ConsoleColor.Red;
                if (currentDirection == Direction.Up)
                {
                    gridIcon = "^";
                }
                if (currentDirection == Direction.Down)
                {
                    gridIcon = "V";
                }
                if (currentDirection == Direction.Right)
                {
                    gridIcon = ">";
                }
                if (currentDirection == Direction.Left)
                {
                    gridIcon = "<";
                }
            }
            else if (containsBody)
            {

                colour = ConsoleColor.Red;
                gridIcon = "@";


            }
            else
            if (containsPickup)
            {

                colour = ConsoleColor.Green;
                gridIcon = "S";
            }
            else
            if (containsWall)
            {

                colour = ConsoleColor.DarkRed;
                gridIcon = "#";
            }
            else
            {

                colour = ConsoleColor.Green;
                gridIcon = defaultGridIcon;

            }
           

        }

    }
}

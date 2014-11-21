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

        public string defaultGridIcon = " ";
        public bool containsHead;
        public bool containsBody;
        public bool containsPickup;
        public enum Direction
        {
            Up,
            Down,
            Left,
            Right
        }
        public Direction currentDirection= Direction.Right;
        public void Update()
        {
            if(containsHead)
            {
                if(currentDirection == Direction.Up)
                    gridIcon = "^";
                if (currentDirection == Direction.Down)
                    gridIcon = "V";
                if (currentDirection == Direction.Right)
                    gridIcon = ">";
                if (currentDirection == Direction.Left)
                    gridIcon = "<";

            }
            else if(containsBody)
            {
                gridIcon = "@";
            }
            else
            {
                gridIcon = defaultGridIcon;
            }
            if(containsPickup)
            {
                gridIcon = "x";
            }
        }

     

    }
}

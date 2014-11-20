using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;

namespace Snake
{
    class Player
    {

        public int[,] headPosition = new int[1,2];
        public int snakeLength;
        public List<int[,]> bodyPositions;
        public bool active;
        public string snakeIcon;
        public enum Direction 
        {
            Up,
            Down,
            Left,
            Right,
        }
        public Direction currentDirection;



        public void Initialize(int x, int y, int snakelength, string snakeicon)
        {
            headPosition[0,0]= x;
            headPosition[0,1] = y;
            bodyPositions = new List<int[,]>();
            snakeLength = snakelength;
            snakeIcon = snakeicon;
            currentDirection = Direction.Right;
        }


        public void PlayerInput()
        {
            Direction lastDirection = currentDirection;
            
            var input = Console.ReadKey();
            switch (input.Key)
            {
                case ConsoleKey.W:
                    if (lastDirection == Direction.Right || lastDirection == Direction.Left)
                    {
                        currentDirection = Direction.Up;
                    }
                    break;
                case ConsoleKey.D:
                    if (lastDirection == Direction.Up || lastDirection == Direction.Down)
                    {
                        currentDirection = Direction.Right;
                    }
                    break;
                case ConsoleKey.A:
                    if (lastDirection == Direction.Up || lastDirection == Direction.Down)
                    {
                        currentDirection = Direction.Left;
                    }
                    break;
                case ConsoleKey.S:
                    if (lastDirection == Direction.Right || lastDirection == Direction.Left)
                    {
                        currentDirection = Direction.Down;
                    }
                    break;
            }
        }

        public void PlayerMove()
        {
            bodyPositions.Add(headPosition);
            if(currentDirection == Direction.Up)
            {
                headPosition[0,1] -= 1;
            }
            if (currentDirection == Direction.Down)
            {
                headPosition[0, 1] += 1;
            }
            if (currentDirection == Direction.Right)
            {
                headPosition[0, 0] += 1;
            }
            if (currentDirection == Direction.Left)
            {
                headPosition[0, 0] -= 1;
            }

        }
        public void CheckLength()
        {
            
                if(bodyPositions.Count>snakeLength)
                {
                    bodyPositions.RemoveAt(bodyPositions.Count-1);
                }
            
        }
        public void Update()
        {
            PlayerInput();
            PlayerMove();
            CheckLength();
        }


    }
}

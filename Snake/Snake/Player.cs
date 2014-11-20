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
        Thread inputThread;
        public int[,] headPosition = new int[1,2];
        public int snakeLength;
        public List<int[,]> bodyPositions;
        public bool active;
        public string snakeIcon;
        public int gridWidth, gridHeight;
        public enum Direction 
        {
            Up,
            Down,
            Left,
            Right,
        }
        public Direction currentDirection;
        public Direction lastDirection;


        public void Initialize(int x, int y, int snakelength, string snakeicon,int gridwidth,int gridheight)
        {
            gridHeight = gridheight;
            gridWidth = gridwidth;
            headPosition[0,0]= x;
            headPosition[0,1] = y;
            bodyPositions = new List<int[,]>();
            snakeLength = snakelength;
            snakeIcon = snakeicon;
            currentDirection = Direction.Right;
        }


        public void PlayerInput()
        {
             lastDirection = currentDirection;
            var input = Console.ReadKey(true);

            switch (input.Key)
            {
                case ConsoleKey.W:
                    if (lastDirection == Direction.Right || lastDirection == Direction.Left)
                    {
                        currentDirection = Direction.Up;
                        lastDirection = currentDirection;
                    }
                    break;
                case ConsoleKey.D:
                    if (lastDirection == Direction.Up || lastDirection == Direction.Down)
                    {
                        currentDirection = Direction.Right;
                        lastDirection = currentDirection;
                    }
                    break;
                case ConsoleKey.A:
                    if (lastDirection == Direction.Up || lastDirection == Direction.Down)
                    {
                        currentDirection = Direction.Left;
                        lastDirection = currentDirection;
                    }
                    break;
                case ConsoleKey.S:
                    if (lastDirection == Direction.Right || lastDirection == Direction.Left)
                    {
                        currentDirection = Direction.Down;
                        lastDirection = currentDirection;
                    }
                    break;
            }
        }

        public void PlayerMove()
        {
            Thread.Sleep(100);
           
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
            if (headPosition[0, 0] > gridWidth-1)
            {
                headPosition[0, 0] = 0;
            }
            if (headPosition[0, 0] <0)
            {
                headPosition[0, 0] = gridWidth - 1;
            }

            if (headPosition[0, 1] > gridHeight - 1)
            {
                headPosition[0, 1] = 0;
            }

            if (headPosition[0, 1] < 0)
            {
                headPosition[0, 1] = gridHeight - 1;
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
            inputThread = new Thread(new ThreadStart(PlayerInput));
            inputThread.Start();
            

            PlayerMove();
            CheckLength();
        }


    }
}

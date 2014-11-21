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
        Thread playerThread;
        public int[,] headPosition = new int[1,2];
        public int playerSpeed;
        public int[,] bodyPosition;
        public int snakeLength;
        public List<int[,]> bodyPositions= new List<int[,]>();
        public bool active = true;
        public string snakeIcon;
        public int gridWidth, gridHeight;
        public enum Direction 
        {
            Up,
            Down,
            Left,
            Right
        }
        public Direction currentDirection;
        public Direction lastDirection;

        public void Initialize(int x, int y, int snakelength, string snakeicon,int gridwidth,int gridheight, int playerspeed)
        {
            playerSpeed = playerspeed;
            gridHeight = gridheight;
            gridWidth = gridwidth;
            headPosition[0,0]= x;
            headPosition[0,1] = y;
            
            snakeLength = snakelength;
            snakeIcon = snakeicon;
            currentDirection = Direction.Right;
            playerThread = new Thread(new ThreadStart(PlayerInput));

            playerThread.Start();
            
        }


        public void PlayerInput()
        {
            while(active)
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
        }

        public void PlayerMove()
        {
            bodyPosition = new int[1, 2];
            bodyPosition[0, 1] = headPosition[0, 1];

            bodyPosition[0, 0] = headPosition[0, 0];

            bodyPositions.Add(bodyPosition);
           
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

            Thread.Sleep(playerSpeed);
           

        }
        public void CheckLength()
        {
            
                if(bodyPositions.Count>snakeLength)
                {
                    bodyPositions.RemoveAt(0);
                }
            
        }
        
        public void Update()
        {

            PlayerMove();
            CheckLength();

        }
        


    }
}

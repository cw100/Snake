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
        public string username;
        public bool multiplayer = false;
        Thread playerInputThread;
        public int[,] headPosition = new int[1, 2];
        public int playerSpeed;
        public int[,] bodyPosition;
        public int snakeLength;
        public List<int[,]> bodyPositions = new List<int[,]>();
        public bool active = true;
        public string snakeIcon;
        public int gridWidth, gridHeight;
        public bool canMove = true;
        public int playerNumber;
        public ConsoleKeyInfo input;
        public int score;
        public enum Direction
        {
            Up,
            Down,
            Left,
            Right
        }
        public Direction currentDirection;
        public Direction lastDirection;
        public ConsoleKey upKey = ConsoleKey.W;
        public ConsoleKey downKey = ConsoleKey.S;
        public ConsoleKey rightKey = ConsoleKey.D;
        public ConsoleKey leftKey = ConsoleKey.A;
        public void Initialize(int x, int y, int snakelength, int gridwidth, int gridheight, int playerspeed, int playernum)
        {
            playerNumber = playernum;
            playerSpeed = playerspeed;
            gridHeight = gridheight;
            gridWidth = gridwidth;
            headPosition[0, 0] = x;
            headPosition[0, 1] = y;

            snakeLength = snakelength;
            currentDirection = Direction.Right;
            lastDirection = Direction.Right;
            if (playerNumber == 1)
            {
                upKey = ConsoleKey.W;
                downKey = ConsoleKey.S;
                rightKey = ConsoleKey.D;
                leftKey = ConsoleKey.A;
            }
            if (playerNumber == 2)
            {
                upKey = ConsoleKey.UpArrow;
                downKey = ConsoleKey.DownArrow;
                rightKey = ConsoleKey.RightArrow;
                leftKey = ConsoleKey.LeftArrow;
            }
            playerInputThread = new Thread(new ThreadStart(PlayerInput));

            playerInputThread.Start();


        }


        public void PlayerInput()
        {
            while (true)
            {
                
                    new System.Threading.ManualResetEvent(false).WaitOne(10);
                
                    if (Program.input.Key == upKey)
                    {

                        if (lastDirection == Direction.Right || lastDirection == Direction.Left)
                        {
                            currentDirection = Direction.Up;

                        }
                    }
                    else
                        if (Program.input.Key == rightKey)
                        {
                            if (lastDirection == Direction.Up || lastDirection == Direction.Down)
                            {
                                currentDirection = Direction.Right;

                            }
                        }
                        else
                            if (Program.input.Key == downKey)
                            {
                                if (lastDirection == Direction.Right || lastDirection == Direction.Left)
                                {
                                    currentDirection = Direction.Down;
                                }

                            }
                            else
                                if (Program.input.Key == leftKey)
                                {
                                    if (lastDirection == Direction.Up || lastDirection == Direction.Down)
                                    {
                                        currentDirection = Direction.Left;
                                    }

                                }
                
            }


        }







        public void PlayerMove()
        {

            bodyPosition = new int[1, 2];
            bodyPosition[0, 1] = headPosition[0, 1];

            bodyPosition[0, 0] = headPosition[0, 0];

            bodyPositions.Add(bodyPosition);

            if (currentDirection == Direction.Up)
            {
                headPosition[0, 1] -= 1;
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
            lastDirection = currentDirection;

            if (headPosition[0, 0] > gridWidth - 1)
            {
                headPosition[0, 0] = 0;
            }
            if (headPosition[0, 0] < 0)
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
            if (playerSpeed != 0)
            {
                new System.Threading.ManualResetEvent(false).WaitOne(playerSpeed);
            }

        }
        public bool CheckLength()
        {

            if (bodyPositions.Count > snakeLength)
            {

                return true;
            }

            return false;

        }

        public void Update()
        {

            PlayerMove();




        }
        public void GameEnd()
        {

            playerInputThread.Abort();
        }



    }
}

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
        Thread playerMoveThread;
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
        public enum Direction
        {
            Up,
            Down,
            Left,
            Right
        }
        public Direction currentDirection;
        public Direction lastDirection;

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

            playerInputThread = new Thread(new ThreadStart(PlayerInput));

            playerInputThread.Start();


        }


        public void PlayerInput()
        {
            while (true)
            {
                if (playerNumber == 1)
                {
                    switch (Program.input.Key)
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
                        case ConsoleKey.S:
                            if (lastDirection == Direction.Right || lastDirection == Direction.Left)
                            {
                                currentDirection = Direction.Down;
                            }

                            break;
                        case ConsoleKey.A:
                            if (lastDirection == Direction.Up || lastDirection == Direction.Down)
                            {
                                currentDirection = Direction.Left;
                            }
                            break;
                    }
                }
                if (playerNumber == 2)
                {
                    switch (Program.input.Key)
                    {
                        case ConsoleKey.UpArrow:
                            if (lastDirection == Direction.Right || lastDirection == Direction.Left)
                            {
                                currentDirection = Direction.Up;

                            }
                            break;


                        case ConsoleKey.RightArrow:
                            if (lastDirection == Direction.Up || lastDirection == Direction.Down)
                            {
                                currentDirection = Direction.Right;

                            }
                            break;

                        case ConsoleKey.DownArrow:
                            if (lastDirection == Direction.Right || lastDirection == Direction.Left)
                            {
                                currentDirection = Direction.Down;
                            }

                            break;

                        case ConsoleKey.LeftArrow:
                            if (lastDirection == Direction.Up || lastDirection == Direction.Down)
                            {
                                currentDirection = Direction.Left;
                            }
                            break;
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
            lastDirection = currentDirection;

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
            if (active)
            {
                PlayerMove();
            }
            else
            {
                playerInputThread.Abort();
            }

        }
        public void GameEnd()
        {


        }



    }
}

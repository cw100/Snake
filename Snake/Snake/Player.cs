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

        //Sets default values
        public void Initialize(int x, int y, int snakelength, int gridwidth, int gridheight, int playerspeed, int playernum)
        {
            playerNumber = playernum;
            playerSpeed = playerspeed;
            gridHeight = gridheight;
            gridWidth = gridwidth;
            //Spawn location
            headPosition[0, 0] = x;
            headPosition[0, 1] = y;

            snakeLength = snakelength;
            currentDirection = Direction.Right;
            lastDirection = Direction.Right;
            //Sets the control methods
            if (playerNumber == 1 || multiplayer == true)
            {

                upKey = Program.currentOptions.playerOneUpKey;
                downKey = Program.currentOptions.playerOneDownKey;
                rightKey = Program.currentOptions.playerOneRightKey;
                leftKey = Program.currentOptions.playerOneLeftKey;
            }
            else
            {
                if (playerNumber == 2)
                {
                    upKey = Program.currentOptions.playerTwoUpKey;
                    downKey = Program.currentOptions.playerTwoDownKey;
                    rightKey = Program.currentOptions.playerTwoRightKey;
                    leftKey = Program.currentOptions.playerTwoLeftKey;
                }
            }
            playerInputThread = new Thread(new ThreadStart(PlayerInput));

            playerInputThread.Start();//Starts the player input thread


        }


        public void PlayerInput()
        {
            while (true)//Constant loop
            {
                
                    new System.Threading.ManualResetEvent(false).WaitOne(10);//Pause thread to prevent massive cpu usage

                //Takes input from main input thread if local game, uses if else statements instead of switch case to allow for variable key input
                    if (!multiplayer)
                    {
                        if (Program.input.Key == upKey)
                        {

                            if (lastDirection == Direction.Right || lastDirection == Direction.Left)//Prevents player going back on themselves
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

                //Multiplayer input taken from client
                    if (multiplayer)
                    {
                        if (input.Key == upKey)
                        {

                            if (lastDirection == Direction.Right || lastDirection == Direction.Left)
                            {
                                currentDirection = Direction.Up;

                            }
                        }
                        else
                            if (input.Key== rightKey)
                            {
                                if (lastDirection == Direction.Up || lastDirection == Direction.Down)
                                {
                                    currentDirection = Direction.Right;

                                }
                            }
                            else
                                if (input.Key== downKey)
                                {
                                    if (lastDirection == Direction.Right || lastDirection == Direction.Left)
                                    {
                                        currentDirection = Direction.Down;
                                    }

                                }
                                else
                                    if (input.Key== leftKey)
                                    {
                                        if (lastDirection == Direction.Up || lastDirection == Direction.Down)
                                        {
                                            currentDirection = Direction.Left;
                                        }

                                    }
                    }
            }


        }






        //Moves the player
        public void PlayerMove()
        {

            bodyPosition = new int[1, 2];//New body location

            //Body set to current head
            bodyPosition[0, 1] = headPosition[0, 1];
            bodyPosition[0, 0] = headPosition[0, 0];

            bodyPositions.Add(bodyPosition);//Body added to list


            //Moves head location depending on direction
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
            lastDirection = currentDirection;//For preventing player moving back on self

            //If the player hits the sides of screen they loop
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
            

        }

        //Check for length being higher than max
        public bool CheckLength()
        {

            if (bodyPositions.Count > snakeLength)
            {

                return true;//Returns true if too high
            }

            return false;

        }

        public void Update()
        {

            PlayerMove();


        }
        public void GameEnd()
        {

            playerInputThread.Abort();//Ends input thread
        }



    }
}

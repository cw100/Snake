using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Snake
{
    class Program
    {

        static Tile[,] Grid;
        static Player playerOne;

        static bool gameRunning = true;
        static bool running = true;
        static Random randomizer = new Random();
        static List<Pickup> pickupList = new List<Pickup>();
        static Pickup pickup;
        static int maxPickups;
        static int difficulty;
        static int gridHeight ;
        static int gridWidth;
        static int currentPickupNo;
        static int pickupNoAdded;
        static int lengthAdded;
        static int speedAdded;
        static int startSpeed;
        static int startLength;

        static int maxSpeed;
        static void AddPickup(int gridwidth, int gridheight, int maxpickups)
        {
            if (pickupList.Count < currentPickupNo)
            {
                pickup = new Pickup();

                pickup.Initialize(randomizer, gridwidth, gridheight);

                pickupList.Add(pickup);
            }
        }
        static void CreateGrid(int gridwidth, int gridheight)
        {
            Grid = new Tile[gridheight, gridwidth];
            for (int i = 0; i < gridheight; i++)
            {
                for (int j = 0; j < gridwidth; j++)
                {
                    Grid[i, j] = new Tile();
                }
                Console.WriteLine();

            }
        }
        static void DrawGrid()
        {
            Console.Clear();

            for (int i = 0; i < Grid.GetLength(0); i++)
            {
                for (int j = 0; j < Grid.GetLength(1); j++)
                {
                    Console.Write(Grid[i,j].gridIcon);
                }
                Console.WriteLine();
            }
            Console.SetCursorPosition(0, Console.WindowHeight * 2 / 3);
            for (int i = 0; i < Grid.GetLength(1); i++)
            {
                Console.Write("-");
            }
            Console.SetCursorPosition(13, Console.WindowHeight * 2 / 3 +1);
           
            Console.Write("     _______..__   __.      ___       __  ___  _______");

            Console.SetCursorPosition(13, Console.WindowHeight * 2 / 3 + 2);
            Console.Write("    /       ||  \\ |  |     /   \\     |  |/  / |   ____|");

            Console.SetCursorPosition(13, Console.WindowHeight * 2 / 3 + 3);
            Console.Write("   |   (----`|   \\|  |    /  ^  \\    |  '  /  |  |__");

            Console.SetCursorPosition(13, Console.WindowHeight * 2 / 3 + 4);
            Console.Write("    \\   \\    |  . `  |   /  /_\\  \\   |    <   |   __|");

            Console.SetCursorPosition(13, Console.WindowHeight * 2 / 3 + 5);
            Console.Write(".----)   |   |  |\\   |  /  _____  \\  |  .  \\  |  |____");

            Console.SetCursorPosition(13, Console.WindowHeight * 2 / 3 + 6);
            Console.Write("|_______/    |__| \\__| /__/     \\__\\ |__|\\__\\ |_______|");
                                                                   
            

        }
        static void DrawScore()
        {

            Console.SetCursorPosition(10, Console.WindowHeight * 2 / 3 + 10);
            Console.WriteLine("Score: " + ((playerOne.snakeLength - startLength) * ((difficulty / 3) + 1)));
          
        }
        static void UpdateGrid()
        {            
            for (int i = 0; i < Grid.GetLength(0); i++)
            {
                for (int j = 0; j < Grid.GetLength(1); j++)
                {
                    if (Grid[i, j].hasChanged == true)
                    {
                        Console.SetCursorPosition(j, i);

                        Console.Write("\b" + Grid[i, j].gridIcon);

                        Grid[i, j].containsHead = false;
                    }
                }
            }
        }  
        static void DirectionConvert()
        {
            if(playerOne.currentDirection == Player.Direction.Up)
            {

                Grid[playerOne.headPosition[0, 1], playerOne.headPosition[0, 0]].currentDirection = Tile.Direction.Up;
            }
            if(playerOne.currentDirection == Player.Direction.Down)
            {

                Grid[playerOne.headPosition[0, 1], playerOne.headPosition[0, 0]].currentDirection = Tile.Direction.Down;
            }
            if(playerOne.currentDirection == Player.Direction.Left)
            {

                Grid[playerOne.headPosition[0, 1], playerOne.headPosition[0, 0]].currentDirection = Tile.Direction.Left;
            }
            if(playerOne.currentDirection == Player.Direction.Right)
            {

                Grid[playerOne.headPosition[0, 1], playerOne.headPosition[0, 0]].currentDirection = Tile.Direction.Right;
            }
        }
        static bool Collision(int[,] one, int[,] two)
        {
            if (one[0, 0] == two[0, 0] && one[0, 1] == two[0, 1])
            {
                return true;
            }
            return false;
        }
        static void Initialize()
        {
            Console.CursorVisible = false;
            gridHeight = Console.WindowHeight*2/3 ;
             gridWidth = Console.WindowWidth ;
             gameRunning = true;
            CreateGrid(gridWidth, gridHeight);
            startSpeed = 81 - (5*difficulty);
            maxPickups = 10;
            lengthAdded = (2 * difficulty);
            startLength = (3 * difficulty);
            pickupNoAdded = 1;
            speedAdded = (5 * difficulty);
            currentPickupNo = 1;
            maxSpeed = 50 - (5 * difficulty);


            playerOne = new Player();
            playerOne.Initialize(10, 10, startLength, gridWidth, gridHeight, startSpeed);

            DrawGrid();
        }
        static void GridLogic()
    {

        Grid[playerOne.headPosition[0, 1], playerOne.headPosition[0, 0]].containsHead = true;
        for (int i = 0; i < pickupList.Count; i++)
        {

            Grid[pickupList[i].position[0, 1], pickupList[i].position[0, 0]].containsPickup = true;

            if (Collision(pickupList[i].position, playerOne.headPosition))
            {

                Grid[pickupList[i].position[0, 1], pickupList[i].position[0, 0]].containsPickup = false;

                pickupList.RemoveAt(i);
                playerOne.snakeLength+= lengthAdded;
                if (currentPickupNo < maxPickups)
                {
                    currentPickupNo +=pickupNoAdded;
                }
                if (playerOne.playerSpeed > maxSpeed)
                {
                    playerOne.playerSpeed -= speedAdded;
                }
                else
                {
                    playerOne.playerSpeed =maxSpeed;
                }
            }
        }
        foreach (Tile tile in Grid)
        {
            tile.containsBody = false;
        }
        foreach (int[,] body in playerOne.bodyPositions)
        {
            Grid[body[0, 1], body[0, 0]].containsBody = true;
            if (Collision(body, playerOne.headPosition))
            {
                gameRunning = false;
            }
        }
        foreach (Tile tile in Grid)
        {
            tile.Update();
        }
    }
        static void GameLoop()
        {

            Initialize();
            while (gameRunning)
            {
                foreach (Tile tile in Grid)
                {
                    tile.didContainHead = tile.containsHead;
                    tile.didContainBody = tile.containsBody;
                    tile.didContainPickup = tile.containsPickup;
                }

                playerOne.Update();

                AddPickup(gridWidth, gridHeight, maxPickups);

                DirectionConvert();
                GridLogic();
                DrawScore();
                UpdateGrid();
            }

            Console.Clear();
            Console.SetCursorPosition((gridWidth-9)/2, gridHeight / 2);

            Console.Write("Game Over");

            Console.SetCursorPosition((gridWidth - 9) / 2, 1 + (gridHeight / 2));
            Console.Write("Score: " + ((playerOne.snakeLength - startLength) * ((difficulty / 3) + 1)));
            playerOne.GameEnd();

        }
        static bool DifficultySelect()
        {
            bool valid = true;
            do
            {
                Console.Clear();
                Console.WriteLine("1:Easy\n2:Medium\n3:Hard\n4:Back");
                switch (Console.ReadLine())
                {
                    case "1":

                        difficulty = 1;
                        return true;
                    case "2":

                        difficulty = 2;
                        return true;
                    case "3":
                        difficulty = 3;
                        return true;
                    case "4":
                        return false;
                    default:
                        valid = false;
                        break;
                }
            }
            while (!valid);
            return true;
        }
        static void Main(string[] args)
        {
            while (running)
            {

                Console.WindowHeight = 50;
                Console.Clear();
                Console.WriteLine("1:Single Player\n2:Multiplayer\n3:Exit");
                switch (Console.ReadLine())
                {
                    case"1":
                        if (DifficultySelect())
                        {
                            GameLoop();
                        }
                        break;
                    case"2":
                        break;
                    case"3":
                        running = false;
                        break;
                }

            }

        }
    }
}

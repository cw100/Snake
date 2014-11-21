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
        static bool running = true;
        static Random randomizer = new Random();
        static List<Pickup> pickupList = new List<Pickup>();
        static Pickup pickup;
        static int maxPickups;

        static int gridHeight = Console.WindowHeight ;
        static int gridWidth = Console.WindowWidth ;
        static int currentPickupNo;
        static int pickupNoAdded;
        static int lengthAdded;
        static int speedAdded;
        static int startSpeed;
        static int startLength;

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

            CreateGrid(gridWidth, gridHeight);
            startSpeed = 50;
            maxPickups = 10;
            lengthAdded = 1;
            startLength = 1;
            pickupNoAdded = 1;
            speedAdded = 5;
            currentPickupNo = 100;


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
                if (playerOne.playerSpeed > 1)
                {
                    playerOne.playerSpeed -= speedAdded;
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
                running = false;
            }
        }
        foreach (Tile tile in Grid)
        {
            tile.Update();
        }
    }
        static void GameLoop()
        {
            while (running)
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
                UpdateGrid();
            }
        }
        
        static void Main(string[] args)
        {
            Initialize();
            GameLoop();
            Console.WriteLine("Game Over");

        }
    }
}

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
        static void AddPickup(int gridwidth, int gridheight, int maxpickups)
        {
            if (pickupList.Count < maxpickups)
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
        
        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            
            playerOne = new Player();
            int gridHeight = Console.WindowHeight-1;
            int gridWidth = Console.WindowWidth-1;
            playerOne.Initialize(10, 10, 5, "O", gridWidth, gridHeight, 50);

            CreateGrid(gridWidth, gridHeight);
            maxPickups = 1;
            DrawGrid();
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
               
                Grid[playerOne.headPosition[0, 1], playerOne.headPosition[0, 0]].containsHead = true;

                for(int i =0; i < pickupList.Count; i++)
                {
                    Grid[pickupList[i].position[0, 1], pickupList[i].position[0, 0]].containsPickup = true;
                    
                    if(Collision(pickupList[i].position, playerOne.headPosition))
                    {

                        Grid[pickupList[i].position[0, 1], pickupList[i].position[0, 0]].containsPickup = false;
                       
                        pickupList.RemoveAt(i);
                        playerOne.snakeLength++;
                        maxPickups++;
                        if(playerOne.playerSpeed>1)
                        {
                            playerOne.playerSpeed--;
                        }
                    }
                }
                foreach (Tile tile in Grid)
                {
                    tile.containsBody = false;
                }
               
                foreach(int[,] body in playerOne.bodyPositions)
                {

                    Grid[body[0, 1], body[0, 0]].containsBody = true;

                    if (Collision(body, playerOne.headPosition))
                    {
                        running = false;
                    }
                }
                DirectionConvert();
                foreach(Tile tile in Grid)
                {
                    tile.Update();

                }
                
                UpdateGrid();
            }
            Console.WriteLine("Game Over");

        }
    }
}

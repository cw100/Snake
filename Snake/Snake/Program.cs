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
                    Grid[i, j].containsHead = false;

                    Grid[i, j].containsBody = false;
                }
                Console.WriteLine();
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
        
        static void Main(string[] args)
        {
            Console.CursorVisible = false;

            playerOne = new Player();
            int gridHeight = Console.WindowHeight-1;
            int gridWidth = Console.WindowWidth-1;
            playerOne.Initialize(10, 10, 5, "O", gridWidth, gridHeight);

            CreateGrid(gridWidth, gridHeight);
           
            while (running)

            {
                playerOne.Update();
                Grid[playerOne.headPosition[0, 1], playerOne.headPosition[0, 0]].containsHead = true;
                foreach(int[,] body in playerOne.bodyPositions)
                {

                    Grid[body[0, 1], body[0, 0]].containsBody = true;
                }
                DirectionConvert();
                foreach(Tile tile in Grid)
                {
                    tile.Update();
                }
                DrawGrid();
                Thread.Sleep(100);
            }

        }
    }
}

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
                    Grid[i, j].containsSnake = false;
                }
                Console.WriteLine();
                
            }
        }

        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            
            playerOne = new Player();
            int gridHeight = 10;
            int gridWidth = 10;
            playerOne.Initialize(10, 10, 1, "O", gridWidth, gridHeight);
           
            CreateGrid(gridHeight, gridWidth);
            
            while (running)
            {
               
                playerOne.Update();
               
                
                Grid[playerOne.headPosition[0, 1], playerOne.headPosition[0, 0]].containsSnake = true;
                foreach(Tile tile in Grid)
                {
                    tile.Update();
                }
                DrawGrid();
            }

        }
    }
}

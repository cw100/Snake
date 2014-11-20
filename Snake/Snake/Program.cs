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
        static void CreateGrid(int gridWidth, int gridHeight)
        {
            Grid = new Tile[gridHeight, gridWidth];
            for (int i = 0; i < gridHeight; i++)
            {
                for (int j = 0; j < gridWidth; j++)
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
                }
                
            }
        }

        static void Main(string[] args)
        {
            playerOne = new Player();
            playerOne.Initialize(10, 10, 1, "O");
            CreateGrid(Console.WindowWidth, Console.WindowHeight-1);
            DrawGrid();
            while (running)
            {
               
                playerOne.Update();
                Grid[playerOne.headPosition[0, 1], playerOne.headPosition[0, 0]].gridIcon = playerOne.snakeIcon;
                
                DrawGrid();
            }

        }
    }
}

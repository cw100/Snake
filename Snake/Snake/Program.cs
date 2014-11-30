using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
namespace Snake
{
    class Program
    {
        static Server server = new Server();
        static Thread serverThread;
       public  static Tile[,] Grid;
       public static Player playerOne;
        static List<Object> wallList;
        static Object wall;
        static bool gameRunning = true;
        static bool running = true;
        static Random randomizer = new Random();
        static List<Object> pickupList;
        static Object pickup;
        static int maxPickups;
        static int difficulty;
        static int gridHeight;
        static int gridWidth;
        static int currentPickupNo;
        static int pickupNoAdded;
        static int lengthAdded;
        static int speedAdded;
        static int startSpeed;
        static int startLength;

        static int maxSpeed;

        static System.Net.Sockets.TcpClient clientSocket = new System.Net.Sockets.TcpClient();
       
        static void AddPickup(int gridwidth, int gridheight, int maxpickups)
        {
            if (pickupList.Count < currentPickupNo)
            {
                pickup = new Object();

                pickup.Initialize(randomizer, gridwidth, gridheight,wallList);

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

            for (int i = 0; i < gridHeight; i++)
            {
                for (int j = 0; j < gridWidth; j++)
                {

                    Console.ForegroundColor = Grid[i, j].colour;
                    Console.Write(Grid[i, j].gridIcon);
                }
            }
            Console.SetCursorPosition(0, Console.WindowHeight * 2 / 3);
            for (int i = 0; i < Grid.GetLength(1); i++)
            {
                Console.Write("-");
            }

            DrawTitle(13, Console.WindowHeight * 2 / 3);
        }
        static void LevelOne()
        {
            wallList = new List<Object>();
            for (int i = 0; i < gridWidth; i++)
            {
                
                    wall = new Object();
                    wall.Initialize(i, 0);
                    wallList.Add(wall);
                
            }
            for (int i = 0; i < gridWidth; i++)
            {

                wall = new Object();
                wall.Initialize(i, gridHeight-1);
                wallList.Add(wall);

            }
            for (int i = 0; i < gridHeight; i++)
            {

                wall = new Object();
                wall.Initialize(1, i);
                wallList.Add(wall);

            }
            for (int i = 0; i < gridHeight; i++)
            {

                wall = new Object();
                wall.Initialize(gridWidth-1 , i);
                wallList.Add(wall);

            }
        }

        static void DrawTitle(int x, int y)
        {

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.SetCursorPosition(x, y + 1);

            Console.Write("     _______..__   __.      ___       __  ___  _______");

            Console.SetCursorPosition(x, y + 2);
            Console.Write("    /       ||  \\ |  |     /   \\     |  |/  / |   ____|");

            Console.SetCursorPosition(x, y + 3);
            Console.Write("   |   (----`|   \\|  |    /  ^  \\    |  '  /  |  |__");

            Console.SetCursorPosition(x, y + 4);
            Console.Write("    \\   \\    |  . `  |   /  /_\\  \\   |    <   |   __|");

            Console.SetCursorPosition(x, y + 5);
            Console.Write(".----)   |   |  |\\   |  /  _____  \\  |  .  \\  |  |____");

            Console.SetCursorPosition(x, y + 6);
            Console.Write("|_______/    |__| \\__| /__/     \\__\\ |__|\\__\\ |_______|");
        }
        static void DrawScore()
        {
            Console.ForegroundColor= ConsoleColor.Gray;
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
                        Console.ForegroundColor = Grid[i, j].colour;
                        Console.Write("\b" + Grid[i, j].gridIcon);

                        Grid[i, j].containsHead = false;
                    }
                }
            }
        }
        static void DirectionConvert()
        {
            if (playerOne.currentDirection == Player.Direction.Up)
            {

                Grid[playerOne.headPosition[0, 1], playerOne.headPosition[0, 0]].currentDirection = Tile.Direction.Up;
            }
            if (playerOne.currentDirection == Player.Direction.Down)
            {

                Grid[playerOne.headPosition[0, 1], playerOne.headPosition[0, 0]].currentDirection = Tile.Direction.Down;
            }
            if (playerOne.currentDirection == Player.Direction.Left)
            {

                Grid[playerOne.headPosition[0, 1], playerOne.headPosition[0, 0]].currentDirection = Tile.Direction.Left;
            }
            if (playerOne.currentDirection == Player.Direction.Right)
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
            pickupList = new List<Object>();
            gridHeight = Console.WindowHeight * 2 / 3;
            gridWidth = Console.WindowWidth;
            gameRunning = true;
            CreateGrid(gridWidth, gridHeight);
            startSpeed = 81 - (5 * difficulty);
            maxPickups = 10;
            lengthAdded = (2 * difficulty);
            startLength = (3 * difficulty);
            pickupNoAdded = 1;
            speedAdded = (5 * difficulty);
            currentPickupNo = 1;
            maxSpeed = 70 - (5 * difficulty);
            //if (WallToggle())
            //{
            //    LevelOne();
            //}
            //else
            //{
            //    wallList = new List<Object>();
            //}
            playerOne = new Player();
            playerOne.Initialize(15, 15, startLength, gridWidth, gridHeight, startSpeed);
           
            
            DrawGrid();
        }
        static void GridLogic()
        {

            Grid[playerOne.headPosition[0, 1], playerOne.headPosition[0, 0]].containsHead = true;
            try
            {
                for (int i = 0; i < wallList.Count; i++)
                {

                    Grid[wallList[i].position[0, 1], wallList[i].position[0, 0]].containsWall = true;

                    if (Collision(wallList[i].position, playerOne.headPosition))
                    {
                        gameRunning = false;
                    }
                }
            }
            catch
            {

            }
            for (int i = 0; i < pickupList.Count; i++)
            {

                Grid[pickupList[i].position[0, 1], pickupList[i].position[0, 0]].containsPickup = true;

                if (Collision(pickupList[i].position, playerOne.headPosition))
                {

                    Grid[pickupList[i].position[0, 1], pickupList[i].position[0, 0]].containsPickup = false;

                    pickupList.RemoveAt(i);
                    playerOne.snakeLength += lengthAdded;
                    if (currentPickupNo < maxPickups)
                    {
                        currentPickupNo += pickupNoAdded;
                    }
                    if (playerOne.playerSpeed > maxSpeed)
                    {
                        playerOne.playerSpeed -= speedAdded;
                        if (playerOne.playerSpeed > maxSpeed)
                        {
                            playerOne.playerSpeed = maxSpeed;
                        }
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
        static void SinglePlayerGameLoop()
        {
            while (gameRunning)
            {
                foreach (Tile tile in Grid)
                {

                    tile.didContainWall = tile.containsWall;
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

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Clear();
            Console.SetCursorPosition((gridWidth - 9) / 2, gridHeight / 2);

            Console.Write("Game Over");

            Console.SetCursorPosition((gridWidth - 9) / 2, 1 + (gridHeight / 2));
            Console.Write("Score: " + ((playerOne.snakeLength - startLength) * ((difficulty / 3) + 1)));
            playerOne.GameEnd();
            Console.ReadKey();

        }
        static void MultiPlayerGameLoop()
        {
            while (gameRunning)
            {
                foreach (Tile tile in Grid)
                {

                    tile.didContainWall = tile.containsWall;
                    tile.didContainHead = tile.containsHead;
                    tile.didContainBody = tile.containsBody;
                    tile.didContainPickup = tile.containsPickup;
                }

                DrawScore();
                UpdateGrid();
            }

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Clear();
            Console.SetCursorPosition((gridWidth - 9) / 2, gridHeight / 2);

            Console.Write("Game Over");

            Console.SetCursorPosition((gridWidth - 9) / 2, 1 + (gridHeight / 2));
            Console.Write("Score: " + ((playerOne.snakeLength - startLength) * ((difficulty / 3) + 1)));
            playerOne.GameEnd();
            Console.ReadKey();

        }
        static bool DifficultySelect()
        {
            bool valid = true;
            do
            {
                Console.Clear();
                valid = true;
                DrawTitle(13, 2);
                Console.SetCursorPosition(0, 12);
                Console.WriteLine("\t1:Easy\n\t2:Medium\n\t3:Hard\n\t4:Impossible\n\t5:Back");
                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.D1:

                        difficulty = 1;
                        return true;
                    case ConsoleKey.D2:

                        difficulty = 2;
                        return true;
                    case ConsoleKey.D3:
                        difficulty = 3;
                        return true;
                    case ConsoleKey.D4:
                        difficulty = 7;
                        return true;
                    case ConsoleKey.D5:
                        return false;
                    default:
                        valid = false;
                        break;
                }
            }
            while (!valid);
            return true;
        }
        static bool WallToggle()
        {
            bool valid = true;
            do
            {

                Console.Clear();
                Console.WriteLine("Walls?");
                Console.WriteLine("\t1:Yes\n\t2:No");
                valid = true;
                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.D1:
                        return true;
                    case ConsoleKey.D2:
                        return false;
                    default:
                        valid = false;
                        break;
                }
            }
            while (!valid);
             return true;
        }
        public static byte[] SerializeToBytes<T>(T item)
        {
            var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, item);
                stream.Seek(0, SeekOrigin.Begin);
                return stream.ToArray();
            }
        }
        public static object DeserializeFromBytes(byte[] bytes)
        {
            var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream(bytes))
            {
                return formatter.Deserialize(stream);
            }
        }
        static void ServerStart()
        {

            TcpListener serverSocket = new TcpListener(8888);
            TcpClient clientSocket = default(TcpClient);
            int counter = 0;
            serverSocket.Start();
            Console.WriteLine("Server Started");

            counter = 0;
            while ((true))
            {
                counter += 1;
                clientSocket = serverSocket.AcceptTcpClient();
                Server client = new Server();
                client.startClient(clientSocket, Convert.ToString(counter));
               
            }
        }
       

        
     
        static void ClientStart()
        {
                clientSocket.Connect("192.168.1.29", 8888);

                Console.WriteLine("Connected ...");
                DrawGrid();
                while (gameRunning)
                {
                    
                    NetworkStream serverStream = clientSocket.GetStream();
                    byte[] outStream = SerializeToBytes<Player.Direction>(playerOne.currentDirection);

                    serverStream.Write(outStream, 0, outStream.Length);
                    serverStream.Flush();

                    byte[] inStream = new byte[1000000];
                    serverStream.Read(inStream, 0, 1000000);
                    Grid = (Tile[,])DeserializeFromBytes(inStream);
                    
                        
                        DrawScore();
                        UpdateGrid();
                    
                }
            
            
          
        }
        static void MultiplayerSelect()
        {
            Console.Clear();
            DrawTitle(13, 2);
            Console.SetCursorPosition(0, 12);
            Console.WriteLine("\t1:Server\n\t2:Client\n\t3:Back");
            switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.D1:
                       
                       ServerStart();
                       SinglePlayerGameLoop();
                       
            
                        break;
                    case ConsoleKey.D2:

                        ClientStart();
                        break;
                    case ConsoleKey.D3:
                        break;
                }
        }
        static void Main(string[] args)
        {
            while (running)
            {
                Console.CursorVisible = false;
                Console.WindowHeight = 50;
                Console.Clear();
                DrawTitle(13, 2);
                Console.SetCursorPosition(0, 12);
                Console.WriteLine("\t1:Single Player\n\t2:Multiplayer\n\t3:Exit");
                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.D1:

                        if (DifficultySelect())
                        {


                            Initialize();
                            SinglePlayerGameLoop();
                        }
                        break;
                    case ConsoleKey.D2:

                        difficulty = 3;


                        Initialize();
                        MultiplayerSelect();
                        
                        break;
                    case ConsoleKey.D3:
                        running = false;
                        break;
                }

            }

        }
    }
}

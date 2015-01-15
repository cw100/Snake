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
using System.Xml.Serialization;
namespace Snake
{
    class Program
    {

        //Varible declaration 
        public static List<Thread> playerThreads = new List<Thread>();
        static int playersDead = 0;
        public static List<Player> players = new List<Player>();
        static Server server = new Server();
        static Thread serverThread;
        static Thread InputThread;
        public static Tile[,] Grid;
        public static int numOfPlayers = 1;
        public static Tile[,] previousGrid;
        public static List<ScoreObject> easyHighScores;
        public static List<ScoreObject> normalHighScores;
        public static List<ScoreObject> hardHighScores;
        public static List<ScoreObject> impossibleHighScores;
        public static List<Object> wallList;
        public static Object wall;
        static bool gameRunning = true;
        static bool running = true;
        static Random randomizer = new Random();
        public static List<Object> pickupList;
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
        public static ConsoleKeyInfo input;



        //Generate new pickup objects
        static void AddPickup(int gridwidth, int gridheight, int maxpickups) 
        {
            if (pickupList.Count < currentPickupNo) //Only adds more if less than current max
            {
                pickup = new Object(); 

                pickup.Initialize(randomizer, gridwidth, gridheight, wallList); //Creates new pickup

                pickupList.Add(pickup); //Adds to list of pickups
            }
        }

        //Creates game space
        static void CreateGrid(int gridwidth, int gridheight)
        {
            Grid = new Tile[gridheight, gridwidth]; //New array of required lengths
            for (int i = 0; i < gridheight; i++)
            {
                for (int j = 0; j < gridwidth; j++)
                {
                    Grid[i, j] = new Tile(); //Cycles through all array elements and creates blank tile class
                }

            }
        }

        //Draws the inital grid onto the console
        static void DrawGrid()
        {
            Console.Clear();

            for (int i = 0; i < gridHeight; i++)
            {
                for (int j = 0; j < gridWidth; j++)
                {

                    Console.ForegroundColor = Grid[i, j].colour;
                    Console.Write(Grid[i, j].gridIcon); //Cycles through all grid elements and draws their icon and colour

                }
            }

            Console.SetCursorPosition(0, Console.WindowHeight * 2 / 3);//Puts cursor two thirds of the way down the console
            for (int i = 0; i < Grid.GetLength(1); i++)
            {
                Console.Write("-"); //Creates a border for the game grid
            }

            DrawTitle(13, Console.WindowHeight * 2 / 3);//Draws the Snake tile under the game grid
        }


        //Creates a border to prevent the player leaving the screen, currently unused
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
                wall.Initialize(i, gridHeight - 1);
                wallList.Add(wall);

            }
            for (int i = 0; i < gridHeight; i++)
            {

                wall = new Object();
                wall.Initialize(0, i);
                wallList.Add(wall);

            }
            for (int i = 0; i < gridHeight; i++)
            {

                wall = new Object();
                wall.Initialize(gridWidth - 1, i);
                wallList.Add(wall);

            }
        }


        //Draws the Snake title at required position
        static void DrawTitle(int x, int y)
        {

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.SetCursorPosition(x, y + 1); //Moves the cursor down a line and to the right x position

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


        //Draws all player scores under game grid
        static void DrawScore()
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            for (int i = 0; i < players.Count; i++)
            {
                Console.SetCursorPosition(10, Console.WindowHeight * 2 / 3 + 10 +i);
                players[i].score =((players[i].snakeLength - startLength) * ((difficulty / 3) + 1));//Creates score and store in player class
                Console.Write(players[i].username + "'s score: " + players[i].score);//Draws player score and username under the game grid
            }
        }

        //Updates the grid to current icons
        static void UpdateGrid()
        {
            for (int i = 0; i < Grid.GetLength(0); i++)
            {
                for (int j = 0; j < Grid.GetLength(1); j++)
                {

                    if (Grid[i, j].hasChanged == true)//Only updates if the icon has changed since last update
                    {

                        Console.SetCursorPosition(j, i);//Moves cursor to required position
                        Console.ForegroundColor = Grid[i, j].colour;
                        Console.Write(Grid[i, j].gridIcon);//Changes the old icon to current icon

                        Grid[i, j].hasChanged = false;
                    }
                    Grid[i, j].Update(); //Calls the update method of every Grid Tile
                }
            }
        }

        //Allows Grid icons to represent the direction the player is going
        static void DirectionConvert(Player player)
        {
            if (player.currentDirection == Player.Direction.Up)
            {

                Grid[player.headPosition[0, 1], player.headPosition[0, 0]].currentDirection = Tile.Direction.Up;
            }
            if (player.currentDirection == Player.Direction.Down)
            {

                Grid[player.headPosition[0, 1], player.headPosition[0, 0]].currentDirection = Tile.Direction.Down;
            }
            if (player.currentDirection == Player.Direction.Left)
            {

                Grid[player.headPosition[0, 1], player.headPosition[0, 0]].currentDirection = Tile.Direction.Left;
            }
            if (player.currentDirection == Player.Direction.Right)
            {

                Grid[player.headPosition[0, 1], player.headPosition[0, 0]].currentDirection = Tile.Direction.Right;
            }
        }

        //Checks if two grid positions are the same
        static bool Collision(int[,] one, int[,] two)
        {
            if (one[0, 0] == two[0, 0] && one[0, 1] == two[0, 1])//Checks both x and y coordinates 
            {
                return true; //Returns true if positions are the same
            }
            return false;
        }

        //Input method to give all players the same input, prevents inputs overwriting each other
        static void Input()
        {
            while (true)
            {
                  input = Console.ReadKey(true);//Reads next key input, stops it being shown
                
            }
        }


        //Initializes everything required to start a game
        static void Initialize()
        {

            wallList = new List<Object>(); //Resets any walls
            pickupList = new List<Object>(); //Resets all pickups
            gridHeight = Console.WindowHeight * 2 / 3; //Sets game grid to two thirds of the console window height
            gridWidth = Console.WindowWidth; //Sets game grid to the console window width
            gameRunning = true;
            CreateGrid(gridWidth, gridHeight); //Creates new grid
            startSpeed = 150 - (5 * difficulty); // Sets starting speed of snake depending on difficulty
            maxPickups = 10; //Sets maximum pickups 
            lengthAdded = (2 * difficulty); //Sets snake length added when collecting pickup depending on difficulty 
            startLength = (3 * difficulty); //Sets inital snake length depending on difficulty
            pickupNoAdded = 1; //Sets amount of extra pickups added when one is collected
            speedAdded = (difficulty); //Sets the amount the speed is increased by when pickups are collected depending on difficulty 
            currentPickupNo = 1; //Sets inital amount of pickups
            maxSpeed = 70 - (5 * difficulty); //Sets the maximum speed 

            players = new List<Player>(); //Creates a blank player list

            //Creates a new player depending on how many the user requested
            for (int i = 1; i <= numOfPlayers; i++)
            {
                Player player;

                player = new Player();

                player.Initialize(5, 5 + 5 * i, startLength, gridWidth, gridHeight, startSpeed, i); //Set inital values for the player
                
                Console.Clear();
                DrawTitle(13, 2);
                Console.SetCursorPosition(0, 12);
                Console.WriteLine("\tPlayer "+i+" please enter a username: "); //Asks for username for use in highscores
                player.username = Console.ReadLine(); //Takes input and stores it in player class
                players.Add(player); //Store new player in list
            }


            InputThread = new Thread(Input); 
            InputThread.Start(); //Begins new input thread that constantly gets console input


            List<Thread> playerThreads = new List<Thread>(); //List for all player threads

            //Creates new thread for each player
            foreach (Player plyr in players)
            {
                Thread playerthread = new Thread(() => PlayerLogic(plyr)); //Creates new thread with a variable from the player list
                playerThreads.Add(playerthread); //Stores the thread
                

            }

            DrawGrid(); //Draws the intial grid

            //Starts every player thread
            foreach(Thread playerthread in playerThreads)
            {
                playerthread.Start();
            }

        }



        //Everything required to update the players position on the grid, also handles collisions
        static void PlayerLogic(Player player)
        {

            while (true)
            {
                int[,] previouspos = player.headPosition; // Stores the head position before anything changes
                player.Update(); //Calls player update method, updating player position numbers
                DirectionConvert(player);
                Grid[player.bodyPositions[player.bodyPositions.Count - 1][0, 1], player.bodyPositions[player.bodyPositions.Count - 1][0, 0]].containsHead = false; //Deletes previous head
                Grid[player.bodyPositions[player.bodyPositions.Count - 1][0, 1], player.bodyPositions[player.bodyPositions.Count - 1][0, 0]].headNumber -= 1; //Removes a head number, head number allows for collisions with other players heads, without causing collisions with itself

              
                Grid[player.headPosition[0, 1], player.headPosition[0, 0]].headNumber += 1;

                Grid[player.headPosition[0, 1], player.headPosition[0, 0]].containsHead = true; //Updates icon in new head position



                Grid[player.headPosition[0, 1], player.headPosition[0, 0]].Update(); //Calls tile update

                //Wall collision 
                if (wallList.Count > 0)
                {
                    for (int i = 0; i < wallList.Count; i++)
                    {
                        if (Collision(wallList[i].position, player.headPosition))
                        {
                            player.active = false; //Kills player on collision
                        }
                    }
                }


                //Collision with pickups
                for (int i = 0; i < pickupList.Count; i++)
                {
                    if (Collision(pickupList[i].position, player.headPosition))
                    {


                        Grid[pickupList[i].position[0, 1], pickupList[i].position[0, 0]].containsPickup = false; //Deletes pickup on collect

                        Grid[pickupList[i].position[0, 1], pickupList[i].position[0, 0]].Update(); //Calls tile update

                        pickupList.RemoveAt(i); //Removes collected pickup from the list

                        player.snakeLength += lengthAdded; //Increases snake length 

                        if (currentPickupNo < maxPickups)
                        {
                            currentPickupNo += pickupNoAdded; //Adds a new pickup to the grid if the current amount is lower than max
                        }

                        if (player.playerSpeed > maxSpeed)
                        {
                            player.playerSpeed -= speedAdded;
                            if (player.playerSpeed > maxSpeed)
                            {
                                player.playerSpeed = maxSpeed;
                            }
                        }

                    }
                }
                
                
                foreach (int[,] body in player.bodyPositions)
                {

                    Grid[body[0, 1], body[0, 0]].containsBody = true;
                    Grid[body[0, 1], body[0, 0]].Update();

                }
                if (player.CheckLength())
                {

                    Grid[player.bodyPositions[0][0, 1], player.bodyPositions[0][0, 0]].didContainBody = true;
                    Grid[player.bodyPositions[0][0, 1], player.bodyPositions[0][0, 0]].containsBody = false;
                    Grid[player.bodyPositions[0][0, 1], player.bodyPositions[0][0, 0]].Update();

                    player.bodyPositions.RemoveAt(0);

                }

                if (Grid[player.headPosition[0, 1], player.headPosition[0, 0]].containsBody == true)
                {
                    player.active = false;
                }
                if (Grid[player.headPosition[0, 1], player.headPosition[0, 0]].containsHead == true && Grid[player.headPosition[0, 1], player.headPosition[0, 0]].headNumber > 1)
                {
                    player.active = false;
                }
                if (player.active == false)
                {
                    Thread.CurrentThread.Abort();

                    Thread.CurrentThread.Join();
                }
                if (player.playerSpeed != 0)
                {
                    new System.Threading.ManualResetEvent(false).WaitOne(player.playerSpeed);
                }


            }
        }
        static void GridLogic()
        {

            for (int i = 0; i < pickupList.Count; i++)
            {

                Grid[pickupList[i].position[0, 1], pickupList[i].position[0, 0]].containsPickup = true;

            }
            try
            {

                for (int i = 0; i < wallList.Count; i++)
                {

                    Grid[wallList[i].position[0, 1], wallList[i].position[0, 0]].containsWall = true;
                }

            }
            catch { }


            UpdateGrid();
        }
        static void GameOver()
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Clear();
            Console.SetCursorPosition((gridWidth - 9) / 2, gridHeight / 2);

            Console.Write("Game Over");

            for (int i = 0; i < players.Count; i++)
            {
                Console.SetCursorPosition((gridWidth - 9) / 2, 1 + (gridHeight / 2) + i);
                Console.Write(players[i].username + "'s score: " + players[i].score);
                ScoreObject score = new ScoreObject();
                score.score = players[i].score;
                score.username = players[i].username;
                if (difficulty == 1)
                {
                    easyHighScores.Add(score);
                }
                if (difficulty == 2)
                {
                    normalHighScores.Add(score);
                }
                if (difficulty == 3)
                {
                    hardHighScores.Add(score);
                }
                if (difficulty == 7)
                {
                    impossibleHighScores.Add(score);
                }
            }
            SaveHighScores(easyHighScores, "easy");
            SaveHighScores(normalHighScores, "normal");
            SaveHighScores(hardHighScores, "hard");
            SaveHighScores(impossibleHighScores, "impossible");

            foreach (Player player in players)
            {
                player.GameEnd();
            }
            foreach (Thread playerthread in playerThreads)
            {
                playerthread.Abort();
            }
            
        }



        static void SaveHighScores(List<ScoreObject> highscore, string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<ScoreObject>));
            string location = @"" + filename + "Scores.xml";
            using (TextWriter writer = new StreamWriter(location))
            {
                serializer.Serialize(writer, highscore);
            } 
           

        }
        static void ClearHighScores( string filename)
        {
            List<ScoreObject> blank = new List<ScoreObject>();
            XmlSerializer serializer = new XmlSerializer(typeof(List<ScoreObject>));
            string location = @"" + filename + "Scores.xml";
            using (TextWriter writer = new StreamWriter(location))
            {
                serializer.Serialize(writer, blank);
            }

        }
        static List<ScoreObject> LoadHighScores(string filename)
        {
            List<ScoreObject> highscore = new List<ScoreObject>();
            StreamReader streamReader;
            XmlSerializer serializer = new XmlSerializer(typeof(List<ScoreObject>));
            string location = @"" + filename + "Scores.xml";
            try
            {
                 streamReader = new StreamReader(location);
            }
            catch
            {
                using (TextWriter writer = new StreamWriter(location))
                {
                    serializer.Serialize(writer, highscore);
                }
                 streamReader = new StreamReader(location);
            }


            highscore = (List<ScoreObject>)serializer.Deserialize(streamReader);
            streamReader.Close();


            return highscore;
        }

        static List<ScoreObject> SortHighScores(List<ScoreObject> highscore)
        {
            ScoreObject scoreHolder= new ScoreObject();
            bool sorted = false;
            while (!sorted)
            {
                sorted = true;
                for (int i = 0; i < highscore.Count - 1; i++)
                {
                    if (highscore[i].score < highscore[i + 1].score)
                    {
                        scoreHolder = highscore[i];
                        highscore[i] = highscore[i + 1];
                        highscore[i + 1] = scoreHolder;
                        sorted = false;
                    }
                }
            }
            return highscore;
        }

        static void DisplayHighScores(List<ScoreObject> highscore, string type)
        {
            Console.Clear();
            DrawTitle(13, 2);
            Console.SetCursorPosition(0, 12);
            Console.WriteLine("\t"+type+" scores");
            foreach(ScoreObject score in highscore)
            {
                Console.WriteLine();
                Console.WriteLine("\t"+score.username + " " + score.score);
            }
            Console.ReadKey();

        }




        static void SinglePlayerGameLoop()
        {
            while (gameRunning)
            {
                AddPickup(gridWidth, gridHeight, maxPickups);
                DrawScore();
                GridLogic();
                playersDead = 0;
                foreach (Player player in players)
                {
                    if (player.active == false)
                    {
                        playersDead++;
                    }

                }
                if (playersDead == players.Count)
                {
                    InputThread.Abort();

                    InputThread.Join();

                    gameRunning = false;
                }
            }

            GameOver();

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
        static bool PlayerSelect()
        {
            bool valid = true;
            do
            {
                Console.Clear();
                valid = true;
                DrawTitle(13, 2);
                Console.SetCursorPosition(0, 12);
                Console.WriteLine("\tHow many players? (max of 2):");
                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.D1:

                        numOfPlayers = 1;
                        return true;
                    case ConsoleKey.D2:

                        numOfPlayers = 2;
                        return true;
                    case ConsoleKey.D3:
                       
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

        static void CreateServer()
        {
            Console.Clear();
            serverThread = new Thread(new ThreadStart(ServerStart));
            serverThread.Start();


        }
        static bool connected = false;
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
                client.startClient(clientSocket, counter);
                connected = true;
            }
        }




        static void ClientStart()
        {
            clientSocket.Connect("127.0.0.1", 8888);

            Console.WriteLine("Connected ...");
            DrawGrid();
            while (gameRunning)
            {

                NetworkStream serverStream = clientSocket.GetStream();
                byte[] outStream = SerializeToBytes<ConsoleKeyInfo>(input);


                serverStream.Write(outStream, 0, outStream.Length);
                serverStream.Flush();

                byte[] inStream = new byte[200000];
                serverStream.Read(inStream, 0, 200000);
                previousGrid = Grid;
                Grid = (Tile[,])DeserializeFromBytes(inStream);
                for (int i = 0; i < Grid.GetLength(0); i++)
                {
                    for (int j = 0; j < Grid.GetLength(1); j++)
                    {

                        Grid[i, j].didContainWall = previousGrid[i, j].containsWall;
                        Grid[i, j].didContainHead = previousGrid[i, j].containsHead;
                        Grid[i, j].didContainBody = previousGrid[i, j].containsBody;
                        Grid[i, j].didContainPickup = previousGrid[i, j].containsPickup;
                    }
                }


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
                    numOfPlayers = 2;
                    
                    CreateServer();
                    do
                    {
                        if (connected)
                        {
                            Initialize();
                            SinglePlayerGameLoop();
                        }
                    }
                    while (!connected);


                    break;
                case ConsoleKey.D2:
                    numOfPlayers = 0;
                    Initialize();
                    ClientStart();
                    break;
                case ConsoleKey.D3:
                    break;
            }
        }
        static bool ClearSelect()
        {
            bool valid;
            do
            {
                Console.Clear();
                valid = true;
                DrawTitle(13, 2);
                Console.SetCursorPosition(0, 12);
                Console.WriteLine("\tClear scores for:\n\t1:Easy\n\t2:Medium\n\t3:Hard\n\t4:Impossible\n\t5:Back");
                


                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.D1:

                        ClearHighScores("Easy");
                        easyHighScores = new List<ScoreObject>();
                        return true;
                    case ConsoleKey.D2:
                         ClearHighScores("Normal");
                        normalHighScores = new List<ScoreObject>();
                        return true;
                    case ConsoleKey.D3:
                         ClearHighScores("Hard");
                        hardHighScores = new List<ScoreObject>();
                        return true;
                    case ConsoleKey.D4:
                         ClearHighScores("Impossible");
                        impossibleHighScores = new List<ScoreObject>();
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
        static bool DrawHighScores()
        {
            bool valid;
            do
            {
             Console.Clear();
                 valid = true;
                DrawTitle(13, 2);
                Console.SetCursorPosition(0, 12);
                Console.WriteLine("\tShow scores for:\n\t1:Easy\n\t2:Medium\n\t3:Hard\n\t4:Impossible\n\t5:Clear Scores\n\t6:Back");
                easyHighScores = SortHighScores(easyHighScores);

                normalHighScores = SortHighScores(normalHighScores);

                hardHighScores = SortHighScores(hardHighScores);

                impossibleHighScores = SortHighScores(impossibleHighScores);
               
               
                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.D1:

                        DisplayHighScores(easyHighScores,"Easy");
                        return true;
                    case ConsoleKey.D2:
                        DisplayHighScores(normalHighScores,"Normal");
                        return true;
                    case ConsoleKey.D3:
                        DisplayHighScores(hardHighScores,"Hard");
                        return true;
                    case ConsoleKey.D4:
                        DisplayHighScores(impossibleHighScores,"Impossible");
                        return true;
                    case ConsoleKey.D5:
                        ClearSelect();
                        return true;
                    case ConsoleKey.D6:
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
            easyHighScores = LoadHighScores("easy");

            normalHighScores = LoadHighScores("normal");

            hardHighScores = LoadHighScores("hard");

            impossibleHighScores = LoadHighScores("impossible");
            while (running)
            {
                
                Console.Title = "Snake";
                Console.CursorVisible = false;
                Console.WindowHeight = 50;
                Console.Clear();
                DrawTitle(13, 2);
                Console.SetCursorPosition(0, 12);
                Console.WriteLine("\t1:Single Player\n\t2:Multiplayer\n\t3:Scores\n\t4:Exit");
                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.D1:

                        if (DifficultySelect())
                        {
                            if (PlayerSelect())
                            {
                           
                            Initialize();
                            SinglePlayerGameLoop();
                        }
                        }
                        break;
                    case ConsoleKey.D2:

                        difficulty = 3;


                        MultiplayerSelect();

                        break;
                    case ConsoleKey.D3:
                         while(DrawHighScores())
                         {
                             
                         }
                        
                        

                        break;
                    case ConsoleKey.D4:
                        running = false;
                        break;
                }

            }

        }
    }
}

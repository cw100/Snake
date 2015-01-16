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

        public static bool multiplayer = false;
        public static Options currentOptions = new Options();
        public static List<Thread> playerThreads = new List<Thread>();
        static int playersDead = 0;
        public static List<Player> players = new List<Player>();
        static Server server = new Server();
        public static Thread serverThread;
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
        public static bool gameRunning = true;
        static bool running = true;
        static Random randomizer = new Random();
        public static List<Object> pickupList;
        static Object pickup;
        static int maxPickups;
        public static int difficulty;
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

                    Grid[i, j].didContainBody = false;
                    Grid[i, j].didContainHead = false;
                    Grid[i, j].didContainPickup = false;
                    Grid[i, j].didContainWall = false;
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

            DrawTitle((currentOptions.windowWidth / 2) - 27, Console.WindowHeight * 2 / 3);//Draws the Snake tile under the game grid
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
                Console.SetCursorPosition(10, Console.WindowHeight * 2 / 3 + 10 + i);
                players[i].score = ((players[i].snakeLength - startLength) * ((difficulty / 3) + 1));//Creates score and store in player class
                Console.Write(players[i].username + "'s score: " + players[i].score);//Draws player score and username under the game grid
            }
            if (multiplayer)
            {
                if (players.Count > 0)
                {
                    for (int i = 0; i < players.Count; i++)
                    {
                        if (Grid[0, 0].multiplayerscores.Count < players.Count)
                        {
                            for (int j = 0; j < players.Count; j++)
                            {
                                Grid[0, 0].multiplayerscores.Add(1);
                            }
                        }
                        Grid[0, 0].multiplayerscores[i] = ((players[i].snakeLength - startLength) * ((difficulty / 3) + 1));//Creates score and store in player class
                    }
                }
                for (int i = 0; i < Grid[0, 0].multiplayerscores.Count; i++)
                {

                    Console.SetCursorPosition(10, Console.WindowHeight * 2 / 3 + 10 + i);
                    Console.Write("Player " + (i + 1) + "'s score: " + Grid[0, 0].multiplayerscores[i]);//Draws player score and username under the game grid
                }
            }
        }

        //Updates the grid to current icons
        static void UpdateGrid()
        {
            for (int i = 0; i < Grid.GetLength(0); i++)
            {
                for (int j = 0; j < Grid.GetLength(1); j++)
                {
                    //Fore checking updates
                    Grid[i, j].didContainBody = Grid[i, j].containsBody;
                    Grid[i, j].didContainHead = Grid[i, j].containsHead;
                    Grid[i, j].didContainPickup = Grid[i, j].containsPickup;
                    Grid[i, j].didContainWall = Grid[i, j].containsWall;
                    Grid[i, j].Update(); //Calls the update method of every Grid Tile
                    if (Grid[i, j].hasChanged == true)//Only updates if the icon has changed since last update
                    {

                        Console.SetCursorPosition(j, i);//Moves cursor to required position
                        Console.ForegroundColor = Grid[i, j].colour;
                        Console.Write(Grid[i, j].gridIcon);//Changes the old icon to current icon

                        Grid[i, j].hasChanged = false;
                    }


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
            if (numOfPlayers > 0)
            {
                if (WallToggle())
                {
                    LevelOne();
                }
            }
            players = new List<Player>(); //Creates a blank player list

            //Creates a new player depending on how many the user requested
            for (int i = 1; i <= numOfPlayers; i++)
            {
                Player player;

                player = new Player();

                player.Initialize(5, 5 + 5 * i, startLength, gridWidth, gridHeight, startSpeed, i); //Set inital values for the player
                if (!multiplayer)
                {
                    Console.Clear();
                    DrawTitle((currentOptions.windowWidth / 2) - 27, 2);
                    Console.SetCursorPosition(0, 12);

                    Console.WriteLine("\tPlayer " + i + " please enter a username: "); //Asks for username for use in highscores
                    player.username = Console.ReadLine(); //Takes input and stores it in player class
                }
                else
                {
                    player.username = "Player " + i;
                }
                if (multiplayer == true && i > 1)
                {
                    player.multiplayer = true;
                }
                players.Add(player); //Store new player in list
            }


            InputThread = new Thread(Input);
            InputThread.Start(); //Begins new input thread that constantly gets console input

            playerThreads = new List<Thread>(); //List for all player threads

            //Creates new thread for each player
            foreach (Player plyr in players)
            {
                Thread playerthread = new Thread(() => PlayerLogic(plyr)); //Creates new thread with a variable from the player list
                playerThreads.Add(playerthread); //Stores the thread


            }



            //Starts every player thread
            if (!multiplayer)
            {
                DrawGrid(); //Draws the intial grid
                foreach (Thread playerthread in playerThreads)
                {
                    playerthread.Start(); //Starts the thread
                }
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

                        if (player.playerSpeed > maxSpeed) //If Player is going slower than the max,  speed is increased on pickup collect
                        {
                            player.playerSpeed -= speedAdded;
                            if (player.playerSpeed < maxSpeed) //If speed goes over max, set to max
                            {
                                player.playerSpeed = maxSpeed;
                            }
                        }

                    }
                }


                foreach (int[,] body in player.bodyPositions)
                {

                    Grid[body[0, 1], body[0, 0]].containsBody = true; //Adds every snake body for this player to the grid
                    Grid[body[0, 1], body[0, 0]].Update();

                }
                if (player.CheckLength()) //Calls a method for checking if the snake length is higher than max
                {

                    Grid[player.bodyPositions[0][0, 1], player.bodyPositions[0][0, 0]].didContainBody = true; //Needed to prevent desyncs in multiplayer
                    Grid[player.bodyPositions[0][0, 1], player.bodyPositions[0][0, 0]].containsBody = false; // Removes the oldest body in the list from the grid
                    Grid[player.bodyPositions[0][0, 1], player.bodyPositions[0][0, 0]].Update();

                    player.bodyPositions.RemoveAt(0); //Removes the oldest body in the list from the list

                }

                if (Grid[player.headPosition[0, 1], player.headPosition[0, 0]].containsBody == true) //Collisions with bodys
                {
                    player.active = false;
                }
                if (Grid[player.headPosition[0, 1], player.headPosition[0, 0]].containsHead == true && Grid[player.headPosition[0, 1], player.headPosition[0, 0]].headNumber > 1) //Collisions with other player heads, head number prevents collision with own head
                {
                    player.active = false;
                }
                if (player.active == false)
                {
                    Thread.CurrentThread.Abort(); //Stops the current players thread if player dies
                    Thread.CurrentThread.Join();
                }
                if (player.playerSpeed != 0)
                {
                    new System.Threading.ManualResetEvent(false).WaitOne(player.playerSpeed); //Pauses thread, allows snake to move diferent speeds
                }


            }
        }


        //Handles adding pickups and walls to the grid
        static void GridLogic()
        {

            for (int i = 0; i < pickupList.Count; i++)
            {

                Grid[pickupList[i].position[0, 1], pickupList[i].position[0, 0]].containsPickup = true; //Adds every pickup to grid

            }

            if (0 < wallList.Count)
            {
                for (int i = 0; i < wallList.Count; i++)
                {
                    Grid[wallList[i].position[0, 1], wallList[i].position[0, 0]].containsWall = true; // Adds every wall to the grid
                }

            }


            UpdateGrid();
        }


        //Method that is called on every game end
        static void GameOver()
        {
            Console.ForegroundColor = ConsoleColor.Gray; // Resets all console colour
            Console.Clear();
            Console.SetCursorPosition((gridWidth / 2) - 4, gridHeight / 2); //Moves cursor to middle of screan

            started = false; //Stops the server sockets, if running
            Console.Write("Game Over");


            if (!multiplayer)
            {

                //Cycles through every player and displays and saves score
                for (int i = 0; i < players.Count; i++)
                {
                    Console.SetCursorPosition((gridWidth / 2) - 4, 1 + (gridHeight / 2) + i);
                    Console.Write(players[i].username + "'s score: " + players[i].score); //Displays score


                    ScoreObject score = new ScoreObject(); //Creates new score to save to lists
                    score.score = players[i].score;
                    score.username = players[i].username;

                    //Saves score to list depending on difficulty
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

                //Saves scores to xml file
                SaveHighScores(easyHighScores, "easy");
                SaveHighScores(normalHighScores, "normal");
                SaveHighScores(hardHighScores, "hard");
                SaveHighScores(impossibleHighScores, "impossible");
            }


            if (multiplayer)
            {
                for (int i = 0; i < Grid[0, 0].multiplayerscores.Count; i++) // Uses Grid[0,0] to save and load scores for lan multiplayer
                {
                    Console.SetCursorPosition((gridWidth / 2) - 4, 1 + (gridHeight / 2) + i);
                    Console.Write("Player " + (i + 1) + "'s score: " + Grid[0, 0].multiplayerscores[i]);//Draws player score and username under the game grid
                }
            }


            foreach (Player player in players)
            {
                player.GameEnd();//Ends all the input threads for the players
            }
            foreach (Thread playerthread in playerThreads)
            {
                playerthread.Abort();//Ends all the player threads, if not already ended
            }

            Console.ReadKey(true); //Waits for input before returning to main menu

        }


        //Saves score lists to xml file
        static void SaveHighScores(List<ScoreObject> highscore, string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<ScoreObject>)); //Creates a serializer for ScoreObject lists


            string location = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Snake\" + filename + @"Scores.xml"; //Sets the file location and name, stored in %appdata% folder


            using (TextWriter writer = new StreamWriter(location)) //Creates a textwriter for saving the xml file, opening the xml file
            {
                serializer.Serialize(writer, highscore); //Serializes the highscore list to a xml file
            }


        }


        //Resets the highscore file to blank
        static void ClearHighScores(string filename)
        {
            List<ScoreObject> blank = new List<ScoreObject>(); //New blank list for reseting file
            XmlSerializer serializer = new XmlSerializer(typeof(List<ScoreObject>)); //Creates a serializer for ScoreObject lists
            string location = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Snake\" + filename + @"Scores.xml"; //Sets the file location and name, stored in %appdata% folder

            using (TextWriter writer = new StreamWriter(location)) //Creates a textwriter for saving the xml file, opening the xml file
            {
                serializer.Serialize(writer, blank); //Serializes the blank list to a xml file, which resets the file to blank

            }

        }

        //Opens an xml file and deserializes the List contained in the file, returning it. If file doesn't exists file is created
        static List<ScoreObject> LoadHighScores(string filename)
        {
            List<ScoreObject> highscore = new List<ScoreObject>();// List for returning
            StreamReader streamReader;//Stream reader for opening file
            XmlSerializer serializer = new XmlSerializer(typeof(List<ScoreObject>));//Xml serializer for deserializing the xml file to the List
            string location = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Snake\" + filename + @"Scores.xml"; //Sets the file location and name, stored in %appdata% folder

            try
            {
                streamReader = new StreamReader(location); //Trys to open file
            }
            catch
            {
                using (TextWriter writer = new StreamWriter(location))
                {
                    serializer.Serialize(writer, highscore);//Creates blank file if it doesn't exist
                }
                streamReader = new StreamReader(location);
            }


            highscore = (List<ScoreObject>)serializer.Deserialize(streamReader); //Deserializes the xml file to List
            streamReader.Close();//Closes the stream reader
            return highscore; //Returns to List from the file
        }


        //For serializing Options class to xml file
        static void SaveOptions(Options options)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Options)); //Creates a Options class xml serializer
            string location = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Snake\" + @"Options.xml"; //Sets the file location and name, stored in %appdata% folder

            using (TextWriter writer = new StreamWriter(location))
            {
                serializer.Serialize(writer, options);//Serializes options class to file
            }

        }

        //Resets file options to default, not currently used
        static void DefaultOptions()
        {

            Options options = new Options();
            XmlSerializer serializer = new XmlSerializer(typeof(List<ScoreObject>));
            string location = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Snake\" + @"Options.xml";
            using (TextWriter writer = new StreamWriter(location))
            {
                serializer.Serialize(writer, options);
            }
            currentOptions = options;

        }

        // Loads and deserializes the options xml, sets options in file to current options
        static Options LoadOptions()
        {
            Options options = new Options(); //Creates blank options class for returning
            StreamReader streamReader;
            XmlSerializer serializer = new XmlSerializer(typeof(Options)); //Creates a Options class xml serializer
            string location = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Snake\" + @"Options.xml"; //Sets the file location and name, stored in %appdata% folder

            try
            {
                streamReader = new StreamReader(location); //Attempts to open the options xml file
            }
            catch
            {
                using (TextWriter writer = new StreamWriter(location))
                {
                    serializer.Serialize(writer, options); //Creates the file if it doesn't exist
                }
                streamReader = new StreamReader(location);
            }


            options = (Options)serializer.Deserialize(streamReader); //Sets the options in the file to the options class to be returned
            streamReader.Close(); //Closes the file
            return options; //Returns options
        }

        //Sorts the Highscores into desending order with a Bubble sort
        static List<ScoreObject> SortHighScores(List<ScoreObject> highscore)
        {
            ScoreObject scoreHolder = new ScoreObject();//For holding scores temporarily

            bool sorted = false;//Creats sorted bool

            while (!sorted) //Continues to sort if not finished
            {
                sorted = true; //Assumes its sorted
                for (int i = 0; i < highscore.Count - 1; i++) //Cycles through all highscores
                {
                    if (highscore[i].score < highscore[i + 1].score) // If current score is lower then next, swap them
                    {
                        scoreHolder = highscore[i];
                        highscore[i] = highscore[i + 1];
                        highscore[i + 1] = scoreHolder;
                        sorted = false; //Forces another cycle of the code to check if its finished
                    }
                }
            }
            return highscore;//Returns the sorted list
        }


        //Draws the highscores onto the console
        static void DisplayHighScores(List<ScoreObject> highscore, string type)
        {

            //Clears screen and draws the title
            Console.Clear();
            DrawTitle((currentOptions.windowWidth / 2) - 27, 2);
            Console.SetCursorPosition(0, 12);


            Console.WriteLine("\t" + type + " scores"); //Tells user what type of scores they're looking at
            Console.WriteLine();
            foreach (ScoreObject score in highscore)
            {

                Console.WriteLine("\t" + score.username + " " + score.score); //Displays each score in order 
            }
            Console.ReadKey(true); //Waits for user input

        }



        //Loop that handles game logic
        static void GameLoop()
        {
            while (gameRunning)//Only runs while game is running
            {
                AddPickup(gridWidth, gridHeight, maxPickups);//Trys to add a pickup
                DrawScore();//Constantly draws the current score to screen
                GridLogic();//Logic for adding pickups and walls to grid
                playersDead = 0;//Resets amount of dead players
                foreach (Player player in players)
                {
                    if (player.active == false)
                    {
                        playersDead++;//Counts number of dead players
                    }

                }

                //Ends the game if all players are dead
                if (playersDead == players.Count)
                {
                    InputThread.Abort();

                    InputThread.Join();

                    gameRunning = false;
                    GameOver();//Call all logic required to end the game
                }
            }



        }

        //Logic to let users change controls
        static bool EditKeyBindingsMenu()
        {
            bool valid = true;//For checking user entered the right input
            do
            {
                //Draws title
                Console.Clear();
                valid = true;
                DrawTitle((currentOptions.windowWidth / 2) - 27, 2);



                Console.SetCursorPosition(0, 12);
                Console.WriteLine("\tEdit Bindings for:\n\t1:Player One\n\t2:Player Two\n\t3:Back");//Writes menu info

                //Menu
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.D1:
                        while (EditBindings("one"))//Opens the bindings menu for player one, keeps open until user exits or changes something
                        {

                        }
                        SaveOptions(currentOptions);//Saves binding changes
                        return true;//Keeps user in the menu
                    case ConsoleKey.D2:
                        while (EditBindings("two"))//Opens the bindings menu for player tow, keeps open until user exits or changes something
                        {

                        }
                        SaveOptions(currentOptions);//Saves binding changes
                        return true;//Keeps user in the menu

                    case ConsoleKey.D3:
                        return false;//Exit menu
                    default:
                        valid = false;//Input is wrong
                        break;
                }
            }
            while (!valid);//Keeps menu going while input is wrong
            return true;//Keeps user in the menu
        }

        //For editing key bindings
        public static bool EditBindings(string player)
        {
            bool valid = true;
            do
            {
                //Draws title
                Console.Clear();
                valid = true;
                DrawTitle((currentOptions.windowWidth / 2) - 27, 2);

                //Menus for each player, displays current key bindings
                Console.SetCursorPosition(0, 12);
                if (player == "one")
                {
                    Console.WriteLine("\tCurrent bindings for Player " + player +
                        "\n\t1:Move Up:    " + currentOptions.playerOneUpKey +
                        "\n\t2:Move Left:  " + currentOptions.playerOneLeftKey +
                        "\n\t3:Move Right: " + currentOptions.playerOneRightKey +
                        "\n\t4:Move Down:  " + currentOptions.playerOneDownKey +
                        "\n\t5:Defaults" +
                        "\n\t6:Back");
                }
                if (player == "two")
                {
                    Console.WriteLine("\tCurrent bindings for Player " + player +
                        "\n\t1:Move Up:    " + currentOptions.playerTwoUpKey +
                        "\n\t2:Move Left:  " + currentOptions.playerTwoLeftKey +
                        "\n\t3:Move Right: " + currentOptions.playerTwoRightKey +
                        "\n\t4:Move Down:  " + currentOptions.playerTwoDownKey +
                        "\n\t5:Defaults" +
                        "\n\t6:Back");
                }

                //Menu selection
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.D1:
                        Console.Clear();
                        DrawTitle((currentOptions.windowWidth / 2) - 27, 2);
                        Console.SetCursorPosition(0, 12);

                        Console.WriteLine("\tPlease select new Move Up for Player " + player + ":");
                        //Sets user input to the binding for that key
                        if (player == "one")
                        {
                            currentOptions.playerOneUpKey = Console.ReadKey(true).Key;
                        }
                        if (player == "two")
                        {
                            currentOptions.playerTwoUpKey = Console.ReadKey(true).Key;
                        }
                        return true;
                    case ConsoleKey.D2:
                        Console.Clear();
                        DrawTitle((currentOptions.windowWidth / 2) - 27, 2);
                        Console.SetCursorPosition(0, 12);

                        Console.WriteLine("\tPlease select new Move Left for Player " + player + ":");
                        //Sets user input to the binding for that key
                        if (player == "one")
                        {
                            currentOptions.playerOneLeftKey = Console.ReadKey(true).Key;
                        }
                        if (player == "two")
                        {
                            currentOptions.playerTwoLeftKey = Console.ReadKey(true).Key;
                        }
                        return true;
                    case ConsoleKey.D3:
                        Console.Clear();
                        DrawTitle((currentOptions.windowWidth / 2) - 27, 2);
                        Console.SetCursorPosition(0, 12);
                        Console.WriteLine("\tPlease select new Move Right for Player " + player + ":");
                        //Sets user input to the binding for that key
                        if (player == "one")
                        {
                            currentOptions.playerOneRightKey = Console.ReadKey(true).Key;
                        }
                        if (player == "two")
                        {
                            currentOptions.playerTwoRightKey = Console.ReadKey(true).Key;
                        }
                        return true;
                    case ConsoleKey.D4:
                        Console.Clear();
                        DrawTitle((currentOptions.windowWidth / 2) - 27, 2);
                        Console.SetCursorPosition(0, 12);
                        //Sets user input to the binding for that key
                        Console.WriteLine("\tPlease select new Move Down for Player " + player + ":");
                        if (player == "one")
                        {
                            currentOptions.playerOneDownKey = Console.ReadKey(true).Key;
                        }
                        if (player == "two")
                        {
                            currentOptions.playerTwoDownKey = Console.ReadKey(true).Key;
                        }
                        return true;

                    case ConsoleKey.D5:

                        Options options = new Options();//Creates blank option class with default keys


                        //Sets user input to default
                        if (player == "one")
                        {
                            currentOptions.playerOneUpKey = options.playerOneUpKey;
                            currentOptions.playerOneLeftKey = options.playerOneLeftKey;
                            currentOptions.playerOneRightKey = options.playerOneRightKey;
                            currentOptions.playerOneDownKey = options.playerOneDownKey;
                        }
                        if (player == "two")
                        {

                            currentOptions.playerTwoUpKey = options.playerTwoUpKey;
                            currentOptions.playerTwoLeftKey = options.playerTwoLeftKey;
                            currentOptions.playerTwoRightKey = options.playerTwoRightKey;
                            currentOptions.playerTwoDownKey = options.playerTwoDownKey;
                        }
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

        //Menu for selecting difficulty
        static bool DifficultySelect()
        {
            bool valid = true;
            do
            {
                Console.Clear();
                valid = true;
                DrawTitle((currentOptions.windowWidth / 2) - 27, 2);
                Console.SetCursorPosition(0, 12);

                //Menu
                Console.WriteLine("\t1:Easy\n\t2:Medium\n\t3:Hard\n\t4:Impossible\n\t5:Back");
                //Sets difficulty number depending on selection
                switch (Console.ReadKey(true).Key)
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

        //Menu to select amount of players
        static bool PlayerSelect()
        {
            bool valid = true;
            do
            {
                Console.Clear();
                valid = true;
                DrawTitle((currentOptions.windowWidth / 2) - 27, 2);
                Console.SetCursorPosition(0, 12);


                Console.WriteLine("\tHow many players? (max of 2):");

                //Menu to select amount of players
                switch (Console.ReadKey(true).Key)
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

        //Asks user if they want walls
        static bool WallToggle()
        {
            bool valid = true;
            do
            {
                Console.Clear();
                valid = true;
                DrawTitle((currentOptions.windowWidth / 2) - 27, 2);
                Console.SetCursorPosition(0, 12);

                Console.WriteLine("\tWalls?");
                Console.WriteLine("\t1:Yes\n\t2:No");
                //Menu that returns true if yes and false if no
                switch (Console.ReadKey(true).Key)
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

        //Serializes any object to a byte array,for networking
        public static byte[] SerializeToBytes<T>(T item)//T item can be any object
        {
            var formatter = new BinaryFormatter(); //Creates a Binaryformater 
            using (var stream = new MemoryStream())//Stream to contain serialized object
            {
                formatter.Serialize(stream, item);//Serializes the object onto the memory stream
                stream.Seek(0, SeekOrigin.Begin);//Sets postition in the stream to begining
                return stream.ToArray();//Returns serialized byte array
            }
        }


        //Deserialize a byte array,for networking
        public static object DeserializeFromBytes(byte[] bytes)
        {
            var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream(bytes))
            {
                return formatter.Deserialize(stream); //Returns the deserialized BinaryFormatter
            }
        }
        //Starts the serverThread
        static void CreateServer()
        {
            serverThread = new Thread(new ThreadStart(ServerStart));//Creates new thread
            serverThread.Start();//Starts thread

        }

        //Variables for network
        static bool connected = false;
        public static bool started = false;
        public static TcpListener serverSocket;

        static void ServerStart()
        {

            serverSocket = new TcpListener(8888); //Creates new TcpListener at port 8888
            TcpClient clientSocket = default(TcpClient); // Creates new TcpClient
            int counter = 0;//For numbering clients
            serverSocket.Start();//Begins the TcpListener
            started = true;//Server has started
            counter = 0;//Resests counter

            //Loop used to connect multiple clients, currently only one can connect
            while (!connected)
            {
                counter += 1;
                clientSocket = serverSocket.AcceptTcpClient();//Waits for connection with client
                Server client = new Server();//New server class
                client.startClient(clientSocket, counter);//Starts new client connection
                connected = true;

            }
            while (gameRunning)
            {
                Thread.Sleep(100);//Waits for game end
            }
            //Closes connections any abort thread on game end
            serverSocket.Stop();
            clientSocket.Close();
            Thread.CurrentThread.Abort();
        }



        //Method for all client logic
        static void ClientStart()
        {

            TcpClient clientSocket = new TcpClient();//New TcpClient
            bool valid = true;//Validation for checking connection timeout
            int count = 0;//Timeout count
            Console.WriteLine("\tPlease enter a ip address:");//Request ip
            string ipAddress = Console.ReadLine();//Takes ip input, no validation as connection will timeout if wrong
            Console.Write("\tConnecting");//Displayed until connection or timeout
            do
            {

                try
                {

                    valid = true;//Valid if connected
                    clientSocket.Connect(ipAddress, 8888);//Trys to connect to given ip on port 8888

                }
                catch
                {
                    valid = false;
                    count++;//Plus one to the count
                    Console.Write(".");//Adds a dot to the connecting
                    Thread.Sleep(1);
                }
            }
            while (count <= 5 && valid == false);//Waits for 5 loops
            if (count < 5)//Count must be <5 if connected
            {

                Initialize();//Initilize gameboard and input
                DrawGrid();//Draws inital grid


                while (clientSocket.Connected)//Only runs while socket connected
                {


                    try
                    {
                        NetworkStream serverStream = clientSocket.GetStream(); //Sets the serverStream to the client sockets NetworkStream
                        byte[] outStream = SerializeToBytes<ConsoleKeyInfo>(input);//Serializes the current key input to a byte array


                        serverStream.Write(outStream, 0, outStream.Length);//Writes the input byte array to the serverStream
                        serverStream.Flush();//Removes the current data from the stream

                        byte[] inStream = new byte[300000];//Creates new byte array for incoming byte array data
                        serverStream.Read(inStream, 0, 300000);//Reads the current data from the stream
                        previousGrid = Grid;//Sets previousGrid to current grid, for knowing which tiles have changed
                        Grid = (Tile[,])DeserializeFromBytes(inStream);//Deserializes the current incoming byte array into a Tile class array and sets the current grid to it

                        //For making sure all tiles on the grid are updated
                        for (int i = 0; i < Grid.GetLength(0); i++)
                        {
                            for (int j = 0; j < Grid.GetLength(1); j++)
                            {
                                Grid[i, j].didContainWall = previousGrid[i, j].containsWall;
                                Grid[i, j].didContainHead = previousGrid[i, j].containsHead;
                                Grid[i, j].didContainBody = previousGrid[i, j].containsBody;
                                Grid[i, j].didContainPickup = previousGrid[i, j].containsPickup;
                                Grid[i, j].Update();
                            }
                        }



                        DrawScore();//Draws the score onto the screen
                        UpdateGrid();//Updates the displayed grid
                    }
                    catch
                    {

                    }
                }
                InputThread.Abort();//Ends input thread on game over

                InputThread.Join();
                GameOver();//Calls game over logic
            }
            else
            {
                Console.WriteLine("Connection timed out");//If the count hits 5 the connection times out
                Console.ReadKey(true);

            }



        }



        //Menu for selecting server or client
        static void MultiplayerSelect()
        {
            Console.Clear();
            DrawTitle((currentOptions.windowWidth / 2) - 27, 2);
            Console.SetCursorPosition(0, 12);
            Console.WriteLine("\t1:Server\n\t2:Client\n\t3:Back");


            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.D1:
                    if (DifficultySelect())
                    {
                        numOfPlayers = 2;//Sets number of players on server to 2

                        CreateServer();//Starts the server
                        Initialize();
                        //Loops while server isn't connected
                        do
                        {
                            Console.Clear();
                            DrawTitle((currentOptions.windowWidth / 2) - 27, 2);
                            Console.SetCursorPosition(0, 12);


                            Console.WriteLine("Waiting for players...");
                            Thread.Sleep(100);

                            //Starts game if connected
                            if (connected)
                            {

                                DrawGrid(); //Draws inital grid
                                foreach (Thread playerthread in playerThreads)
                                {
                                    playerthread.Start();//Begins all the player threads
                                }

                                GameLoop();//Starts the game
                            }



                        }
                        while (!connected);

                    }
                    break;
                case ConsoleKey.D2:
                    numOfPlayers = 0;//Sets players to 0 for the client, client only sends input key, does no game logic
                    ClientStart();// begins client connection
                    break;
                case ConsoleKey.D3:
                    break;
            }
        }


        //Wipes the scores
        static bool ClearSelect()
        {
            bool valid;
            do
            {
                Console.Clear();
                valid = true;
                DrawTitle((currentOptions.windowWidth / 2) - 27, 2);
                Console.SetCursorPosition(0, 12);
                Console.WriteLine("\tClear scores for:\n\t1:Easy\n\t2:Medium\n\t3:Hard\n\t4:Impossible\n\t5:Back");


                //Sets scores to new list
                switch (Console.ReadKey(true).Key)
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

        //Menu for displaying highscores
        static bool DrawHighScores()
        {
            bool valid;
            do
            {
                Console.Clear();
                valid = true;
                DrawTitle((currentOptions.windowWidth / 2) - 27, 2);
                Console.SetCursorPosition(0, 12);
                Console.WriteLine("\tShow scores for:\n\t1:Easy\n\t2:Medium\n\t3:Hard\n\t4:Impossible\n\t5:Clear Scores\n\t6:Back");

                //Sorts all scores
                easyHighScores = SortHighScores(easyHighScores);

                normalHighScores = SortHighScores(normalHighScores);

                hardHighScores = SortHighScores(hardHighScores);

                impossibleHighScores = SortHighScores(impossibleHighScores);

                //Displays all scores
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.D1:

                        DisplayHighScores(easyHighScores, "Easy");
                        return true;
                    case ConsoleKey.D2:
                        DisplayHighScores(normalHighScores, "Normal");
                        return true;
                    case ConsoleKey.D3:
                        DisplayHighScores(hardHighScores, "Hard");
                        return true;
                    case ConsoleKey.D4:
                        DisplayHighScores(impossibleHighScores, "Impossible");
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

        //Main loop
        static void Main(string[] args)
        {

            //Creates folder for snake in %appdata% if it doesn't exist
            if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Snake\"))
            {
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Snake\");
            }
            //Loads all highscores and options from file
            easyHighScores = LoadHighScores("easy");

            normalHighScores = LoadHighScores("normal");

            hardHighScores = LoadHighScores("hard");

            impossibleHighScores = LoadHighScores("impossible");

            currentOptions = LoadOptions();
            while (running)//Loops the main menu until program close
            {
                connected = false;
                Console.Title = "Snake";//Sets window title
                Console.CursorVisible = false;//Removes cursor

                //Sets window dimensions from options
                Console.WindowWidth = currentOptions.windowWidth;
                Console.WindowHeight = currentOptions.windowHeight;
                //Draws title
                Console.Clear();
                DrawTitle((currentOptions.windowWidth / 2) - 27, 2);
                Console.SetCursorPosition(0, 12);
                //Main menu
                Console.WriteLine("\t1:Local Game\n\t2:Networked Game\n\t3:Scores\n\t4:Edit options\n\t5:Exit");
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.D1:
                        multiplayer = false;//Local game
                        if (DifficultySelect())//Sets difficulty
                        {
                            if (PlayerSelect())//Sets amount of players
                            {
                                //Begins game
                                Initialize();
                                GameLoop();
                            }
                        }
                        break;
                    case ConsoleKey.D2:
                        multiplayer = true;//Lan multiplayer

                        MultiplayerSelect();//Starts Lan multiplayer menus

                        break;
                    case ConsoleKey.D3:

                        while (DrawHighScores())//Highscores menu
                        {

                        }



                        break;
                    case ConsoleKey.D4:
                        while (EditKeyBindingsMenu())// Key Bindings Menu
                        {

                        }
                        break;
                    case ConsoleKey.D5:
                        running = false;//Ends program
                        break;
                }

            }

        }
    }
}

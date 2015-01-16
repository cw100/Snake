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
    class Server
    {
            TcpClient clientSocket;
            int clientNumber;

            public void startClient(TcpClient inClientSocket, int clientnumber)//Begins new client with arguements
            {
                this.clientSocket = inClientSocket;
                this.clientNumber = clientnumber;

                Thread clientThread = new Thread(sendData);
                clientThread.Start();//Begins client thread
            }

            private void sendData()
            {
                Byte[] sendBytes = null;//Blank byte array to be sent


                while (Program.gameRunning == true)//Only while game is running
                {


                    try
                    {
                        NetworkStream networkStream = clientSocket.GetStream();//Sets networkStream to current socket stream

                        byte[] bytesFrom = new byte[10000];//Byte array to hold incoming data
                        networkStream.Read(bytesFrom, 0, 10000);//Collects incoming data to byte array
                        Program.players[this.clientNumber].input = (ConsoleKeyInfo)Program.DeserializeFromBytes(bytesFrom);//Deserializes the data into Console key Info and sets the programs main input to this


                        sendBytes = Program.SerializeToBytes<Tile[,]>(Program.Grid);//Serializes grid

                        networkStream.Write(sendBytes, 0, sendBytes.Length);//Sends serialized grid to clients
                    }
                    catch 
                    {
                        
                    }
                    
                   
                    
                   
                }
                Program.started = false;//Ends the server
                clientSocket.Close();//Closes current connection
                
                Thread.CurrentThread.Abort();//Ends thread


            }
        }

    }



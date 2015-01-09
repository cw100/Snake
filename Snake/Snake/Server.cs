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
        ConsoleKeyInfo clientInput;
            TcpClient clientSocket;
            int clNo;

            public void startClient(TcpClient inClientSocket, int clineNo)
            {
                this.clientSocket = inClientSocket;
                this.clNo = clineNo;
                Thread ctThread = new Thread(sendData);
                ctThread.Start();
            }

            private void sendData()
            {
                int requestCount = 0;
                Player.Direction direction;
                Byte[] sendBytes = null;
                string rCount = null;
                requestCount = 0;


                while ((true))
                {
                    try
                    {

                        requestCount = requestCount + 1;
                        NetworkStream networkStream = clientSocket.GetStream();

                        byte[] bytesFrom = new byte[10000];
                        networkStream.Read(bytesFrom, 0, 10000);
                        Program.players[this.clNo].input = (ConsoleKeyInfo)Program.DeserializeFromBytes(bytesFrom);

                        rCount = Convert.ToString(requestCount);

                        MemoryStream stream = new MemoryStream();

                        sendBytes = Program.SerializeToBytes<Tile[,]>(Program.Grid);

                       networkStream.Write(sendBytes, 0, sendBytes.Length);
                        

                    }
                    catch 
                    {
                        Thread.CurrentThread.Abort();

                    }
                }

            }
        }

    }



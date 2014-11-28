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
        public void Start()
    {
                TcpListener serverSocket = new TcpListener(8888);
                TcpClient clientSocket = default(TcpClient);
                int counter = 0;

                
                serverSocket.Start();
                Console.WriteLine("Server Started");

                counter = 0;
                while (true)
                {
                    counter += 1;
                    clientSocket = serverSocket.AcceptTcpClient();
                    Console.WriteLine("Client No:" + Convert.ToString(counter) + " started");
                    handleClient client = new handleClient();
                    client.startClient(clientSocket, Convert.ToString(counter));
                }

                clientSocket.Close();
                serverSocket.Stop();
                Console.WriteLine("exit");
                Console.ReadLine();
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
        public class handleClient
        {

            TcpClient clientSocket;
            string clNo;


            public void startClient(TcpClient inClientSocket, string clineNo)
            {
                this.clientSocket = inClientSocket;
                this.clNo = clineNo;
                Thread ctThread = new Thread(sendData);
                ctThread.Start();
            }

            private void sendData()
            {
                int requestCount = 0;
                Tile[,] dataFromClient = null;
                Byte[] sendBytes = null;
                string rCount = null;
                requestCount = 0;





                while ((true))
                {
                    try
                    {

                        requestCount = requestCount + 1;
                        NetworkStream networkStream = clientSocket.GetStream();

                        byte[] bytesFrom = new byte[100000];
                        networkStream.Read(bytesFrom, 0, 100000);

                        dataFromClient = (Tile[,])DeserializeFromBytes(bytesFrom);
                        
                        rCount = Convert.ToString(requestCount);


                        MemoryStream stream = new MemoryStream();

                        sendBytes = SerializeToBytes<Tile[,]>(dataFromClient);
                        networkStream.Write(sendBytes, 0, sendBytes.Length);
                        networkStream.Flush();

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }

            }
        }

    }
}

using System;

using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace _2DMultiplayer.ServerModules {

    class Server {

        //---------------------------------------------//
        //----CLIENT THREAD----//
        //---------------------------------------------//

        static int CurrentClientID = -1;
        static bool[] AvaliablePlaces = new bool[10];
        static List<Socket> clients = new List<Socket>();

        static void ClientHandlerThread(Socket client) {
            int currentID = 0;

            for (int i = 0; i < AvaliablePlaces.Length; i++) {
                if (!AvaliablePlaces[i]) {
                    currentID = i+1;
                    AvaliablePlaces[i] = true;
                    break;
                }
            }
            if (currentID == 0) CloseClient(client);

            string IDString = "ID: "+(currentID-1).ToString();
            string startMessage = IDString+" ";

            byte[] messageBytes = Encoding.UTF8.GetBytes(startMessage);
            byte[] bytes = new byte[2048];

            client.Send(messageBytes);
            OnClientConnected(client, IDString);
            Task.Run(() => ReceiveAndSendMessages(client, IDString, currentID));
        }

        static void ReceiveAndSendMessages(Socket client, string IDString, int clientID) {
            IPEndPoint clientEndPoint = client.RemoteEndPoint as IPEndPoint;
            byte[] bytes = new byte[2048];

            while (client.Connected) {
                bytes = new byte[2048];
                int i = client.Receive(bytes);

                int index = 0;

                string command = Encoding.UTF8.GetString(bytes);
                Console.WriteLine(command);

                if (command.StartsWith("[POSITION]: ")) {
                    SendToAllClients(command, client, clientID);
                }

                foreach (byte b in bytes) {
                    if (b == ' ') index++;
                }
                if (index == 0) {
                    OnClientDisconnected(client, clientID, IDString);
                    break;
                }
            }
        }


        //---------------------------------------------//
        //----SERVER----//
        //---------------------------------------------//

        static IPAddress ip = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0];
        static Socket serverSocket;   

        static void Main(string[] args) {
            serverSocket = new Socket(AddressFamily.InterNetwork, 
                                      SocketType.Stream, 
                                      ProtocolType.Tcp);
            
            IPEndPoint iPEndPoint = new IPEndPoint(ip, 5050);
            serverSocket.Bind(iPEndPoint);
            serverSocket.Listen(5);

            ReceiveClients();
        }

        static void ReceiveClients() {
            while (true) {
                Socket client = serverSocket.Accept();
                CurrentClientID++;
                clients.Add(client);
                Task.Run(() => ClientHandlerThread(client));
            }
        }

        //---------------------------------------------//
        //----CLIENT EVENTS----//
        //---------------------------------------------//

        static void SendToAllClients(string command, Socket ignore, int id) {

            string formattedCommand = "[ID]: " + id + "[COMMAND_BUFFER]" + command;

            foreach(Socket clt in clients) {
                try {
                    if (clt != ignore) {
                        byte[] messageBytes = Encoding.UTF8.GetBytes(formattedCommand);
                        clt.Send(messageBytes);
                    }
                }
                catch(Exception e) {}
            }
        }

        static void CloseClient(Socket client) {
            client.Close();
        }

        static void OnClientConnected(Socket client, string IDString) {
            string ClientJoinedCommand = "[CLIENT_ON_JOINED]: " + IDString;

            foreach(Socket clt in clients) {
                try {
                    if (clt != client) {
                        byte[] messageBytes = Encoding.UTF8.GetBytes(ClientJoinedCommand);
                        clt.Send(messageBytes);
                    }
                }
                catch(Exception e) {}
            }
        }

        static void OnClientDisconnected(Socket client, int id, string IDString) {
            Console.WriteLine("client {0} disconnected from the server", id);

            clients.Remove(client);
            AvaliablePlaces[id-1] = false;
            CloseClient(client);

            string ClientJoinedCommand = "[CLIENT_ON_LEFT]: " + IDString;

            foreach(Socket clt in clients) {
                try {
                    if (clt != client) {
                        byte[] messageBytes = Encoding.UTF8.GetBytes(ClientJoinedCommand);
                        clt.Send(messageBytes);
                    }
                }
                catch(Exception e) {}
            }
        }
    }
}

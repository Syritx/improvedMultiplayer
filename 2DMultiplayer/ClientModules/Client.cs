using System;
using System.Collections.Generic;

using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;

using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Text;
using System.Threading;

namespace _2DMultiplayer.ClientModules {
    class Client {

        static IPAddress ip = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0];
        static TcpClient client;
        static NetworkStream networkStream;
        static string CommandBuffer = "[COMMAND_BUFFER]";
        static List<Vector2> clientPositions = new List<Vector2>();

        static void Main() {

            client = new TcpClient(ip.ToString(), 5050);
            networkStream = client.GetStream();

            GameWindowSettings gameWindowSettings = new GameWindowSettings();
            NativeWindowSettings nativeWindowSettings = new NativeWindowSettings() {
                Size = new Vector2i(1000, 720),
                Title = "Multiplayer",
                APIVersion = new Version(3, 2),
                Flags = ContextFlags.ForwardCompatible,
                Profile = ContextProfile.Core,
                StartVisible = true,
                StartFocused = true,
            };

            Task messageReceiver = Task.Factory.StartNew(ReceiveMessages);
            for (int i = 0; i < 10; i++) {
                clientPositions.Add(new Vector2(0,0));
            }

            new Game(gameWindowSettings, nativeWindowSettings);
            Task.WaitAll(messageReceiver);
        }

        static void ReceiveMessages() {

            while (true) {
                byte[] data = new byte[2048];

                int response = networkStream.Read(data, 0, data.Length);
                string responseData = Encoding.ASCII.GetString(data, 0, response);

                Console.WriteLine(responseData);
                // SPLITING COMMANDS
                try {
                    // CLIENT ID
                    string[] removeBufferedCommands = responseData.Split(CommandBuffer, StringSplitOptions.None);
                    
                    int playerID = Int32.Parse(removeBufferedCommands[0].Split("[ID]: ",StringSplitOptions.None)[1])-1;
                    Console.WriteLine(playerID);

                    // POSITION
                    string positionString = removeBufferedCommands[1].Split("[POSITION]: ",StringSplitOptions.None)[1];
                    string[] coords = positionString.Split(", ", StringSplitOptions.None);

                    float[] coordsInFloat = new float[coords.Length];
                    for (int i = 0; i < coords.Length; i++) {
                        coordsInFloat[i] = float.Parse(coords[i]);
                    }
                    clientPositions[playerID] = new Vector2(coordsInFloat[0], coordsInFloat[1]);
                    Game.LoadNPC(playerID, clientPositions[playerID]);
                    Console.WriteLine("everything works");
                }
                catch(Exception e) {}
            }
        }

        public static void GetCommand(string command) {
            SendMessage(command);
        }

        public static void SendMessage(string message) {
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            networkStream.Write(bytes, 0, bytes.Length);
            networkStream.Flush();
        }

        ~Client()
        {
            networkStream.Close();
            client.Close();
        }
    }
}
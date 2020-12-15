using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;


// 2 clients to play the game
namespace Algebra_MP_ClientServerRPC_Server
{
    class Program
    {


        static void Main(string[] args)
        {
            WebSocketServer server = new WebSocketServer(8080);
            // localhost:8080/Game
            server.AddWebSocketService<ServerBehaviour>("/Game");
            server.Start();
            while (true) { }
        }

        class ServerBehaviour : WebSocketBehavior
        {
            static string[] players = new string[2];
            static int currentPlayerIndex; // has default value zero

            protected override void OnOpen()
            {
                Console.WriteLine("Joined: " + ID);
                base.OnOpen();
            }

            protected override void OnMessage(MessageEventArgs e)
            {

                (var messageType, string data) = parsePacket(e.Data);

                Console.WriteLine(messageType);

                if (messageType == MessageType.PlayerJoin)
                {
                    // ...
                    players[currentPlayerIndex++] = ID;
                    if (currentPlayerIndex == 2) // meaning that if two players joined
                    {
                        Console.WriteLine("START GAME");
                        foreach (var player in players)
                        {
                            Sessions.SendTo("1", player);
                        }
                    }
                }
                //base.OnMessage(e);
            }

            (MessageType, string data) parsePacket(string packet)
            {
                return ((MessageType)int.Parse(packet[0].ToString()), packet.Substring(1));
            }

            // string message
            // on string message
            // 1. char = MessageType
            // rest (2-n). char = data
            enum MessageType
            {
                PlayerJoin, // client -> server = 0
                StartTheGame, // server -> client = 1
                GeneratedNumber,// server -> client

            }
        }
    }


}

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
            static Random random = new Random();
            static int generatedNumber;
            static DateTime[] lastSentNumber = new DateTime[2];

            static string[] players = new string[2];
            static int currentPlayerIndex; // has default value zero

            protected override void OnOpen()
            {
                Console.WriteLine("Joined: " + ID);
                base.OnOpen();
            }

            protected override void OnMessage(MessageEventArgs e)
            {
                // 1. deserialize data
                (var messageType, string data) = parsePacket(e.Data);

                Console.WriteLine(messageType);

                if (messageType == MessageType.PlayerJoin)
                {
                    lastSentNumber[currentPlayerIndex] = DateTime.MinValue;
                    players[currentPlayerIndex] = ID;
                    currentPlayerIndex++;

                    if (currentPlayerIndex == 2) // meaning that if two players joined
                    {
                        generatedNumber = random.Next(100);

                        Console.WriteLine("START GAME");
                        foreach (var player in players)
                        {
                            Sessions.SendTo(getMessageType(MessageType.StartTheGame), player);
                        }
                    }
                }

                if (messageType == MessageType.GuessingNumber)
                {
                    int playerIndex = getIndexOfPlayer(ID);
                    // 1. get my datetime
                    var lastSent = lastSentNumber[playerIndex];

                    bool cheated = false;
                    if (lastSent != DateTime.MinValue)
                    {
                        if ((DateTime.Now - lastSent).TotalSeconds < 1)
                        {
                            cheated = true;
                            // player is cheating!
                            Console.WriteLine("Player: " + ID + " is cheating!");
                            playerWon(players[getIndexOfEnemy(ID)]);
                        }
                    }

                        lastSentNumber[playerIndex] = DateTime.Now;

                    if (!cheated)
                    {
                        int number = int.Parse(data);

                        if (number == generatedNumber)
                        {
                            playerWon(ID);
                        }
                        else if (number > generatedNumber)
                        {
                            Sessions.SendTo(getMessageType(MessageType.GoSmaller), ID);
                        }
                        else if (number < generatedNumber)
                        {
                            Sessions.SendTo(getMessageType(MessageType.GoSmaller), ID);
                        }
                    }
                }
                //base.OnMessage(e);
            }

            int getIndexOfPlayer(string id)
            {
                for (int i = 0; i < players.Length; i++)
                {
                    if (id == players[i]) return i;
                }
                return -1;
            }

            int getIndexOfEnemy(string id)
            {
                for (int i = 0; i < players.Length; i++)
                {
                    if (id != players[i]) return i;
                }
                return -1;
            }

            void playerWon(string id)
            {
                for (int i = 0; i < players.Length; i++)
                {
                    if (players[i] == id)
                    {
                        // win
                        Sessions.SendTo(getMessageType(MessageType.PlayerWon), players[i]);
                    }
                    else
                    {
                        // loss
                        Sessions.SendTo(getMessageType(MessageType.PlayerLost), players[i]);
                    }
                }
            }

            (MessageType, string data) parsePacket(string packet)
            {
                MessageType messageType = (MessageType)int.Parse(packet[0].ToString());
                string data = packet.Substring(1);
                return (messageType, data);
            }

            public string getMessageType(MessageType messageType)
            {
                return ((int)messageType).ToString();
            }

            // string message
            // on string message
            // 1. char = MessageType
            // rest (2-n). char = data
           public enum MessageType
            {
                PlayerJoin,     // client -> server = 0
                StartTheGame,   // server -> client = 1
                GuessingNumber ,// client -> server = 2
                PlayerWon,      // server -> client = 3
                PlayerLost,     // server -> client = 4
                GoBigger,       // server -> client = 5
                GoSmaller       // server -> client = 6
            }
        }
    }


}

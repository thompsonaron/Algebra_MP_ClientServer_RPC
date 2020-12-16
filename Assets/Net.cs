using System.Collections.Generic;
using WebSocketSharp;

public static class Net 
{
    public static WebSocket client;

    public static List<NetPacket> receiving;

    public static void joinServer()
    {
        client = new WebSocket("ws://localhost:8080/Game");
        client.OnMessage += Client_OnMessage;
        client.OnOpen += Client_OnOpen;
        client.Connect();
    }

    public static void sendPacket(NetPacket packet)
    {
        // better way: using a seriallizator
        UnityEngine.Debug.Log(packet.messageType + packet.data);
        client.Send((int)packet.messageType + packet.data);
    }

    public static void guessTheNumber(int number)
    {

    }

    private static void Client_OnOpen(object sender, System.EventArgs e)
    {
        UnityEngine.Debug.Log("onopen");
        client.Send("0");
    }

    private static void Client_OnMessage(object sender, MessageEventArgs e)
    {
        NetPacket packet = parsePacket(e.Data);

        UnityEngine.Debug.Log(packet.messageType);

        lock (receiving)
        {
            receiving.Add(packet);
        }
        
        //if (message.messageType == MessageType.StartTheGame)
        //{
        //    SceneManager.LoadScene("Game");
        //}
    }

    public static List<NetPacket> doUpdate()
    {
        List<NetPacket> output = new List<NetPacket>(receiving);
        receiving.Clear();

        return output;
    }

    static NetPacket parsePacket(string data)
    {
        var packet = new NetPacket();
        packet.messageType = (MessageType)int.Parse(data[0].ToString());
        packet.data = data.Substring(1);
        return packet;
    }
}
// string message
// on string message
// 1. char = MessageType
// rest (2-n). char = data
public enum MessageType
{
    PlayerJoin,     // client -> server = 0
    StartTheGame,   // server -> client = 1
    GuessingNumber, // client -> server = 2
    PlayerWon,      // server -> client = 3
    PlayerLost,     // server -> client = 4
    GoBigger,       // server -> client = 5
    GoSmaller       // server -> client = 6
}

public class NetPacket
{
    public MessageType messageType;
    public string data;
}
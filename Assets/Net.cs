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

    private static void Client_OnOpen(object sender, System.EventArgs e)
    {
        UnityEngine.Debug.Log("onopen");
        client.Send("0");
    }

    private static void Client_OnMessage(object sender, MessageEventArgs e)
    {
        NetPacket packet = parsePacket(e.Data);

        UnityEngine.Debug.Log(packet.messageType);

        lock (packet)
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
    PlayerJoin, // client -> server = 0
    StartTheGame, // server -> client = 1
    GeneratedNumber,// server -> client

}

public class NetPacket
{
    public MessageType messageType;
    public string data;
}
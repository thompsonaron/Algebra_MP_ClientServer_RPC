using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void join()
    {
        Net.joinServer();
    }

    private void Update()
    {
        var packets = Net.doUpdate();

        foreach (var packet in packets)
        {
            if (packet.messageType == MessageType.StartTheGame)
            {
                SceneManager.LoadScene("Game");
            }
        }
    }

    private void OnApplicationQuit()
    {
        Net.closeConnection();
    }
}

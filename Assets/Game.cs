using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public TMP_InputField numberInput;
    public TextMeshProUGUI statusLbl;
    public Button btnGuess;

    public void guess()
    {
        int myNumber = int.Parse(numberInput.text);
        // 2. send the number
        Net.sendPacket(new NetPacket() { messageType = MessageType.GuessingNumber, data = myNumber.ToString() });

    }

    // Update is called once per frame
    void Update()
    {
        var packets = Net.doUpdate();
        foreach (var packet in packets)
        {
            if (packet.messageType == MessageType.PlayerWon)
            {
                statusLbl.text = "I won!";
                btnGuess.gameObject.SetActive(false);
            }
            else if (packet.messageType == MessageType.PlayerLost)
            {
                statusLbl.text = "I lost";
                btnGuess.gameObject.SetActive(false);
            }
            else if (packet.messageType == MessageType.GoBigger)
            {
                statusLbl.text = "Go bigger";
            }
            else if (packet.messageType == MessageType.GoSmaller)
            {
                statusLbl.text = "Go smaller";
            }
        }
    }
}

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
    public Image cooldownImage;

    [HideInInspector] public float guessCooldown;

    public void guess()
    {
        if (guessCooldown <= 0)
        {

            int myNumber = int.Parse(numberInput.text);
            // 2. send the number
            Net.sendPacket(new NetPacket() { messageType = MessageType.GuessingNumber, data = myNumber.ToString() });
            guessCooldown = 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (guessCooldown > 0)
        {
            guessCooldown -= Time.deltaTime;
        }
        cooldownImage.fillAmount = guessCooldown;
        poll();
    }

    public void poll()
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

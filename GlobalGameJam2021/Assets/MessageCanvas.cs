using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageCanvas : MonoBehaviour
{
    [SerializeField] Text messageText;
    [SerializeField] Text coolDownText;
    float cooldownTimer;
    public void DisplayNotHereMsg()
    {
        messageText.text = "The criminal isn't here.\n Let's wrap up and try somewhere else.";
    }

    public void Update()
    {
        if(cooldownTimer <= 0)
        {
            coolDownText.text = "Ready!";
        }
        else
        {
            coolDownText.text = "Ready in: " + (cooldownTimer-= Time.deltaTime).ToString("F0") + "(s)";
        }

    }

    public void DisplayCooldown(float reportCooldown)
    {
        cooldownTimer = reportCooldown;
    }

}

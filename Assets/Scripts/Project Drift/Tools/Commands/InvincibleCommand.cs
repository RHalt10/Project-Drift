using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WSoft.Tools.Console;

public class Invincible : ConsoleCommand
{
    public Invincible()
    {
        commandWord = "invinsible";
    }

    public override List<string> GetValidArgs()
    {
        return new List<string>() {"on", "off" };
    }

    public override bool Process(string[] args)
    {
        if (args.Length != 1)
        {
            Debug.LogError("Incorrect Number of Args");
            return false;
        }

        if (args[0] == "on")
        {
            SetPlayerInvincible(true);
        }
        else if (args[0] == "off")
        {
            SetPlayerInvincible(false);
        }
        else
        {
            Debug.LogError("Invalid Arg 0");
            return false;
        }    

        return true;
    }

    void SetPlayerInvincible(bool toggle)
    {
        PlayerController.Instance.GetComponent<PlayerHealth>().isInvincible = toggle;
    }
}

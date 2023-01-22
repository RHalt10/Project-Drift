using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WSoft.Tools.Console;

public class HealthCommand : ConsoleCommand
{
    public HealthCommand()
    {
        commandWord = "health";
    }

    public override List<string> GetValidArgs()
    {
        return new List<string>() { "upgrade" };
    }

    public override bool Process(string[] args)
    {
        if(args.Length < 1 && args.Length > 2)
        {
            Debug.LogError("requires one or two arguments");
            return false;
        }
        // if arg0 is "upgrade", upgrade the health by the amount specified in arg1
        if (args[0] == "upgrade")
        {
            PlayerController.Instance.GetComponent<PlayerHealth>().UpgradeHealth(int.Parse(args[1]));
        }
        // else if integer, set the health of the player to be the value of arg0
        else if (int.TryParse(args[0], out _))
        {
            PlayerController.Instance.GetComponent<PlayerHealth>().SetHealth(int.Parse(args[0]));
        }
        else
        {
            Debug.LogError("invalid arg 0");
            return false;
        }
        return true;
    }
}
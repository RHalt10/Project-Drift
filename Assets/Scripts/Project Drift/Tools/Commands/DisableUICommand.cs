using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WSoft.Tools.Console;

public class DisableUI : ConsoleCommand
{
    public DisableUI()
    {
        commandWord = "disableui";
    }

    public override List<string> GetValidArgs()
    {
        return new List<string>() { "on", "off" };
    }

    public override bool Process(string[] args)
    {
        if (args.Length != 1)
        {
            Debug.LogError("Incorrect Number of Args");
            return false;
        }

        if (args[0] == "on")
            SetUIEnabled(true);
        else if (args[0] == "off")
            SetUIEnabled(false);
        else
        {
            Debug.LogError("Invalid Arg 0");
            return false;
        }

        return true;
    }

    void SetUIEnabled(bool enabled)
    {
        foreach(Canvas canvas in GameObject.FindObjectsOfType<Canvas>())
        {
            if (canvas.gameObject.name != "Developer Console")
            {
                canvas.gameObject.SetActive(enabled);
            }
        }
    }
}

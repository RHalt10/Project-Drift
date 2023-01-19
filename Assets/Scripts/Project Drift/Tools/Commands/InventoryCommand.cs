using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using WSoft.Tools.Console;

public class InventoryCommand : ConsoleCommand
{
    public InventoryCommand()
    {
        commandWord = "inventorycommand";
    }

    public override List<string> GetValidArgs()
    {
        return new List<string>();
    }

    public override bool Process(string[] args)
    {
        // returns false if number of arguments != 2
        if (args.Length != 2)
        {
            return false;
        }

        // update the respective values
        if (args[0] == "keys")
        {
            PlayerInventoryManager.Instance.SetValue(args[0], Int32.Parse(args[1]));
            return true;
        }
        else if (args[0] == "currency")
        {
            PlayerInventoryManager.Instance.SetValue(args[0], Int32.Parse(args[1]));
            return true;
        }

        // return false for all other inputs
        return false;
    }
}

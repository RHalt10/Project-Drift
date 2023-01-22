using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WSoft.Tools.Console;

public class RechargeGun : ConsoleCommand
{
    public RechargeGun()
    {
        commandWord = "rechargegun";
    }

    public override List<string> GetValidArgs()
    {
        return new List<string>();
    }

    public override bool Process(string[] args)
    {
        PlayerGun gunComponent = PlayerController.Instance.GetComponent<PlayerGun>();
        int ammoAmount = gunComponent.currentWeapon.maxAmmo;
        if(args.Length > 0)
        {
            int inputAmmo;
            if (int.TryParse(args[0], out inputAmmo))
            {
                ammoAmount = inputAmmo;
            } else
            {
                Debug.Log("Invalid ammo amount.");
                return false;
            }
        }
        //Set ammo
        gunComponent.currentAmmo = ammoAmount;

        return true;
    }
}

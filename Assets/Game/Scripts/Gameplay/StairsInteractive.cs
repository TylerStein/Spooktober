using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairsInteractive : Interactive
{
    public Stairs stairs;
    public string mountPrompt = "Press E to Mount";

    public override string GetPrompt(PlayerController player) {
        return mountPrompt;
    }

    public override bool ShowPrompt(PlayerController player) {
        return !player.controller.isOnStairs;
    }

    public override void Interact(PlayerController player) {
        if (!player.controller.isOnStairs) {
            player.controller.MountStairs(stairs);
            player.uiManager.ClearUIPrompt();
        }
    }
}

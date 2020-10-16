using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupInteractive : Interactive
{
    public string itemPrompt = "Press E To Pick Up";
    public Item item;

    public override string GetPrompt(PlayerController player) {
        return itemPrompt;
    }

    public override void Interact(PlayerController player) {
        player.GiveItem(item);
        Destroy(gameObject);
    }

    public override bool ShowPrompt(PlayerController player) {
        return true;
    }
}

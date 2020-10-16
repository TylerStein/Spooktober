using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteractive : Interactive
{
    public string requiredItemId = "thing";
    public string hasItemPrompt = "Press E to Unlock";
    public string missingItemPrompt = "The door is locked";

    public SelfFader unlockRoomFader;

    public override string GetPrompt(PlayerController player) {
        if (player.HasItemWithId(requiredItemId)) {
            return hasItemPrompt;
        } else {
            return missingItemPrompt;
        }
    }

    public override void Interact(PlayerController player) {
        if (player.HasItemWithId(requiredItemId)) {
            player.DisposeItemWithId(requiredItemId);
            if (unlockRoomFader) {
                unlockRoomFader.FadeToTransparent();
            }
            Destroy(gameObject);
        }
    }

    public override bool ShowPrompt(PlayerController player) {
        return true;
    }
}

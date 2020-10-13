using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideInteractive : Interactive
{
    public Color hiddenColor = new Color(1f, 1f, 1f, 0.2f);
    public string hidePrompt = "Press E to Hide";
    public string unHidePrompt = "Press E to Leave";

    private Color _normalColor = Color.white;

    public override string GetPrompt(PlayerController player) {
        if (player.isHidden) {
            return unHidePrompt;
        } else {
            return hidePrompt;
        }
    }

    public override bool ShowPrompt(PlayerController player) {
        return true;
    }

    public override void Interact(PlayerController player) {
        if (player.isHidden) {
            player.isHidden = false;
            player.p2dSubject.canBeVisible = true;
            player.controller.UnFreeze();
            player.sprite.color = _normalColor;
        } else {
            player.isHidden = true;
            player.p2dSubject.canBeVisible = false;
            player.controller.Freeze(true);
            player.playerLight.gameObject.SetActive(false);
            _normalColor = player.sprite.color;
            player.sprite.color = hiddenColor;
        }
    }
}

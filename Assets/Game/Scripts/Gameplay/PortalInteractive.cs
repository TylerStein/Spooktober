using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalInteractive : Interactive
{
    public string toSceneName;
    public string portalPrompt = "Press E to Enter";

    public override string GetPrompt(PlayerController player) {
        return portalPrompt;
    }

    public override void Interact(PlayerController player) {
        player.controller.Freeze(true);
        SceneManager.LoadScene(toSceneName);
    }

    public override bool ShowPrompt(PlayerController player) {
        return true;
    }
}

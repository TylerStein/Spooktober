using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public abstract class Interactive : MonoBehaviour
{
    public abstract void Interact(PlayerController player);
    public abstract string GetPrompt(PlayerController player);
    public abstract bool ShowPrompt(PlayerController player);
}

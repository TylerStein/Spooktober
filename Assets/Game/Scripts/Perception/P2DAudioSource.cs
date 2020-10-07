using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class P2DSound
{
    public Vector2 origin;
    public float radius;

    public static float GetAudioLevel(P2DSound sound, Vector2 ears) {
        float distance = (ears - sound.origin).magnitude;
        if (distance > sound.radius) return 0f;
        else return distance / sound.radius; // linear falloff for now
    }
}

public class P2DAudioSource : MonoBehaviour
{
    public P2DManager manager;

    // Start is called before the first frame update
    private void Start()
    {
        if (!manager) manager = FindObjectOfType<P2DManager>();
        if (!manager) throw new UnityException("P2DAudioSource requires a P2DManager");
    }

    public void Emit(float radius) {
        if (radius == 0f) return;
        manager.RegisterSound(new P2DSound { origin = transform.position, radius = radius });
    }
}

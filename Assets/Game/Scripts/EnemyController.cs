using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(P2DSubject))]
public class EnemyController : MonoBehaviour
{
    public P2DSubject perception;

    public bool heardNoise = false;
    public Vector2 lastNoiseLocation = Vector2.zero;
    public float lastNoiseIntensity = 0f;

    // Start is called before the first frame update
    void Start()
    {
        if (!perception) perception = GetComponent<P2DSubject>();
        perception.hearEvent.AddListener((P2DSound sound, float audioLevel) => {
            lastNoiseIntensity = audioLevel;
            lastNoiseLocation = sound.origin;
            heardNoise = true;
        });
    }

    private void OnDrawGizmos() {
        if (heardNoise) {
            Gizmos.color = Color.HSVToRGB(0.5f, lastNoiseIntensity, 1f);
            Gizmos.DrawLine(lastNoiseLocation, transform.position);
        }
    }
}

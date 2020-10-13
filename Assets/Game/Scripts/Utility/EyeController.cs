using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeController : MonoBehaviour
{
    public float eyeRadius = 0.5f;
    public Transform track;
    public Transform center;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (track && center) {
            Vector3 dir = (track.position - center.position).normalized;
            Vector3 position = center.position + (dir * eyeRadius);
            transform.position = position;
        }
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(center.position, eyeRadius);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolPoints : MonoBehaviour
{
    public List<Transform> points = new List<Transform>();

    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        for (int i = 0; i < points.Count; i++) {
            Gizmos.DrawWireSphere(points[i].transform.position, 0.5f);
            if (i + 1 < points.Count) {
                Gizmos.DrawLine(points[i].transform.position, points[i + 1].transform.position);
            }
        }
    }
}

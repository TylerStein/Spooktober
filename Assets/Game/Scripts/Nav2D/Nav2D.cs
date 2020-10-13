using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nav2D : MonoBehaviour
{
    public List<NavNode2D> nodes = new List<NavNode2D>();

    public void OnDrawGizmos() {
        Gizmos.color = Color.white;
        foreach (NavNode2D node in nodes) {
            Gizmos.DrawWireSphere(node.transform.position, 0.25f);
            foreach (NavNode2D connectedNode in node.connections) {
                Gizmos.DrawLine(node.transform.position, connectedNode.transform.position);

                Vector3 diff = connectedNode.transform.position - node.transform.position;
                float dist = diff.magnitude;

                Gizmos.DrawWireCube(node.transform.position + (diff.normalized * (dist * 0.15f)), Vector3.one * 0.15f);
            }
        }
    }

    public NavNode2D FindNearestNavNode(Vector3 point) {
        int index = 0;
        float magnitude = float.PositiveInfinity;

        for (int i = 0; i < nodes.Count; i++) {
            float dist = Vector3.Distance(nodes[i].position, point);
            if (dist < magnitude) {
                magnitude = dist;
                index = i;
            }
        }

        return nodes[index];
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavNode2D : MonoBehaviour
{
    public Vector3 position => transform.position;
    public List<NavNode2D> connections = new List<NavNode2D>();

    public void OnDrawGizmosSelected() {
        
    }
}

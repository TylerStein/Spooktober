using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleNav2D : MonoBehaviour
{
    public SimpleNav2DController navController;

    public bool hasTarget = false;
    public int targetLevelIndex = 0;
    public float targetLevelX = 0f;

    public void SetTarget(Vector3 point) {
        hasTarget = true;
        targetLevelIndex = navController.GetLevelIndexFromWorldY(point.y);
        targetLevelX = point.x;
    }
}

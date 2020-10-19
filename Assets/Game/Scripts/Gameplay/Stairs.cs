using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stairs : MonoBehaviour
{
    public Transform topPoint;
    public Transform bottomPoint;

    public Transform GetOppositePoint(Transform point) {
        if (point == topPoint) return bottomPoint;
        else return topPoint;
    }
}

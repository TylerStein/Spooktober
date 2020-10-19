using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NavLinkType {
    NORMAL = 0,
    STAIRS = 1,
}

[System.Serializable]
public struct NavArea2DLink
{
    public NavArea2D linkedArea;
    public NavLinkType linkType;

    public Stairs linkStairs;
    public Transform linkStairsPoint;
}

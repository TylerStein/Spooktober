using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum NavNodeType
{
    NORMAL = 0,
    STAIRS = 1,
}

[ExecuteInEditMode]
public class NavNode2D : MonoBehaviour
{
    public Vector3 position => transform.position;
    public List<NavNode2D> connections = new List<NavNode2D>();
    public NavNodeType navNodeType = NavNodeType.NORMAL;
    public Nav2D nav2d;
    public Stairs linkedStairs;

    public void Awake() {
        if (!nav2d) nav2d = FindObjectOfType<Nav2D>();
        if (!nav2d) {
            Debug.LogWarning("NavNode2D created with no Nav2D instance!", this);
        } else if (!nav2d.nodes.Contains(this)) {
            nav2d.nodes.Add(this);
        }
    }

    public void OnDestroy() {
        if (nav2d) {
            nav2d.nodes.Remove(this);
        }

        foreach (NavNode2D node in connections) {
            node.connections.Remove(this);
        }
    }

    public NavNodeData GetNavData() {
        return new NavNodeData(this);
    }
}

public class NavNodeData
{
    public NavNode2D source;
    public NavNodeData parent;
    public Vector2 position => source.position;
    public NavNodeType navType => source.navNodeType;
    public float distanceToTarget;
    public float cost;
    public float weight;
    public float F {
        get {
            if (distanceToTarget != -1 && cost != -1) {
                return distanceToTarget + cost;
            } else {
                return -1;
            }
        }
    }

    public NavNodeData(NavNode2D source) {
        this.source = source;
        distanceToTarget = -1f;
        cost = 1f;
        weight = 1f;
    }

    public override int GetHashCode() {
        return position.GetHashCode();
    }

    public override bool Equals(object obj) {
        return obj.GetHashCode() == GetHashCode() || (obj is NavNodeData && (obj as NavNodeData).position == position);
    }
}

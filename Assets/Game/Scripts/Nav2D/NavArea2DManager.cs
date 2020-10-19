using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class NavArea2DNode
{
    public NavArea2D area;
    public NavArea2DNode parent;
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

    public List<NavArea2DNode> GetNeighbors() {
        return area.GetNeighbors().ConvertAll(navArea => new NavArea2DNode(navArea));
    }

    public float DistanceTo(NavArea2DNode node) {
        return area.DistanceTo(node.area);
    }

    public NavArea2DNode(NavArea2D area) {
        this.area = area;
        distanceToTarget = -1f;
        cost = 1f;
        weight = 1f;
    }

    public override int GetHashCode() {
        return area.GetHashCode();
    }

    public override bool Equals(object obj) {
        return obj.GetHashCode() == GetHashCode() || (obj is NavArea2DNode && (obj as NavArea2DNode).area == area);
    }
}


public class NavArea2DManager : MonoBehaviour
{
    public List<NavArea2D> navAreas = new List<NavArea2D>();

    public NavArea2D GetContainingArea(Vector3 point) {
        foreach (NavArea2D area in navAreas) {
            if (area.bounds.Contains(point)) {
                return area;
            }
        }

        return null;
    }

    public NavArea2D GetClosestArea(Vector3 point) {
        float closestPointDistance = float.PositiveInfinity;
        NavArea2D closestNavArea = null;

        foreach (NavArea2D area in navAreas) {
            if (area.bounds.Contains(point)) {
                return area;
            } else {
                Vector3 nearest = area.bounds.ClosestPoint(point);
                float distance = Vector3.Distance(point, nearest);
                if (distance < closestPointDistance) {
                    closestNavArea = area;
                    closestPointDistance = distance;
                }
            }
        }

        return closestNavArea;
    }

    public Stack<NavArea2D> GetPath(NavArea2D start, NavArea2D goal) {
        int maxIterations = 2000;

        List<NavArea2DNode> openNodes = new List<NavArea2DNode>();
        List<NavArea2DNode> closedNodes = new List<NavArea2DNode>();

        NavArea2DNode startNode = new NavArea2DNode(start);
        NavArea2DNode goalNode = new NavArea2DNode(goal);
        NavArea2DNode current = startNode;

        openNodes.Add(current);
        while (maxIterations > 0 && openNodes.Count > 0 && (closedNodes.Contains(goalNode) == false)) {
            maxIterations--;
            current = openNodes[0];
            openNodes.Remove(current);
            closedNodes.Add(current);

            List<NavArea2DNode> neighbors = current.GetNeighbors();
            foreach (NavArea2DNode neighbor in neighbors) {
                if (closedNodes.Contains(neighbor) == false) {
                    if (openNodes.Contains(neighbor) == false) {
                        neighbor.parent = current;
                        // TODO: Improve this Distance function to consider more relevant values
                        neighbor.distanceToTarget = neighbor.DistanceTo(goalNode);
                        neighbor.cost = neighbor.weight + neighbor.parent.cost;
                        openNodes.Add(neighbor);
                        openNodes = openNodes.OrderBy(node => node.F).ToList();
                    }
                }
            }
        }

        if (closedNodes.Contains(goalNode) == false) {
            return null;
        }

        NavArea2DNode temp = current;
        if (closedNodes.Contains(current) == false) {
            return null;
        }

        Stack<NavArea2D> path = new Stack<NavArea2D>();
        do {
            path.Push(temp.area);
            temp = temp.parent;
        } while (temp != startNode && temp != null);

        return path;
    }
}

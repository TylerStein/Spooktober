using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CharacterNav2DController : MonoBehaviour
{
    public Nav2D nav2d;
    public Vector2 lastWorldTarget;
    public Vector2 lastSnapTarget;

    public Stack<NavNodeData> GetPathToTarget(Vector3 target) {
        lastWorldTarget = target;
        NavNode2D targetNode = nav2d.FindNearestNavNode(target);

        lastSnapTarget = targetNode.position;
        NavNode2D startNode = nav2d.FindNearestNavNode(transform.position);

        return AStar(startNode, targetNode);
    }

    private Stack<NavNodeData> AStar(NavNode2D startNode2D, NavNode2D goalNode2D) {
        int maxIterations = 2000;

        NavNodeData start = startNode2D.GetNavData();
        NavNodeData goal = goalNode2D.GetNavData();

        Stack<NavNodeData> path = new Stack<NavNodeData>();
        List<NavNodeData> openNodes = new List<NavNodeData>();
        List<NavNodeData> closedNodes = new List<NavNodeData>();
        NavNodeData current = start;

        openNodes.Add(current);
        while (maxIterations > 0 && openNodes.Count > 0 && (closedNodes.Contains(goal) == false)) {
            maxIterations--;
            current = openNodes[0];
            openNodes.Remove(current);
            closedNodes.Add(current);

            List<NavNodeData> neighbors = current.source.connections.ConvertAll((NavNode2D source) => source.GetNavData());
            foreach (NavNodeData neighbor in neighbors) {
                if (closedNodes.Contains(neighbor) == false) {
                    if (openNodes.Contains(neighbor) == false) {
                        neighbor.parent = current;
                        neighbor.distanceToTarget = Vector2.Distance(neighbor.position, current.position);
                        neighbor.cost = neighbor.weight + neighbor.parent.cost;
                        openNodes.Add(neighbor);
                        openNodes = openNodes.OrderBy(node => node.F).ToList<NavNodeData>();
                    }
                }
            }
        }

        if (closedNodes.Contains(goal) == false) {
            return null;
        }

        NavNodeData temp = current;
        if (closedNodes.Contains(current) == false) {
            return null;
        }

        do {
            path.Push(temp);
            temp = temp.parent;
        } while (temp != start && temp != null);

        return path;
    }

    private void OnDrawGiz() {
        
    }
}

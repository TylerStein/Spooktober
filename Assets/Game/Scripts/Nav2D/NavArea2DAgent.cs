using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterMovementController2D))]
public class NavArea2DAgent : MonoBehaviour
{
    public float closeOnTargetDistance = 0.15f;

    public NavArea2DManager navManager;
    public CharacterMovementController2D movementController;

    public NavArea2D currentArea = null;

    public Vector3 targetPosition = Vector3.zero;
    public NavArea2D targetArea = null;

    public Stack<NavArea2D> path = new Stack<NavArea2D>();
    public NavArea2DLink lastLink;

    public bool hasDestination = false;

    public void SetDestination(Vector3 point) {
        NavArea2D area = navManager.GetClosestArea(point);
        if (!area) {
            Debug.LogWarning($"Failed to find target area of point {point.ToString()} for agent", this);
            return;
        }

        if (!currentArea) currentArea = navManager.GetClosestArea(transform.position);
        path = navManager.GetPath(currentArea, area);
        if (path == null) {
            Debug.LogWarning($"Failed to find path to point {point.ToString()} for agent", this);
            return;
        }
        
        bool hasLink = currentArea.GetLinkToArea(path.Peek(), out lastLink);
        if (path.Count > 1 && !hasLink) {
            path.Clear();
            Debug.LogWarning($"Failed initialize first link to point {point.ToString()} for agent", this);
            hasDestination = false;
            return;
        }

        targetArea = area;
        targetPosition = point;
        hasDestination = true;
    }

    private void NavigateBasic() {
        // move along x
        float xDiff = (targetPosition - transform.position).normalized.x;
        float xDist = Mathf.Abs(xDiff);
        if (xDist < closeOnTargetDistance) {
            hasDestination = false;
            return;
        }

        movementController.Move(Vector2.right * xDiff);
    }

    private void NavigateAreas() {
        currentArea = navManager.GetContainingArea(transform.position);
        if (!movementController.isOnStairs && currentArea == targetArea) {
            // arrived in target area, basic nav takes over from here
            path.Pop();
        } else if (movementController.isOnStairs) {
            // move along stairs
            Vector3 moveTarget;
            if (lastLink.linkStairsPoint != null) {
                moveTarget = lastLink.linkStairs.GetOppositePoint(lastLink.linkStairsPoint).position;
            } else {
                moveTarget = movementController.GetNearestStairsMountPoint();
            }

            // move along stairs
            float xDiff = (moveTarget - transform.position).x;
            movementController.Move(Vector2.right * Mathf.Sign(xDiff));
        } else {
            NavArea2D nextArea = path.Peek();
            if (currentArea == nextArea) {
                // already in next area
                path.Pop();
            } else {
                NavArea2DLink link;
                bool hasLink = currentArea.GetLinkToArea(path.Peek(), out link);
                if (hasLink) {
                    lastLink = link;
                    if (link.linkType == NavLinkType.NORMAL) {

                        // move towards next area
                        float xDiff = (link.linkedArea.center - transform.position).x;
                        movementController.Move(Vector2.right * Mathf.Sign(xDiff));
                    } else if (link.linkType == NavLinkType.STAIRS) {
                        if (currentArea == nextArea) {
                            // already in next area
                            path.Pop();
                        }

                        // move to stairs to get to next area
                        float xDiff = (link.linkStairsPoint.position - transform.position).x;
                        float xDist = Mathf.Abs(xDiff);
                        movementController.Move(Vector2.right * Mathf.Sign(xDiff));

                        if (xDist < closeOnTargetDistance) {
                            movementController.MountStairs(link.linkStairs);
                        }
                    }
                } else {
                    path.Clear();
                    Debug.LogWarning("Path no longer valid, aborting");
                    return;
                }
            }
        }
    }

    public void MoveToDestination() {
        if (!hasDestination) {
            return;
        }

        if (path.Count > 0) NavigateAreas();
        else NavigateBasic();
    }
}

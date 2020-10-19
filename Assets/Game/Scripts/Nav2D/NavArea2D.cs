using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class NavArea2D : MonoBehaviour
{
    public List<NavArea2DLink> links;

    [SerializeField] private Bounds _bounds = new Bounds(Vector3.zero, Vector3.one);
    [SerializeField] private Vector3 _offset = Vector3.zero;

    public Bounds bounds => _bounds;
    public Vector3 center => bounds.center;
    public Vector3 offset {
        set {
            _offset = value;
            _bounds.center = transform.position + _offset;
        }
        get {
            return _offset;
        }
    }

    public float DistanceTo(NavArea2D other) {
        if (other.bounds.Intersects(_bounds)) {
            // touching
            return 0;
        }

        Vector3 closestPointToOther = _bounds.ClosestPoint(other.center);
        Vector3 otherClosestPointToThis = other.bounds.ClosestPoint(center);
        return Vector3.Distance(closestPointToOther, otherClosestPointToThis);
    }

    public List<NavArea2D> GetNeighbors() {
        return links.Select(link => link.linkedArea).ToList();
    }

    public bool IsInBounds(Vector2 point) {
        return bounds.Contains(point);
    }

    public bool GetLinkToArea(NavArea2D otherArea, out NavArea2DLink outLink) {
        foreach (NavArea2DLink link in links) {
            if (link.linkedArea == otherArea) {
                outLink = link;
                return true;
            }
        }

        outLink = new NavArea2DLink();
        return false;
    }

    private void Update() {
        _bounds.center = transform.position + _offset;
    }

    private void Awake() {
        NavArea2DManager navManager = FindObjectOfType<NavArea2DManager>();
        if (navManager != null) {
            if (navManager.navAreas.Contains(this) == false) {
                navManager.navAreas.Add(this);
            }
        }
    }

    private void OnDestroy() {
        NavArea2DManager navManager = FindObjectOfType<NavArea2DManager>();
        if (navManager != null) {
            navManager.navAreas.Remove(this);
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Vector3 bottomRight = bounds.min + (Vector3.right * bounds.size.x);
        Vector3 topLeft = bounds.min + (Vector3.up * bounds.size.y);

        Gizmos.DrawLine(bounds.min, bottomRight);
        Gizmos.DrawLine(bounds.min, topLeft);
        Gizmos.DrawLine(bottomRight, bounds.max);
        Gizmos.DrawLine(topLeft, bounds.max);

        foreach (NavArea2DLink link in links) {
            if (link.linkType == NavLinkType.NORMAL) {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(center, link.linkedArea.center);
            } else {
                if (link.linkStairsPoint != null && link.linkStairs != null) {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawWireSphere(link.linkStairsPoint.position, 0.5f);
                    Vector3 oppositePoint = link.linkStairs.GetOppositePoint(link.linkStairsPoint).position;
                    Gizmos.DrawLine(link.linkStairsPoint.position, oppositePoint);
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawWireSphere(oppositePoint, 0.5f);
                } else {
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(center, link.linkedArea.center);
                }
            }

        }
    }
}

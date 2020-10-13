using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public float defaultRadius = 45f;
    public float defaultDistance = 10f;
    public bool flipX = false;

    public Transform testTarget;

    public bool IsInFOV(Vector3 target) {
        Vector3 diff = target - transform.position;
        if (diff.magnitude > defaultDistance) return false;

        float angle = Vector2.SignedAngle(transform.right * (flipX ? -1f : 1f), diff.normalized);
        if (angle < defaultRadius * 0.5f && angle > -defaultRadius * 0.5f) {
            return true;
        }

        return false;
    }

    public bool IsInFOV(Vector3 target, float radius, float distance) {
        Vector3 diff = target - transform.position;
        if (diff.magnitude > distance) return false;

        float angle = Vector2.SignedAngle(transform.right * (flipX ? -1f : 1f), diff.normalized);
        if (angle < radius * 0.5f && angle > -radius * 0.5f) {
            return true;
        }

        return false;
    }

    public float GetOffsetRotation() {
        return Vector2.SignedAngle(Vector2.up, (Vector2)transform.right) * (flipX ? 1f : -1f);
    }

    public void OnDrawGizmos() {
        Gizmos.color = Color.green;

        float rotationOffset = GetOffsetRotation();
        float halfRadius = defaultRadius * 0.5f;

        float maxRad = rotationOffset + halfRadius;
        float minRad = rotationOffset - halfRadius;

        float xMax = Mathf.Sin(Mathf.Deg2Rad * maxRad);
        float yMax = Mathf.Cos(Mathf.Deg2Rad * maxRad);

        float xMin = Mathf.Sin(Mathf.Deg2Rad * minRad);
        float yMin = Mathf.Cos(Mathf.Deg2Rad * minRad);

        Vector3 dirMin = new Vector3(xMin, yMin).normalized;
        Vector3 dirMax = new Vector3(xMax, yMax).normalized;

        Gizmos.DrawRay(transform.position, dirMin * defaultDistance);
        Gizmos.DrawRay(transform.position, dirMax * defaultDistance);

        if (testTarget) {
            if (IsInFOV(testTarget.position)) {
                Gizmos.color = Color.green;
            } else {
                Gizmos.color = Color.red;
            }

            Gizmos.DrawLine(transform.position, testTarget.position);
            Gizmos.DrawWireSphere(testTarget.position, 0.5f);
        }
    }
}

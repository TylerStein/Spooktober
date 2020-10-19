using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public bool useBounds = false;
    public Bounds bounds = new Bounds(Vector3.zero, Vector3.one);

    public Transform targetTransform;
    public Vector3 targetOffset = new Vector3(0f, 0f, -10f);

    public float xTime = 0.6f;
    public float xMaxSpeed = 10f;

    public float yTime = 0.6f;
    public float yMaxSpeed = 8f;

    public float zTime = 0.6f;
    public float zMaxSpeed = 12f;

    [SerializeField] private float _xVelocity = 0f;
    [SerializeField] private float _yVelocity = 0f;
    [SerializeField] private float _zVelocity = 0f;

    // Update is called once per frame
    private void Start() {
        TeleportToTarget();
    }

    private void FixedUpdate() {
        Vector3 target = GetTarget();
        Vector3 targetPosition = target + targetOffset;

        float x = Mathf.SmoothDamp(transform.position.x, targetPosition.x, ref _xVelocity, xTime, xMaxSpeed, Time.fixedDeltaTime);
        float y = Mathf.SmoothDamp(transform.position.y, targetPosition.y, ref _yVelocity, yTime, yMaxSpeed, Time.fixedDeltaTime);
        float z = Mathf.SmoothDamp(transform.position.z, targetPosition.z, ref _zVelocity, zTime, zMaxSpeed, Time.fixedDeltaTime);

        Vector3 newPosition = new Vector3(x, y, z);
        if (useBounds) {
            if (bounds.Contains(newPosition) == false) {
                newPosition = bounds.ClosestPoint(newPosition);
            }
        }

        transform.position = newPosition;
    }

    public Vector3 GetTarget() {
        return targetTransform.position;
    }

    public void TeleportToTarget() {
        Vector3 targetPosition = GetTarget() + targetOffset;
        transform.position = targetPosition;
        ResetVelocity();
    }

    public void TeleportTo(Vector3 point) {
        Vector3 targetPosition = point + targetOffset;
        transform.position = targetPosition;
        ResetVelocity();
    }

    public void ResetVelocity() {
        _xVelocity = 0f;
        _yVelocity = 0f;
        _zVelocity = 0f;
    }

    public void OnDrawGizmosSelected() {
        if (useBounds) {
            Gizmos.color = Color.yellow;
            Vector3 bottomRight = bounds.min + (Vector3.right * bounds.size.x);
            Vector3 topLeft = bounds.min + (Vector3.up * bounds.size.y);
            Gizmos.DrawLine(bounds.min, bottomRight);
            Gizmos.DrawLine(bounds.min, topLeft);
            Gizmos.DrawLine(bottomRight, bounds.max);
            Gizmos.DrawLine(topLeft, bounds.max);

        }
    }
}

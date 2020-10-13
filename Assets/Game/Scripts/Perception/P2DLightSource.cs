using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class P2DLightSource : P2DVisibilityProvider
{
    public P2DManager manager = null;
    public new Light2D light = null;
    public Vector2 position { get => light.transform.position; }
    public bool drawLightGizmos = false;

    public bool overrideRadii = false;
    public float overrideInnerRadius = 0f;
    public float overrideOuterRadius = 0f;

    public bool overrideAngles = false;
    public float overrideInnerAngle = 0f;
    public float overrideOuterAngle = 0f;

    public float InnerRadius { get => overrideRadii ? overrideInnerRadius : light.pointLightInnerRadius; }
    public float OuterRadius { get => overrideRadii ? overrideOuterRadius : light.pointLightOuterRadius; }

    public float InnerAngle { get => overrideAngles ? overrideInnerAngle : light.pointLightInnerAngle; }
    public float OuterAngle { get => overrideAngles ? overrideOuterAngle : light.pointLightOuterAngle; }

    private void Start() {
        if (!light) light = FindObjectOfType<Light2D>();
        if (!manager) manager = FindObjectOfType<P2DManager>();
        if (!manager) throw new UnityException("P2DLightSource requires a P2DManager");
    }

    private void Update() {
        if (light.lightType != Light2D.LightType.Point) {
            Debug.LogWarning("P2DLightSource only supports Point lights");
            return;
        }
    }

    public override void UpdateSubjectVisibility(P2DSubject subject) {
        if (subject.canBeVisible) {
            float visibility = GetVisibilityOf(subject);
            if (subject.visibility < visibility) {
                subject.visibility = visibility;
            }
        }
    }

    public override float GetVisibilityOf(P2DSubject subject) {
        Vector2 toSubject = (subject.position - position);
        toSubject.x *= -1f;

        float magnitude = toSubject.magnitude;

        // distance intensity
        float radiusIntensity = 0f;
        if (magnitude <= InnerRadius) radiusIntensity = 1f;
        else if (magnitude <= OuterRadius) radiusIntensity = 0.5f;
        else return 0f; // Outside of radius

        float angle = Vector2.SignedAngle(transform.up, toSubject);
        float angleIntensity = 0f;

        // angle intensity
        if (angle <= InnerAngle * 0.5f && angle >= -InnerAngle * 0.5f) {
            angleIntensity = 1f;
        } else if (angle <= OuterAngle * 0.5f && angle >= -OuterAngle * 0.5f) {
            angleIntensity = 0.5f;
        } else {
            return 0f;
        }

        return Mathf.Clamp(angleIntensity * radiusIntensity * light.intensity, 0f, 1f);
    }

    public void OnDrawGizmos() {
        if (drawLightGizmos) {
            foreach (P2DSubject subject in manager.subjects) {
                float visibility = GetVisibilityOf(subject);
                if (visibility == 0f) continue;

                Gizmos.color = Color.HSVToRGB(0f, visibility, 1f);
                Gizmos.DrawLine(position, subject.position);
            }
        }

        Gizmos.color = Color.white;

        // Inner
        {
            float rotationOffset = transform.rotation.eulerAngles.z;
            float halfRadius = InnerAngle * 0.5f;

            float maxRad = rotationOffset + halfRadius;
            float minRad = rotationOffset - halfRadius;

            float xMax = Mathf.Sin(Mathf.Deg2Rad * maxRad);
            float yMax = Mathf.Cos(Mathf.Deg2Rad * maxRad);

            float xMin = Mathf.Sin(Mathf.Deg2Rad * minRad);
            float yMin = Mathf.Cos(Mathf.Deg2Rad * minRad);

            Vector3 dirMin = new Vector3(xMin, yMin).normalized;
            Vector3 dirMax = new Vector3(xMax, yMax).normalized;

            Gizmos.DrawRay(transform.position, dirMin * InnerRadius);
            Gizmos.DrawRay(transform.position, dirMax * InnerRadius);
        }

        // Outer
        {
            float rotationOffset = transform.rotation.eulerAngles.z;
            float halfRadius = OuterAngle * 0.5f;

            float maxRad = rotationOffset + halfRadius;
            float minRad = rotationOffset - halfRadius;

            float xMax = Mathf.Sin(Mathf.Deg2Rad * maxRad);
            float yMax = Mathf.Cos(Mathf.Deg2Rad * maxRad);

            float xMin = Mathf.Sin(Mathf.Deg2Rad * minRad);
            float yMin = Mathf.Cos(Mathf.Deg2Rad * minRad);

            Vector3 dirMin = new Vector3(xMin, yMin).normalized;
            Vector3 dirMax = new Vector3(xMax, yMax).normalized;

            Gizmos.DrawRay(transform.position, dirMin * OuterRadius);
            Gizmos.DrawRay(transform.position, dirMax * OuterRadius);
        }
    }
}

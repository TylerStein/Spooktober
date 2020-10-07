using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class P2DLightSource : MonoBehaviour
{
    public P2DManager manager = null;
    public new Light2D light = null;
    public Vector2 position { get => light.transform.position; }
    public bool drawLightGizmos = false;

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

    public void UpdateSubjectVisibility(P2DSubject subject) {
        if (subject.canBeVisible) {
            float visibility = GetVisibilityOf(subject);
            if (subject.visibility < visibility) {
                subject.visibility = visibility;
            }
        }
    }

    public float GetVisibilityOf(P2DSubject subject) {
        Vector2 toSubject = (subject.position - position);
        float magnitude = toSubject.magnitude;

        // distance intensity
        float radiusIntensity = 0f;
        if (magnitude <= light.pointLightInnerRadius) radiusIntensity = 1f;
        else if (magnitude <= light.pointLightOuterRadius) radiusIntensity = 0.5f;
        else return 0f; // Outside of radius

        float angle = Vector2.Angle(light.transform.up, toSubject) * 2;

        // angle intensity
        float angleIntensity = 0f;
        if (angle <= light.pointLightInnerAngle) angleIntensity = 1f;
        else if (angle <= light.pointLightOuterAngle) angleIntensity = 0.5f;
        else return 0f; // Outside of angle

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
    }
}

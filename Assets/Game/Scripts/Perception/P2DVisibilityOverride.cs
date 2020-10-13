using UnityEngine;

public class P2DVisibilityOverride : P2DVisibilityProvider
{
    public P2DManager manager = null;
    public Vector2 position { get => transform.position; }
    public bool drawLightGizmos = false;

    public float intensity = 1f;
    
    public float innerRadius = 6f;
    public float outerRadius = 8f;
    
    public float innerAngle = 45f;
    public float outerAngle = 90f;

    public float InnerRadius { get => innerRadius; }
    public float OuterRadius { get => outerRadius; }

    public float InnerAngle { get => innerAngle; }
    public float OuterAngle { get => outerAngle; }

    private void Start() {
        if (!manager) manager = FindObjectOfType<P2DManager>();
        if (!manager) throw new UnityException("P2DLightSource requires a P2DManager");
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

        return Mathf.Clamp(angleIntensity * radiusIntensity * intensity, 0f, 1f);
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

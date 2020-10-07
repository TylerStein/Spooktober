using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P2DManager : MonoBehaviour
{
    public List<P2DSubject> subjects = new List<P2DSubject>();
    public List<P2DLightSource> lightSources = new List<P2DLightSource>();

    public List<P2DSound> soundBuffer = new List<P2DSound>();

    public bool drawSoundGizmos = false;
    public float soundGizmoDequeueTime = 5.0f;
    private float gizmoTimer = 0f;
    private Queue<P2DSound> gizmoSoundBuffer = new Queue<P2DSound>();

    // Start is called before the first frame update
    void Start()
    {
        subjects = new List<P2DSubject>(FindObjectsOfType<P2DSubject>());
        lightSources = new List<P2DLightSource>(FindObjectsOfType<P2DLightSource>());
    }

    public void RegisterSound(P2DSound sound) {
        soundBuffer.Add(sound);
    }

    private void Update() {
        foreach (P2DSubject subject in subjects) {
            foreach (P2DSound sound in soundBuffer) {
                subject.ProcessSound(sound);
                if (drawSoundGizmos) {
                    gizmoSoundBuffer.Enqueue(sound);
                }
            }

            subject.visibility = 0f;
            foreach (P2DLightSource source in lightSources) {
                source.UpdateSubjectVisibility(subject);
            }
        }

        if (drawSoundGizmos && gizmoSoundBuffer.Count > 0) {
            gizmoTimer += Time.deltaTime;
            if (gizmoTimer >= soundGizmoDequeueTime) {
                gizmoTimer = 0f;
                gizmoSoundBuffer.Dequeue();
            }
        }

        soundBuffer.Clear();
    }

    private void OnDrawGizmos() {
        if (drawSoundGizmos) {
            foreach (P2DSound sound in gizmoSoundBuffer) {
                Gizmos.color = Color.HSVToRGB(0.5f, 0.5f, 1f);
                Gizmos.DrawWireSphere(sound.origin, sound.radius);
            }
        }
    }
}

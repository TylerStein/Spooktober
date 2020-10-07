using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class P2DHearEvent : UnityEvent<P2DSound, float> { }

public class P2DSubject : MonoBehaviour
{
    public Vector2 position { get => transform.position;   }
    public float visibility {
        get => canBeVisible ? _visibility : 0f;
        set => _visibility = Mathf.Clamp(value, 0f, 1f);
    }

    [SerializeField] public bool canBeVisible = true;
    [SerializeField] private float _visibility = 1f;

    [SerializeField] public bool canHear = true;
    [SerializeField] public float minAudioLevel = 0.15f;
    [SerializeField] public P2DHearEvent hearEvent;

    public void ProcessSound(P2DSound sound) {
        if (!canHear) return;

        float audioLevel = P2DSound.GetAudioLevel(sound, position);
        if (audioLevel > minAudioLevel) hearEvent.Invoke(sound, audioLevel);
    }
}

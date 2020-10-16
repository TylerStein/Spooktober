using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfFader : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public float fadeSpeed = 1f;
    public float targetAlpha = 1f;
    public bool destroyOnFade = false;

    private void Update() {
        Color currentColor = spriteRenderer.color;
        if (currentColor.a < targetAlpha) {
            currentColor.a += Time.deltaTime * fadeSpeed;

            if (currentColor.a >= targetAlpha) {
                // reached target
                if (destroyOnFade) Destroy(gameObject);
            }
        } else if (currentColor.a > targetAlpha) {
            currentColor.a -= Time.deltaTime * fadeSpeed;

            if (currentColor.a <= targetAlpha) {
                // reached target
                if (destroyOnFade) Destroy(gameObject);
            }
        }

        spriteRenderer.color = currentColor;
    }

    public void FadeToOpaque() {
        targetAlpha = 1f;
    }

    public void FadeToTransparent() {
        targetAlpha = 0f;
    }
}

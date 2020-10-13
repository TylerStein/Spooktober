using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class P2DVisibilityProvider : MonoBehaviour
{
    public abstract void UpdateSubjectVisibility(P2DSubject subject);
    public abstract float GetVisibilityOf(P2DSubject subject);
}

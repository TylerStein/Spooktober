using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Settings", menuName = "Custom/EnemyBehaviourSettings", order = 2)]
public class EnemyBehaviourSettings : ScriptableObject
{
    public float patrolPointDistance = 0.5f;
    public float investigateDistance = 0.5f;
    public float maxChaseDistance = 10f;
    public float autoSeePlayerDistance = 0.65f;

    public float maxInvestigateWaitTime = 5f;
    public float maxChaseWaitTime = 8f;
}

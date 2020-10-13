using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum EnemyState
{
    NONE,
    PATROL,
    INVESTIGATE,
    CHASE
}

[RequireComponent(typeof(P2DSubject))]
public class EnemyController : MonoBehaviour
{
    public PlayerController player;
    public CharacterController2D characterController;
    public EnemyBehaviourSettings behaviourSettings;
    public SpriteRenderer sprite;
    public SimpleNav2DController navController;

    public PatrolPoints patrolPoints;
    public FieldOfView fieldOfView;
    public P2DSubject perception;

    public EnemyState state {
        get {
            return _state;
        }
        set {
            _lastState = state;
            _state = value;
        }
    }

    [SerializeField] private EnemyState _lastState = EnemyState.NONE;
    [SerializeField] private EnemyState _state = EnemyState.PATROL;

    [SerializeField] private int _patrolPointIndex = 0;
    [SerializeField] private float _investigateWaitTimer = 0f;
    [SerializeField] private float _chaseWaitTimer = 0f;
    [SerializeField] private bool _isSuspicious = false;
    [SerializeField] private bool _canSeePlayer = false;
    [SerializeField] private bool _heardSound = false;

    //[SerializeField] private float _investigationPointX = 0f;
    //[SerializeField] private int _investigationPointLevel = 0;

    [SerializeField] private Vector2 _investigationPointWorld = Vector2.zero;
    [SerializeField] private P2DSound _lastSoundHeard = null;
    [SerializeField] private RaycastHit2D[] _lineOfSightHits = new RaycastHit2D[4];

    [SerializeField] private SimpleNav2DPoint _investigationPointNav;
    [SerializeField] private Stack<SimpleNav2DStep> _navSteps = new Stack<SimpleNav2DStep>();


    // Start is called before the first frame update
    void Start()
    {
        if (!perception) perception = GetComponent<P2DSubject>();
        perception.hearEvent.AddListener((P2DSound sound, float audioLevel) => {
            _lastSoundHeard = sound;
            _heardSound = true;
        });
    }

    private void OnDrawGizmos() {
        if (state == EnemyState.INVESTIGATE) {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, _investigationPointWorld);
            Gizmos.DrawWireSphere(_investigationPointWorld, 0.5f);
        } else if (state == EnemyState.CHASE) {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, _investigationPointWorld);
            Gizmos.DrawWireSphere(_investigationPointWorld, 0.5f);
        } else if (state == EnemyState.INVESTIGATE) {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, patrolPoints.points[_patrolPointIndex].position);
        }
    }

    private void Update() {
        PreStateUpdate();

        sprite.flipX = characterController.faceDirection == -1 ? true : false;
        fieldOfView.flipX = characterController.faceDirection == -1 ? true : false;

        if (state == EnemyState.INVESTIGATE) {
            UpdateState_MoveToTarget();
        } else if (_isSuspicious) {
            state = EnemyState.INVESTIGATE;
        }

        //if (state == EnemyState.PATROL) {
        //    UpdateState_Patrol();
        //} else if (state == EnemyState.INVESTIGATE) {
        //    UpdateState_Investigate(Time.deltaTime);
        //} else if (state == EnemyState.CHASE) {
        //    UpdateState_Chase(Time.deltaTime);
        //}

        PostStateUpdate();
    }

    private void PreStateUpdate() {
        // Check if player within auto-find distance
        if (!player.isHidden && Vector2.Distance(transform.position, player.transform.position) <= behaviourSettings.autoSeePlayerDistance) {
            _isSuspicious = true;
            _canSeePlayer = true;
            SetInvestigatePoint(player.transform.position);
            return;
        }

        // Else check if player visible
        if (player.visibility > 0f && fieldOfView.IsInFOV(player.transform.position)) {
            Vector3 losPoint = Vector3.zero;
            if (!player.isHidden && HasLineOfSightToPlayer(out losPoint)) {
                _isSuspicious = true;
                _canSeePlayer = true;
                SetInvestigatePoint(losPoint);
                return;
            }
        }

        // Else check if heard something
        if (_heardSound == true) {
            _isSuspicious = true;
            SetInvestigatePoint(_lastSoundHeard.origin);
            _lastSoundHeard = null;
            return;
        }

        _canSeePlayer = false;
        _isSuspicious = false;
    }

    private void PostStateUpdate() {
        _heardSound = false;
    }

    private void UpdateState_MoveToTarget() {
        if (_navSteps.Count == 0) {
            state = EnemyState.NONE;
            return;
        }

        SimpleNav2DStep activeStep = _navSteps.Peek();
        if (activeStep.climbDownStairs) {
            if (!characterController.isOnStairs) {
                // mount stairs
                characterController.MountStairs(activeStep.targetStairs);
            } else {
                // move down stairs
                int downDirection = characterController.GetStairsUpDirection() * -1;
                characterController.Move(Vector2.right * downDirection);

                if (characterController.GetStairsDistanceToEnd(downDirection) <= 0f || !characterController.isOnStairs) {
                    // bottom of stairs
                    _navSteps.Pop();
                }
            }
        } else if (activeStep.climbUpStairs) {
            if (!characterController.isOnStairs) {
                // mount stairs
                characterController.MountStairs(activeStep.targetStairs);
            } else {
                // move up stairs
                int upDirection = characterController.GetStairsUpDirection();
                characterController.Move(Vector2.right * upDirection);

                if (characterController.GetStairsDistanceToEnd(upDirection) <= 0f || !characterController.isOnStairs) {
                    // top of stairs
                    _navSteps.Pop();
                }
            }
        } else {
            // move towards this x
            float difference = activeStep.targetXPosition - transform.position.x;
            if (Mathf.Abs(difference) > 0.5f) {
                // move toward target
                characterController.Move(Vector2.right * Mathf.Sign(difference));
            } else {
                // at target
                _navSteps.Pop();
            }
        }
    }

    //private void UpdateState_Patrol() {
    //    if (_lastState != _state) {
    //        // Init Patrol
    //        _patrolPointIndex = FindNearestPatrolPointIndex();
    //        _lastState = _state;
    //    }

    //    float toPoint = MoveTowards(patrolPoints.points[_patrolPointIndex].position);
    //    if (Mathf.Abs(toPoint) <= behaviourSettings.patrolPointDistance) {
    //        _patrolPointIndex++;
    //        if (_patrolPointIndex >= patrolPoints.points.Count) {
    //            _patrolPointIndex = 0;
    //        }
    //    }

    //    if (_isSuspicious) {
    //        state = EnemyState.INVESTIGATE;
    //    }
    //}

    //private void UpdateState_Investigate(float deltaTime) {
    //    if (_lastState != _state) {
    //        // Init investigate
    //        _investigateWaitTimer = behaviourSettings.maxInvestigateWaitTime;
    //        _lastState = _state;
    //    }

    //    float xDiff = DistanceXTo(_investigationPointWorld);
    //    if (Mathf.Abs(xDiff) > behaviourSettings.investigateDistance) {
    //        MoveTowards(xDiff);
    //    } else {
    //        _investigateWaitTimer -= deltaTime;
    //    }

    //    if (_canSeePlayer) {
    //        state = EnemyState.CHASE;
    //    } else if (_isSuspicious) {
    //        _investigateWaitTimer = 0f;
    //    } else if (_investigateWaitTimer <= 0f) {
    //        state = EnemyState.PATROL;
    //    }
    //}

    //private void UpdateState_Chase(float deltaTime) {
    //    if (_lastState != _state) {
    //        // Init chase
    //        _chaseWaitTimer = behaviourSettings.maxChaseWaitTime;
    //        _lastState = _state;
    //    }

    //    if (_canSeePlayer) {
    //        _chaseWaitTimer = behaviourSettings.maxChaseWaitTime;
    //        float xDiff = DistanceXTo(_investigationPoint);
    //        if (Mathf.Abs(xDiff) > behaviourSettings.investigateDistance) {
    //            MoveTowards(xDiff);
    //        } else {
    //            // kill
    //        }
    //    } else {
    //        float xDiff = DistanceXTo(_investigationPoint);
    //        if (Mathf.Abs(xDiff) > behaviourSettings.investigateDistance) {
    //            MoveTowards(xDiff);
    //        } else {
    //            if (_chaseWaitTimer > 0f) {
    //                _chaseWaitTimer -= deltaTime;
    //                if (_chaseWaitTimer <= 0f) {
    //                    state = EnemyState.PATROL;
    //                }
    //            }
    //        }
    //    }
    //}

    private void SetInvestigatePoint(Vector2 point) {
        //_investigationPointLevel = navController.GetLevelIndexFromWorldY(point.y);
        //_investigationPointX = point.x;
        //_investigationPointWorld = navController.GetWorldPosition(_investigationPointLevel, _investigationPointX);

        SimpleNav2DPoint origin = navController.GetNavPoint(transform.position);
        SimpleNav2DPoint target = navController.GetNavPoint(point);
        _navSteps = navController.GetNavSteps(origin, target);
    }

    private float DistanceXTo(Vector3 target) {
        return (target - transform.position).x;
    }

    private float MoveTowards(Vector3 target) {
        float xDiff = DistanceXTo(target);
        return MoveTowards(xDiff);
    }

    private float MoveTowards(float distanceX) {
        if (distanceX != 0f) {
            float xSign = Mathf.Sign(distanceX);
            characterController.Move(new Vector3(xSign, 0f));
        }
        return distanceX;
    }

    private int FindNearestPatrolPointIndex() {
        float nearestPointDistance = float.PositiveInfinity;
        int nearestPointIndex = 0;

        for (int i = 0; i < patrolPoints.points.Count; i++) {
            float distance = Vector3.Distance(transform.position, patrolPoints.points[i].position);
            if (distance < nearestPointDistance) {
                nearestPointDistance = distance;
                nearestPointIndex = i;
            }
        }

        return nearestPointIndex;
    }

    private Vector3 GetNearestNavPoint(Vector3 target) {
        Vector3 adjustedTarget = target;
        adjustedTarget.y = transform.position.y;

        ContactFilter2D filter = new ContactFilter2D();
        filter.ClearLayerMask();
        
        int contacts = Physics2D.Linecast(transform.position, adjustedTarget, filter, _lineOfSightHits);
        if (contacts > 0) {
            Debug.DrawLine(transform.position, _lineOfSightHits[0].centroid, Color.red);
            Vector3 returnTarget = _lineOfSightHits[0].point;
            returnTarget.y = transform.position.y;
            return returnTarget;
        }

        return adjustedTarget;
    }

    private bool HasLineOfSightToPlayer(out Vector3 losPoint) {
        Vector3 dir = transform.position - player.transform.position;

        ContactFilter2D filter = new ContactFilter2D();
        filter.ClearLayerMask();

        int contacts = Physics2D.Linecast(transform.position, player.transform.position, filter, _lineOfSightHits);
        if (contacts > 0) {
            Debug.DrawLine(transform.position, _lineOfSightHits[0].point, Color.red);
            losPoint = _lineOfSightHits[0].point;
            losPoint.y = transform.position.y;
            return false;
        }

        losPoint = player.transform.position;
        losPoint.y = transform.position.y;
        return true;
    }
}

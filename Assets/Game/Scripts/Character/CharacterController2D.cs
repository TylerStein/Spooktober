using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MoveState
{
    IDLE,
    MOVING,
    STOPPING,
    BRAKING,
    JUMPING,
    FALLING,
}

public class CharacterController2D : MonoBehaviour
{
    public Bounds innerBounds = new Bounds(Vector3.zero, Vector3.one);
    public Bounds outerBounds = new Bounds(Vector3.zero, Vector3.one);

    public CharacterController2DSettings settings;
    public MoveState moveState = MoveState.IDLE;
    
    public float maxYFallVelocity = 16f;
    public float maxYJumpVelocity = 20f;

    public Vector2Int lastDirection = Vector2Int.zero;
    public Vector2 velocity = Vector2.zero;

    public Vector2 moveInput = Vector2.zero;
    public bool jumpInput = false;

    public int faceDirection = 1;

    public bool isGrounded = false;
    public bool isJumping = false;
    public float jumpTimer = 0f;
    public float fallYStart = 0f;

    public int horizontalRays = 3;
    public int verticalRays = 3;
    public bool drawRays = false;

    public bool isFrozen = false;

    public Stairs currentStairs = null;
    public bool isOnStairs = false;

    public float XMaxVelocity {
        get {
            if (isOnStairs) return settings.maxStairsVelocityX;
            else if (!isGrounded) return settings.maxAirVelocityX;
            else return settings.maxGroundVelocityX;
        }
    }
    public float XBraking{
        get {
            if (isOnStairs) return settings.stairsBraking;
            else if (!isGrounded) return settings.airBraking;
            else return settings.groundBraking;
        }
    }
    public float XFriction {
        get {
            if (isOnStairs) return settings.stairsFriction;
            else if (!isGrounded) return settings.airFriction;
            else return settings.groundFriction;
        }
    }
    public float XAcceleration {
        get {
            if (isOnStairs) return settings.stairsAcceleration;
            else if (!isGrounded) return settings.airAccelerationX;
            else return settings.groundAcceleration;
        }
    }

    public void Move(Vector2 inInput) {
        if (moveInput.x > 0f) faceDirection = 1;
        else if (moveInput.x < 0f) faceDirection = -1;
        moveInput = inInput;
    }

    public void Jump(bool state) {
        jumpInput = state;

        if (isGrounded && jumpInput) {
            isJumping = true;
            isGrounded = false;
            velocity = new Vector2(velocity.x, settings.jumpVelocity);
            jumpTimer = settings.maxJumpRiseTime;
        }
    }

    public void Freeze(bool clearVelocity) {
        if (clearVelocity) {
            velocity = Vector2.zero;
            isJumping = false;
        }

        isFrozen = true;
    }

    public void MountStairs(Stairs stairs) {
        isOnStairs = true;
        currentStairs = stairs;
        velocity = Vector2.zero;
        transform.position = GetNearestStairsMountPoint();
    }

    public void UnMountStairs(bool teleportToNearestPoint = true) {
        if (!currentStairs || !isOnStairs) return;
        if (teleportToNearestPoint) {
            transform.position = GetNearestStairsMountPoint();
        }

        isOnStairs = false;
        currentStairs = null;
    }

    public float GetStairsDistanceToEnd(float direction) {
        if (!currentStairs) return 0f;
        if (currentStairs.topPoint.position.x > currentStairs.bottomPoint.position.x) {
            // top is right
            if (direction > 0f) {
                // distance to top
                return Vector2.Distance(transform.position, currentStairs.topPoint.position);
            } else {
                // distance to bottom
                return Vector2.Distance(transform.position, currentStairs.bottomPoint.position);
            }
        } else {
            // bottom is right
            if (direction > 0f) {
                // distance to bottom
                return Vector2.Distance(transform.position, currentStairs.bottomPoint.position);
            } else {
                // distance to top
                return Vector2.Distance(transform.position, currentStairs.topPoint.position);
            }
        }
    }

    public int GetStairsUpDirection() {
        if (currentStairs.topPoint.position.x > currentStairs.bottomPoint.position.x) {
            // up is right
            return 1;
        } else {
            return -1;
        }
    }

    public Vector3 GetNearestStairsMountPoint() {
        Vector3 mountPoint = transform.position;

        if (currentStairs) {
            float distToTop = Vector2.Distance(transform.position, currentStairs.topPoint.position);
            float distToBottom = Vector2.Distance(transform.position, currentStairs.bottomPoint.position);

            if (distToTop < distToBottom) {
                // mount top
                mountPoint = currentStairs.topPoint.position;
            } else {
                // mount bottom
                mountPoint = currentStairs.bottomPoint.position;
            }
        }

        return mountPoint;
    }

    public void UnFreeze() {
        isFrozen = false;
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (isFrozen) return;

        if (isOnStairs) {
            UpdateStairs(Time.fixedDeltaTime);
        } else {
            UpdateNormal(Time.fixedDeltaTime);
        }
    }

    void UpdateNormal(float deltaTime) {
        DetectGround();

        float xVelocity = SolveXVelocity(velocity.x, Time.fixedDeltaTime);
        float yVelocity = SolveYVelocity(velocity.y, Time.fixedDeltaTime);
        velocity = new Vector2(xVelocity, yVelocity);

        if (isJumping) {
            jumpTimer = Approach(jumpTimer, 0f, Time.fixedDeltaTime);
        } else {
            jumpTimer = 0f;
        }

        Vector3 adjustedVelocity = AdjustVelocityForCollision(velocity * Time.fixedDeltaTime);
        if (adjustedVelocity.x == 0) velocity.x = 0;
        if (adjustedVelocity.y == 0) velocity.y = 0;

        transform.position += adjustedVelocity;
    }
    
    void UpdateStairs(float deltaTime) {
        isGrounded = true;
        isJumping = false;

        Vector2 nearestStairPoint = ClosestPointOnLine(currentStairs.bottomPoint.position, currentStairs.topPoint.position, transform.position);
        Vector2 topToBottomDir = (currentStairs.topPoint.position - currentStairs.bottomPoint.position).normalized;

        float xVelocity = SolveXVelocity(velocity.x, Time.fixedDeltaTime);

        Vector2 projectedPosition = nearestStairPoint + (topToBottomDir * xVelocity * Time.fixedDeltaTime);
        // stop progress past edges
        if (currentStairs.topPoint.position.x > currentStairs.bottomPoint.position.x) {
            // top is rightmost
            if (projectedPosition.x > currentStairs.topPoint.position.x) {
                projectedPosition = currentStairs.topPoint.position;
                xVelocity = 0f;
                UnMountStairs();
            } else if (projectedPosition.x < currentStairs.bottomPoint.position.x) {
                projectedPosition = currentStairs.bottomPoint.position;
                xVelocity = 0f;
                UnMountStairs();
            }
        } else {
            // bottom is rightmost
            if (projectedPosition.x > currentStairs.bottomPoint.position.x) {
                projectedPosition = currentStairs.bottomPoint.position;
                xVelocity = 0f;
                UnMountStairs();
            } else if (projectedPosition.x < currentStairs.topPoint.position.x) {
                projectedPosition = currentStairs.topPoint.position;
                xVelocity = 0f;
                UnMountStairs();
            }
        }

        velocity = new Vector2(xVelocity, 0);
        transform.position = projectedPosition;
    }

    float SolveXVelocity(float xVelocity, float deltaTime) {
        float targetXVelocity = moveInput.x * XMaxVelocity;

        if (Mathf.Sign(velocity.x) != faceDirection && velocity.x != 0) {
            // Braking
            xVelocity = Approach(xVelocity, targetXVelocity, XBraking * deltaTime);
            moveState = MoveState.BRAKING;
        } else if (Mathf.Abs(targetXVelocity) == 0f) {
            if (Mathf.Abs(velocity.x) > 0f) {
                // Friction
                xVelocity = Approach(xVelocity, targetXVelocity, XFriction * deltaTime);
                moveState = MoveState.STOPPING;
            } else {
                // Idle
                moveState = MoveState.IDLE;
            }
        } else {
            // Normal acceleration
            xVelocity = Approach(xVelocity, targetXVelocity, XAcceleration * deltaTime);
            moveState = MoveState.MOVING;
        }
        return xVelocity;
    }

    float SolveYVelocity(float yVelocity, float deltaTime) {
        if (isJumping) {
            if (yVelocity > 0) {
                // Approach jump peak
                moveState = MoveState.JUMPING;
                if (!jumpInput && jumpTimer > settings.jumpRiseTime) {
                    // must hold jump to get extended
                    jumpTimer = settings.jumpRiseTime;
                }

                if (jumpInput && jumpTimer > settings.jumpRiseTime) {
                    yVelocity = Approach(yVelocity, 0, settings.jumpVelocity / settings.maxJumpRiseTime * deltaTime);
                } else {
                    yVelocity = Approach(yVelocity, 0, settings.jumpVelocity / settings.jumpRiseTime * deltaTime);
                }

            } else {
                // Hit jump peak
                isJumping = false;
                fallYStart = transform.position.y;
            }
        } else if (isGrounded == false) {
            // Falling
            yVelocity = Approach(yVelocity, maxYFallVelocity * -1f, settings.fallAcceleration * deltaTime);
            moveState = MoveState.FALLING;
        }
        return yVelocity;
    }

    void DetectGround() {
        float verticalRaySpacing = innerBounds.size.y / (verticalRays - 1);
        Vector2 bottomLeft = transform.position + innerBounds.min;
        RaycastHit2D hit;

        bool newGrounded = false;

        // vertical
        {
            newGrounded = false;
            float rayLength = settings.skinWidth + settings.skinWidth;
            for (int i = 0; i < verticalRays; i++) {
                Vector2 rayOrigin = bottomLeft + (i * verticalRaySpacing * Vector2.right);
                Color rayColor = Color.grey;
                hit = RayCast(rayOrigin, Vector2.down, rayLength);
                if (hit.collider != null) {
                    rayColor = Color.yellow;
                    if (hit.distance <= settings.skinWidth * 1.15f) {
                        newGrounded = true;
                        rayColor = Color.green;
                    }
                }

                if (drawRays) Debug.DrawRay(rayOrigin, Vector2.down * 1, rayColor);
            }
        }

        if (newGrounded == false && isGrounded == true) {
            // started falling
            fallYStart = transform.position.y;
        }

        isGrounded = newGrounded;
    }

    Vector2 AdjustVelocityForCollision(Vector2 inVelocity) {
        float horizontalRaySpacing = innerBounds.size.x / (horizontalRays - 1);
        float verticalRaySpacing = innerBounds.size.y / (verticalRays - 1);

        Vector2 bottomLeft = transform.position + innerBounds.min;
        Vector2 topRight = transform.position + innerBounds.max;

        RaycastHit2D hit;

        // vertical
        {
            int vDir = (int)Mathf.Sign(inVelocity.y);
            float vRayDistance = Mathf.Abs(inVelocity.y);

            if (vRayDistance > 0f) {
                Vector2 origin = vDir == -1 ? bottomLeft : topRight;
                Vector2 rayDir = Vector2.up * vDir;
                float rayLength = vRayDistance + settings.skinWidth;
                for (int i = 0; i < verticalRays; i++) {
                    Vector2 rayOrigin = origin + (i * verticalRaySpacing * (vDir == -1 ? Vector2.right : Vector2.left));
                    Color rayColor = Color.grey;
                    hit = RayCast(rayOrigin, rayDir, rayLength);
                    if (hit.collider != null) {
                        inVelocity.y = (hit.distance - settings.skinWidth) * vDir;
                        rayLength = hit.distance;
                        rayColor = Color.yellow;

                        if (hit.distance <= settings.skinWidth * 1.15f) {
                            inVelocity.y = 0f;
                            if (rayDir.y >= 0f) {
                                isJumping = false;
                            }
                            rayColor = Color.green;
                        }
                    }

                    if (drawRays) Debug.DrawRay(rayOrigin, rayDir * 1, rayColor);
                }
            }
        }

        // horizontal
        {
            int hDir = (int)Mathf.Sign(inVelocity.x);
            float hRayDistance = Mathf.Abs(inVelocity.x);
            if (hRayDistance > 0f) {
                Vector2 origin = hDir == -1 ? bottomLeft : topRight;
                Vector2 rayDir = Vector2.right * hDir;
                float rayLength = hRayDistance + settings.skinWidth;
                for (int i = 0; i < horizontalRays; i++) {
                    Vector2 rayOrigin = origin + (i * horizontalRaySpacing * (hDir == -1 ? Vector2.up : Vector2.down));
                    Color rayColor = Color.grey;
                    hit = RayCast(rayOrigin, rayDir, rayLength);
                    if (hit.collider != null) {
                        inVelocity.x = (hit.distance - settings.skinWidth) * hDir;
                        rayLength = hit.distance;
                        rayColor = Color.yellow;

                        if (hit.distance <= settings.skinWidth * 1.15f) {
                            inVelocity.x = 0f;
                            rayColor = Color.green;
                        }
                    }

                    if (drawRays) Debug.DrawRay(rayOrigin, rayDir * 1, rayColor);
                }
            }
        }

        return inVelocity;
    }

    RaycastHit2D RayCast(Vector2 origin, Vector2 direction, float distance) {
        ContactFilter2D filter = new ContactFilter2D();
        filter.ClearLayerMask();
        filter.layerMask = settings.collisionLayerMask;
        return Physics2D.Raycast(origin, direction, distance, settings.collisionLayerMask);
    }

    float Approach(float current, float target, float delta) {
        float adjusted = current;
        if (current > target) {
            adjusted -= delta;
            if (adjusted < target) return target;
        } else if (current < target) {
            adjusted += delta;
            if (adjusted > target) return target;
        }
        return adjusted;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.cyan;
        Vector3 bl = transform.position + innerBounds.min;
        Vector3 br = transform.position + innerBounds.min + new Vector3(innerBounds.size.x, 0);
        Vector3 tr = transform.position + innerBounds.max;
        Vector3 tl = transform.position + innerBounds.max - new Vector3(innerBounds.size.x, 0);

        Gizmos.DrawLine(bl, br);
        Gizmos.DrawLine(br, tr);
        Gizmos.DrawLine(tr, tl);
        Gizmos.DrawLine(tl, bl);


        Gizmos.color = Color.blue;
        bl = transform.position + outerBounds.min;
        br = transform.position + outerBounds.min + new Vector3(outerBounds.size.x, 0);
        tr = transform.position + outerBounds.max;
        tl = transform.position + outerBounds.max - new Vector3(outerBounds.size.x, 0);

        Gizmos.DrawLine(bl, br);
        Gizmos.DrawLine(br, tr);
        Gizmos.DrawLine(tr, tl);
        Gizmos.DrawLine(tl, bl);

    }

    public static Vector2 ClosestPointOnLine(Vector2 a, Vector2 b, Vector2 p) {
        Vector2 ap = p - a;
        Vector2 ab = b - a;

        float abMag = ab.sqrMagnitude;
        float abaProduct = Vector2.Dot(ap, ab);
        float distance = abaProduct / abMag;

        if (distance < 0) return a;
        if (distance > 1) return b;
        else return a + ab * distance;
    }
}

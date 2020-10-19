using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterNavigationController2D : MonoBehaviour
{
    public SimpleNav2DController navController;
    public CharacterMovementController2D characterController;
    public Stack<SimpleNav2DStep> navSteps = new Stack<SimpleNav2DStep>();
    public SimpleNav2DPoint lastNavPoint;
    public SimpleNav2DPoint lastOriginPoint;


    public void SetNavTarget(Vector2 point) {
        lastOriginPoint = navController.GetNavPoint(transform.position);
        lastNavPoint = navController.GetNavPoint(point);
        navSteps = navController.GetNavSteps(lastOriginPoint, lastNavPoint);
    }

    // Update is called once per frame
    void Update() {
        if (navSteps.Count == 0) {
            return;
        }

        SimpleNav2DStep activeStep = navSteps.Peek();
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
                    navSteps.Pop();
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
                    navSteps.Pop();
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
                navSteps.Pop();
            }
        }
    }

    public void OnDrawGizmos() {
        if (navSteps.Count > 0) {
            SimpleNav2DStep step = navSteps.Peek();
            Vector3 targetPos = transform.position;
            targetPos.x = step.targetXPosition;
            Gizmos.DrawLine(transform.position, targetPos);
        }
    }
}

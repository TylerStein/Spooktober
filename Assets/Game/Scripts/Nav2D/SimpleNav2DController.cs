using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleNav2DStep
{
    public bool climbUpStairs = false;
    public bool climbDownStairs = false;

    public Stairs targetStairs = null;
    public float targetXPosition = 0f;
}

public class SimpleNav2DPoint
{
    public int levelIndex = 0;
    public float xPosition = 0f;
}

public class SimpleNav2DController : MonoBehaviour
{
    public List<Stairs> stairs = new List<Stairs>();
    public List<Transform> levels = new List<Transform>();

    private void Start() {
        levels.Sort((Transform a, Transform b) => {
            if (a.position.y > b.position.y) return 1;
            else if (a.position.y < b.position.y) return -1;
            else return 0;
        });
    }

    private void OnDrawGizmosSelected() {
        foreach (Transform level in levels) {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(level.position + Vector3.left * 10f, level.position + Vector3.right * 10f);
        }
    }

    public SimpleNav2DPoint GetNavPoint(Vector2 point) {
        int index = GetLevelIndexFromWorldY(point.y);
        return new SimpleNav2DPoint { levelIndex = index, xPosition = point.x };
    }

    public Stack<SimpleNav2DStep> GetNavSteps(SimpleNav2DPoint origin, SimpleNav2DPoint target) {
        // Create a set of steps to get from origin to target
        List<SimpleNav2DStep> steps = new List<SimpleNav2DStep>();

        int stepLevel = origin.levelIndex;
        float xPosition = origin.xPosition;

        while (stepLevel != target.levelIndex) {
            if (stepLevel > target.levelIndex) {
                // find stairs down
                int stairsDownIndex = GetNearestStairsIndexOnLevelByTop(stepLevel, xPosition);
                Stairs stairsDown = stairs[stairsDownIndex];

                // add step to move to stairs top
                steps.Add(new SimpleNav2DStep { targetXPosition = stairsDown.topPoint.position.x });

                // add step to move down stairs
                steps.Add(new SimpleNav2DStep { climbDownStairs = true, targetStairs = stairsDown });

                stepLevel--;
            } else {
                // find stairs up
                int stairsUpIndex = GetNearestStairsIndexOnLevelByBottom(stepLevel, xPosition);
                Stairs stairsUp = stairs[stairsUpIndex];

                // add step to move to stairs bottom
                steps.Add(new SimpleNav2DStep { targetXPosition = stairsUp.bottomPoint.position.x });

                // add step to move up stairs
                steps.Add(new SimpleNav2DStep { climbUpStairs = true, targetStairs = stairsUp });

                stepLevel++;
            }
        }

        steps.Add(new SimpleNav2DStep { targetXPosition = target.xPosition });
        steps.Reverse();
        return new Stack<SimpleNav2DStep>(steps);
    }

    public int GetLevelIndexFromWorldY(float yPosition) {
        // levels have been sorted from bottom to top
        // the first level with y <= yPosition is the one
        for (int i = levels.Count - 1; i >= 0; i--) {
            if (levels[i].position.y <= yPosition) {
                return i;
            }
        }

        return -1;
    }
    
    public Vector2 GetWorldPosition(int levelIndex, float xPosition) {
        return new Vector2(xPosition, levels[levelIndex].position.y);
    }

    public int GetNearestStairsIndexOnLevelByTop(int levelIndex, float xPosition) {
        float closest = float.PositiveInfinity;
        int index = -1;

        for (int i = 0; i < stairs.Count; i++) {
            int stairTopLevelIndex = GetLevelIndexFromWorldY(stairs[i].topPoint.position.y);
            if (stairTopLevelIndex != levelIndex) continue;

            float d = Mathf.Abs(xPosition - stairs[i].topPoint.position.x);
            if (d < closest) {
                index = i;
                closest = d;
            }
        }

        return index;
    }

    public int GetNearestStairsIndexOnLevelByBottom(int levelIndex, float xPosition) {
        float closest = float.PositiveInfinity;
        int index = -1;

        for (int i = 0; i < stairs.Count; i++) {
            int stairTopLevelIndex = GetLevelIndexFromWorldY(stairs[i].bottomPoint.position.y);
            if (stairTopLevelIndex != levelIndex) continue;

            float d = Mathf.Abs(xPosition - stairs[i].bottomPoint.position.x);
            if (d < closest) {
                index = i;
                closest = d;
            }
        }

        return index;
    }

    public int GetNearestStairsIndexByTop(Vector3 position) {
        int positionLevelIndex = GetLevelIndexFromWorldY(position.y);
        return GetNearestStairsIndexOnLevelByTop(positionLevelIndex, position.x);
    }

    public int GetNearestStairsIndexByBottom(Vector3 position) {
        int positionLevelIndex = GetLevelIndexFromWorldY(position.y);
        return GetNearestStairsIndexOnLevelByBottom(positionLevelIndex, position.x);
    }
}

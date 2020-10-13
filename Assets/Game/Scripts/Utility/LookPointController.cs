using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookPointController : MonoBehaviour
{
    public Transform player;
    public bool useMouse = true;
    public float maxDistance = 5f;

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPosition = player.transform.position;

        if (useMouse) {
            targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPosition.z = player.transform.position.z;
        } else {
            float xInput = Input.GetAxis("Horizontal");
            float yInput = Input.GetAxis("Vertical");
            targetPosition = player.transform.position + new Vector3(xInput * maxDistance, yInput * maxDistance);
        }


        Vector3 toTarget = targetPosition - player.transform.position;
        toTarget = Vector3.ClampMagnitude(toTarget, maxDistance);
        transform.position = player.transform.position + toTarget;
    }
}

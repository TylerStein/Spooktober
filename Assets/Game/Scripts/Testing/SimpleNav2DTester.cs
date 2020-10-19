using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleNav2DTester : MonoBehaviour
{
    public NavArea2DAgent navController;
    public CharacterMovementController2D moveController;

    public Camera mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        if (!mainCamera) mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1")) {
            Vector3 newTarget = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            newTarget.z = 0;

            navController.SetDestination(newTarget);
        }

        if (navController.hasDestination) {
            navController.MoveToDestination();
        }
    }
}

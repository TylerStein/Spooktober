using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public P2DAudioSource p2dAudioSource;
    public float moveSpeed = 2f;
    public float soundRadius = 2.5f;

    // Start is called before the first frame update
    void Start()
    {
        if (!p2dAudioSource) p2dAudioSource = GetComponent<P2DAudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            p2dAudioSource.Emit(soundRadius);
        }

        float h = Input.GetAxis("Horizontal");
        transform.position += Vector3.right * h * moveSpeed * Time.deltaTime;


        float v = Input.GetAxis("Vertical");
        transform.position += Vector3.up * v * moveSpeed * Time.deltaTime;
    }
}

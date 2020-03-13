using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    /// <summary>
    ///     A test script to control the camera to rotate around world origin with key input.
    ///     `speed`: rotate speed
    /// </summary>
    
    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        print(string.Format("Speed initialized: {0: 0.0}", speed));
    }

    // Update is called once per frame
    void Update()
    {
        //get the Input from Horizontal axis
        float horizontalInput = Input.GetAxis("Horizontal");
        //get the Input from Vertical axis
        float verticalInput = Input.GetAxis("Vertical");

        transform.Rotate(
            0,
            horizontalInput * speed * Time.deltaTime,
            0
        );
    }
}

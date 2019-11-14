using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    float horiztonalSpeed = 5f;
    [SerializeField]
    float verticalSpeed = 3f;

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKey(KeyCode.W))
        {
            transform.position += Vector3.forward * Time.deltaTime * horiztonalSpeed;
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position -= Vector3.forward * Time.deltaTime * horiztonalSpeed;
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += Vector3.right * Time.deltaTime * horiztonalSpeed;
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position -= Vector3.right * Time.deltaTime * horiztonalSpeed;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            transform.position -= Vector3.up * Time.deltaTime * verticalSpeed;
        }
        if (Input.GetKey(KeyCode.E))
        {
            Debug.Log("going down");
            transform.position += Vector3.up * Time.deltaTime * verticalSpeed;
        }
    }
}

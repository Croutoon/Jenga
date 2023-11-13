using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    Vector3 targetRot = new Vector3(0, 180, 0);
    public float speed = 1f;
    public float moveSpeed = 1f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(targetRot), speed * Time.deltaTime);
        transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, 0f, transform.position.z), moveSpeed * Time.deltaTime);

        if(Input.GetKeyDown(KeyCode.E))
        {
            targetRot = new Vector3 (0, targetRot.y + 90, 0);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            targetRot = new Vector3(0, targetRot.y + -90, 0);
        }
    }
}

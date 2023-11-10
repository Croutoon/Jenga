using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    Vector3 targetRot = Vector3.zero;
    public float speed = 1f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(targetRot), speed * Time.deltaTime);

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

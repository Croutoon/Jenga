using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{

    private bool touched = false;
    public bool highest = false;

    public Material mat1;
    public Material mat2;

    MeshRenderer mr;

    private void Start()
    {
        mr = transform.GetChild(0).GetComponent<MeshRenderer>();
        
    }
    private void Update()
    {
        if (highest)
        {
            mr.material = mat2;
        }
        else
        {
            mr.material = mat1;
        }
    }
    public void SetTouched(bool touched)
    {
        this.touched = touched;
    }

    public bool GetTouched()
    {
        return touched;
    }

}

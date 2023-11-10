using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    public GameObject blockPrefab;
    public Transform blockHolder;
    private Transform[] blocks = new Transform[48];
    Transform selected;
    Transform oldSelected;
    RaycastHit blockHit;
    RaycastHit mouseHit;

    public Transform mousePlane;
    Vector3 mousePlanePos;

    public LayerMask mousePlaneLM;

    float powerMultiplier = 1;

    float ySpawn = 2.5f;
    float ySpawnOffset = .5f;

    bool canReset = true;

    Vector3[] spawnPositions =
    {
        new Vector3 (0, 2.5f, -.26f),
        new Vector3 (0, 2.5f, 0),
        new Vector3 (0, 2.5f, .26f),
        new Vector3 (-.26f, 2.5f, 0),
        new Vector3 (0, 2.5f, 0),
        new Vector3 (.26f, 2.5f, 0),
    };

    int positionCount = 0;

    void Start()
    {
        for (int i = 0; i < blocks.Length; i++)
        {
            blocks[i] = blockHolder.GetChild(i);
        }
        ResetScene();
        Debug.Log(blocks.Length);
    }

    void Update()
    {
        Array.Sort(blocks, YPositionComparison);
        foreach (Transform block in blocks)
        {
            //block.GetComponent<Block>().highest = false;
        }
        //blocks[47].GetComponent<Block>().highest = true;
        ySpawn = blocks[47].position.y + ySpawnOffset;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out blockHit, 100))
        {
            if(Input.GetMouseButtonDown(0) && blockHit.transform.gameObject.tag == "block" && selected == null)
            {
                if (oldSelected != null)
                {
                    oldSelected.GetComponent<Block>().SetTouched(false);
                }
                selected = blockHit.transform;
                selected.GetComponent<Block>().SetTouched(true);

                mousePlane.position = new Vector3(0f, blockHit.transform.position.y, 0f);

                powerMultiplier = 1;
            }
        }
        if (Input.GetMouseButtonUp(0) && selected != null)
        {
            oldSelected = selected;
            selected = null;
            mousePlane.position = new Vector3(0f, 0f, 0f);
        }

        Physics.Raycast(ray, out mouseHit, 100, mousePlaneLM);
        mousePlanePos = mouseHit.point;

        if(powerMultiplier >= 0)
        {
            powerMultiplier -= .001f;
        }
        
    }

    void FixedUpdate()
    {
        if (selected != null)
        {
            float distance = Vector3.Distance(mousePlanePos, selected.transform.position);
            selected.GetComponent<Rigidbody>().AddForce((mousePlanePos - selected.position).normalized * (50f * (distance*1.5f) * powerMultiplier));
        }
    }

    void MoveBlock(Transform o)
    {
        if (positionCount > 2)
        {
            o.position = new Vector3(spawnPositions[positionCount].x, ySpawn, spawnPositions[positionCount].z);
            o.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
        }
        else
        {
            o.position = new Vector3(spawnPositions[positionCount].x, ySpawn, spawnPositions[positionCount].z);
            o.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }

        o.GetComponent<Rigidbody>().velocity = Vector3.zero;
        o.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        o.GetComponent<Block>().SetTouched(false);

        positionCount++;
        if(positionCount > 5) {
            positionCount = 0;
        }
    }

    IEnumerator resetBlocks()
    {
        canReset = false;
        for (int i = blocks.Length - 2; i >= 0; i--)
        {
            yield return new WaitForSeconds(.2f);
            MoveBlock(blocks[i]);
            canReset = true;
        }
        ySpawnOffset = .5f;
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject o = other.gameObject;
        if(o.tag == "block")
        {
            if (o.transform.parent.GetComponent<Block>().GetTouched())
            {
                MoveBlock(o.transform.parent);
            }
            else
            {
                //SceneManager.LoadScene(0);
                ResetScene();
            }
        }
    }

    private void ResetScene()
    {
        foreach (var b in blocks)
        {
            b.position = new Vector3(0, -1000f, 0f);
        }
        if (canReset)
        {
            positionCount = 0;
            ySpawn = 0.1f;
            ySpawnOffset = .15f;
            MoveBlock(blocks[47]);
            StartCoroutine("resetBlocks");
        }
    }

    private int YPositionComparison(Transform a, Transform b)
    {
        if (a == null) return (b == null) ? 0 : -1;
        if (b == null) return 1;

        var ya = a.position.y;
        var yb = b.position.y;
        return ya.CompareTo(yb);
    }

}

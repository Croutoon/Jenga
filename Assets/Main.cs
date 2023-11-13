using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    [Header("References:")]
    public GameObject blockPrefab;
    public Transform blockHolder;
    public CanvasManager canvasManager;

    [Space(10)]
    [Header("Mouse:")]
    public Transform mousePlane;
    public LayerMask mousePlaneLM;
    Vector3 mousePlanePos;

    [Space(10)]
    [Header("Power Settings:")]
    public float power = 50f;
    public float powerMultiplier = 1;

    Transform[] blocks = new Transform[48];
    Transform selected;
    //Transform oldSelected;

    RaycastHit blockHit;
    RaycastHit mouseHit;

    float ySpawn = 2.5f;
    float ySpawnOffset = .5f;

    bool canReset = true;
    bool canGrab = true;
    bool grabbing = true;

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

    int points = 0;

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

        ySpawn = blocks[47].position.y + ySpawnOffset;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out blockHit, 100))
        {
            if (Input.GetMouseButtonDown(0) && blockHit.transform.gameObject.tag == "block" && canGrab)
            {
                grabbing = true;
                if (selected == blockHit.transform)
                {
                    mousePlane.position = new Vector3(0f, blockHit.transform.position.y, 0f);
                    powerMultiplier = 1;
                }
                else if(selected == null)
                {
                    selected = blockHit.transform;
                    selected.GetComponent<Block>().SetTouched(true);

                    mousePlane.position = new Vector3(0f, blockHit.transform.position.y, 0f);

                    powerMultiplier = 1;
                }
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            grabbing = false;
            mousePlane.position = new Vector3(0f, 0f, 0f);
        }

        Physics.Raycast(ray, out mouseHit, 100, mousePlaneLM);
        mousePlanePos = mouseHit.point;

        if(powerMultiplier >= 0)
        {
            powerMultiplier -= .005f;
        }
        
    }

    void FixedUpdate()
    {
        if (selected != null && grabbing)
        {
            float distance = Vector3.Distance(mousePlanePos, selected.transform.position);
            selected.GetComponent<Rigidbody>().AddForce((mousePlanePos - selected.position).normalized * (power * (distance*1.5f) * powerMultiplier));
        }
    }

    void MoveBlock(Transform o, bool audio)
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

        if (audio)
            o.GetComponent<AudioSource>().time = 0.5f;
            o.GetComponent<AudioSource>().Play();
    }

    IEnumerator resetBlocks()
    {
        canGrab = false;
        canReset = false;
        for (int i = blocks.Length - 2; i >= 0; i--)
        {
            yield return new WaitForSeconds(.2f);
            MoveBlock(blocks[i], true);
        }
        canReset = true;
        canGrab = true;
        ySpawnOffset = .5f;
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject o = other.gameObject;
        if(o.tag == "block")
        {
            if (o.transform.parent.GetComponent<Block>().GetTouched())
            {
                MoveBlock(o.transform.parent, true);
                o.transform.parent.GetComponent<Block>().SetTouched(false);
                points += 1;
            }
            else
            {
                //SceneManager.LoadScene(0);
                points = 0;
                ResetScene();
            }
            selected = null;
        }
        canvasManager.UpdatePoints(points);
    }

    private void ResetScene()
    {
        foreach (var b in blocks)
        {
            b.position = new Vector3(0f, -1000f, 0f);
        }
        if (canReset)
        {
            positionCount = 0;
            ySpawn = 0.1f;
            ySpawnOffset = .15f;
            MoveBlock(blocks[47], true);
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

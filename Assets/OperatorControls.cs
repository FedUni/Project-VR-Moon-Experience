using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OperatorControls : MonoBehaviour
{

    public SceneTransitions SceneTransitions;
    public GameObject dropRigControls;
    private GameObject laserControls;
    private GameObject catapultFire;
    public GameObject objectToDrop;
    private Transform tform;

    private void Start()
    {
        laserControls = GameObject.Find("LaserExperiment");
        catapultFire = GameObject.Find("CatapultFireButton");
        tform = GameObject.Find("SpawnSpot").GetComponent<Transform>();
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            SceneTransitions.teleportViaWatchUI("MoonScene");
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            SceneTransitions.teleportViaWatchUI("MarsScene");
        }

        if (Input.GetKeyDown(KeyCode.LeftBracket))
        {
            SceneTransitions.teleportViaWatchUI("EarthScene");
        }

        if (Input.GetKeyDown(KeyCode.RightBracket))
        {
            SceneTransitions.teleportViaWatchUI("LaunchScene");
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            dropRigControls.GetComponent<DropRigGoTo25m>().setHeight();
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            dropRigControls.GetComponent<DropRigGoTo50m>().setHeight();
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            dropRigControls.GetComponent<DropRigGoTo75m>().setHeight();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            dropRigControls.GetComponent<DropRigSpawner>().spawnPressed();
        }

        if (Input.GetKeyDown(KeyCode.Semicolon))
        {
            dropRigControls.GetComponent<DropRigDrop>().dropPressed();
        }

        if (Input.GetKeyDown(KeyCode.Quote))
        {
            dropRigControls.GetComponent<DropRigReset>().resetPressed();
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            catapultFire.GetComponentInChildren<CatapultFire>().fireCatapult();
        }

        if (Input.GetKeyDown(KeyCode.Backslash))
        {
            if (laserControls != null)
            {
                laserControls.GetComponentInChildren<LaserAnimate>().laserAni();
            }
            
        }

        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            catapultFire.GetComponentInChildren<CatapultFire>().speed = 5f;
        }

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            catapultFire.GetComponentInChildren<CatapultFire>().speed = 10f;
        }

        if (Input.GetKeyDown(KeyCode.Minus))
        {
            catapultFire.GetComponentInChildren<CatapultFire>().speed = 30f;
        }

        if (Input.GetKeyDown(KeyCode.Equals))
        {
            catapultFire.GetComponentInChildren<CatapultFire>().speed = 40f;
        }

        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            catapultFire.GetComponentInChildren<CatapultFire>().launchAngle = 0.2f;
        }

        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            catapultFire.GetComponentInChildren<CatapultFire>().launchAngle = 0.5f;
        }

        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            catapultFire.GetComponentInChildren<CatapultFire>().launchAngle = 1f;
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            objectToDrop.transform.position = new Vector3(tform.position.x, tform.position.y, tform.position.z);  
            Instantiate(objectToDrop);
        }

    }
}

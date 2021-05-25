using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreadPlacer : MonoBehaviour
{
    public GameController GameController;
    public string noteTag;

    private Vector3 mOffset;
    private float mZCoord;

    public static bool IsPressing = false;
    private bool isPressing = false;
    private Transform RightHand;
    private Transform LeftHand;
    private Transform NoteSpawnPoint;

    void Awake()
    {
        noteTag = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.fffff");
        var camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        transform.GetChild(0).GetComponent<Canvas>().worldCamera = camera;

        RightHand = GameObject.FindGameObjectWithTag(nameof(RightHand)).transform;
        LeftHand = GameObject.FindGameObjectWithTag(nameof(LeftHand)).transform;
    }

    public void AddTarget()
    {
        if (GameController.TargetFrom == null)
        {
            GameController.TargetFrom = gameObject;
            GameController.TagFrom = noteTag;
        }
        else if (GameController.TargetTo == null)
        {
            GameController.TargetTo = gameObject;
            GameController.TagTo = noteTag;
        }
        else
        {
            Debug.LogError($"GameController targets are not empty");
            return;
        }
    }

    void Update()
    {
        if (IsPressing != isPressing)
        {
            return;
        }

        Ray ray = new Ray();
        if (OVRInput.Get(OVRInput.Button.PrimaryHandTrigger))
        {
            IsPressing = isPressing = true;
            // Right hand
            ray = new Ray(RightHand.position, RightHand.forward);
        }
        else if (OVRInput.Get(OVRInput.Button.SecondaryHandTrigger))
        {
            IsPressing = isPressing = true;
            // Left hand
            ray = new Ray(LeftHand.position, LeftHand.forward);
        }
        else
        {
            IsPressing = isPressing = false;
        }

        if (isPressing && Physics.Raycast(ray, out RaycastHit hit, 1000.0f))
        {
            if (hit.transform.tag == tag)
            {
                NoteSpawnPoint = GameObject.FindGameObjectWithTag(nameof(NoteSpawnPoint)).transform;
                hit.transform.position = new Vector3(NoteSpawnPoint.position.x, NoteSpawnPoint.position.y, transform.position.z);
            }
        }
    }

    void OnMouseDown()
    {
        mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;

        mOffset = gameObject.transform.position - GetMouseAsWorldPoint();
    }

    void OnMouseDrag()
    {
        transform.position = GetMouseAsWorldPoint() + mOffset;
    }

    private Vector3 GetMouseAsWorldPoint()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = mZCoord;

        return Camera.main.ScreenToWorldPoint(mousePoint);
    }
}

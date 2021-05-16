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

    void Awake()
    {
        noteTag = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.fffff");
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

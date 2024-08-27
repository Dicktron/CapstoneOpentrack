using System;
using UnityEngine;

public class AngleAdjust : MonoBehaviour
{
    public GameObject targetObject;

    public GameObject anchorObject;

    private Vector3 currentPosition;

    private Vector3 anchorPosition;

    private Vector3 targetDir;

    private float angle;

    private void angleCalculate(tarObject, ancObject)
    {
        currentPosition = tarObject.localPosition;
        anchorPosition = ancObject.localPosition;
        angle = Vector3.Angle(currentPosition, anchorPosition);
        tarObject.transform.rotation = Quaternion.AngleAxix(angle, Vector3.up);
    }

    void Update()
    {
        angleCalculate(targetObject, anchorObject);
    }
}
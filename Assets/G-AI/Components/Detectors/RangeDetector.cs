using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RangeDetector : UpdateDetector
{
    public LayerMask detectOnLayers;
    public float detectorRange = 10;
    public int maxDetectionNumber = 20;

    private Collider[] collidersBuffer;
    private readonly List<GameObject> newDetectionList = new List<GameObject>();

    private void Start()
    {
        collidersBuffer = new Collider[maxDetectionNumber];
    }

    public override void Detect()
    {
        var numberDetected =
            Physics.OverlapSphereNonAlloc(transform.position, detectorRange, collidersBuffer, detectOnLayers);
        
#if UNITY_EDITOR
        if (numberDetected == maxDetectionNumber)
        {
            Debug.LogWarning($"{name} Range Detector: max number detected, possible miss");
        }
#endif

        newDetectionList.Clear();
        for (var i = 0; i < numberDetected; i++)
        {
            var detectedGameObject = collidersBuffer[i].gameObject;
            if (IsDetectionValid(detectedGameObject))
            {
                newDetectionList.Add(detectedGameObject);
            }
        }

        UpdateDetectionList(newDetectionList);

        /*
        foreach (var oldDetectedObject in detectedObjects.Where(oldDetectedObject => !currentDetected.Contains(oldDetectedObject)))
        {
            detectedObjects.Remove(oldDetectedObject);
            onLostDetection.Invoke(oldDetectedObject, this);
        }

        foreach (var newDetectedObject in currentDetected.Where(newDetectedObject => !detectedObjects.Contains(newDetectedObject)))
        {
            detectedObjects.Add(newDetectedObject);
            onDetected.Invoke(newDetectedObject, this);
        }
    */
    }

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectorRange);
    }

#endif
}
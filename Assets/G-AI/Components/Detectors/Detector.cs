using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public enum DetectionMode
{
    Update,
    FixedUpdate,
    PeriodicUpdate,
    Manual
}

public abstract class Detector : MonoBehaviour
{
    public List<GameObject> ignoreList;
    public bool enableTagFilter;
    public string[] allowedTags;
    

    // Event is called for each GameObject at the time it is added to the detectors DetectedObjects list
    public Action<GameObject> onDetected;

    // Event is called for each GameObject at the time it is removed to the detectors DetectedObjects list
    public Action<GameObject> onLostDetection;

    public List<GameObject> detectedObjects;

    protected virtual void Awake()
    {
        if (ignoreList == null)
        {
            ignoreList = new List<GameObject>();
            detectedObjects = new List<GameObject>(20);
        }
    }

    public virtual void Detect(){}

    public int DetectedGameObjectLength()
    {
        return detectedObjects.Count;
    }

    public GameObject GetFirstDetectedObject()
    {
        return detectedObjects[0];
    }

    public GameObject GetRandomDetectedObject()
    {
        return detectedObjects[Random.Range(0, DetectedGameObjectLength())];
    }

    public List<GameObject> GetDetectedObjects()
    {
        return detectedObjects;
    }

    public List<GameObject> GetDetectedObjectsSorted()
    {
        return detectedObjects.OrderBy((d) => (d.transform.position - transform.position).sqrMagnitude).ToList();
    }

    public List<T> GetDetectedByComponent<T>()
    {
        var components = new List<T>();
        foreach (var detectedObject in detectedObjects)
        {
            var component = detectedObject.GetComponent<T>();
            if (component == null)
            {
                continue;
            }

            components.Add(component);
        }

        return components;
//            return detectedObjects.Select(detectedObject => detectedObject.GetComponent<T>()).Where(component => component != null).ToList();
    }

    protected bool IsDetectionValid(GameObject detectedGameObject)
    {
        var retVal = false;

        if (ignoreList.Contains(detectedGameObject))
        {
            return false;
        }

        if (enableTagFilter)
        {
            foreach (var allowedTag in allowedTags)
            {
                if (detectedGameObject.CompareTag(allowedTag))
                {
                    retVal = true;
                }
            }
        }
        else
        {
            retVal = true;
        }

        return retVal;
    }

    protected void UpdateDetectionList(List<GameObject> newDetectionList)
    {
        for (int i = detectedObjects.Count - 1, n = -1; i > n; i--)
        {
            var detectedObject = detectedObjects[i];
            if (!newDetectionList.Contains(detectedObject))
            {
                onLostDetection?.Invoke(detectedObject);
                detectedObjects.RemoveAt(i);
            }
        }

        foreach (var o in newDetectionList)
        {
            if (!detectedObjects.Contains(o))
            {
                onDetected?.Invoke(o);
                detectedObjects.Add(o);
            }
        }

        /* 
         var objectsToRemove = new List<GameObject>();
 //            var objectsToAdd = new List<GameObject>();
         foreach (var oldDetectedObject in detectedObjects.Where(oldDetectedObject =>
                      !newDetectionList.Contains(oldDetectedObject)))
         {
             objectsToRemove.Add(oldDetectedObject);
 //                detectedObjects.Remove(oldDetectedObject);
             onLostDetection.Invoke(oldDetectedObject);
         }
 
         foreach (var o in objectsToRemove)
         {
             detectedObjects.Remove(o);
         }
 
         foreach (var newDetectedObject in newDetectionList.Where(newDetectedObject =>
                      !detectedObjects.Contains(newDetectedObject)))
         {
             detectedObjects.Add(newDetectedObject);
             onDetected.Invoke(newDetectedObject);
         } */
    }
}

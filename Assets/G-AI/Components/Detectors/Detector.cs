using System;
using System.Collections.Generic;
using System.Linq;
using CogsAndGoggles.Library.Attributes;
using UnityEngine;
using UnityEngine.Events;

namespace Eadon.AI.Detectors
{
    public enum DetectionMode
    {
        Update,
        FixedUpdate,
        PeriodicUpdate,
        Manual
    }
    
    public abstract class Detector : MonoBehaviour
    {
        public DetectionMode detectionMode;
        public float checkInterval = 1f;

        public List<GameObject> ignoreList;
        public bool enableTagFilter;
        [TagSelector]
        public string[] allowedTags;
        
        [Serializable]
        public class DetectorEventHandler : UnityEvent<Detector> { }

        [Serializable]
        public class DetectionEventHandler : UnityEvent<GameObject> { }

        // Event is called for each GameObject at the time it is added to the detectors DetectedObjects list
        [SerializeField]
        public DetectionEventHandler onDetected;

        // Event is called for each GameObject at the time it is removed to the detectors DetectedObjects list
        [SerializeField]
        public DetectionEventHandler onLostDetection;
        
        protected readonly List<GameObject> detectedObjects = new List<GameObject>();

        private float _checkTimer;
        
        protected virtual void Awake()
        {
            if (ignoreList == null)
            {
                ignoreList = new List<GameObject>();
            }

            if (onDetected == null) 
            {
                onDetected = new DetectionEventHandler();
            }

            if (onLostDetection == null)
            {
                onLostDetection = new DetectionEventHandler();
            }
        }

        private void Update()
        {
            switch (detectionMode)
            {
                case DetectionMode.Update:
                    Detect();
                    break;
                case DetectionMode.PeriodicUpdate:
                    _checkTimer += Time.deltaTime;
                    if (_checkTimer >= checkInterval)
                    {
                        Detect();
                        _checkTimer = 0;
                    }
                    break;
            }
        }

        private void FixedUpdate()
        {
            if (detectionMode == DetectionMode.FixedUpdate)
            {
                Detect();
            }
        }

        public abstract void Detect();
        
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
            var objectsToRemove = new List<GameObject>();
            var objectsToAdd = new List<GameObject>();
            foreach (var oldDetectedObject in detectedObjects.Where(oldDetectedObject => !newDetectionList.Contains(oldDetectedObject)))
            {
                objectsToRemove.Add(oldDetectedObject);
//                detectedObjects.Remove(oldDetectedObject);
                onLostDetection.Invoke(oldDetectedObject);
            }

            foreach (var o in objectsToRemove)
            {
                detectedObjects.Remove(o);
            }

            foreach (var newDetectedObject in newDetectionList.Where(newDetectedObject => !detectedObjects.Contains(newDetectedObject)))
            {
                detectedObjects.Add(newDetectedObject);
                onDetected.Invoke(newDetectedObject);
            }
        }
    }
}

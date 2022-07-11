using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Eadon.AI.Detectors
{
    public class RangeDetector : Detector
    {
        public LayerMask detectOnLayers;
        public float detectorRange = 10;
        public int maxDetectionNumber = 20;
        
        private Collider[] _collidersBuffer;
        
        private void Start()
        {
            _collidersBuffer = new Collider[maxDetectionNumber];
        }

        public override void Detect()
        {
            var numberDetected = Physics.OverlapSphereNonAlloc(transform.position, detectorRange, _collidersBuffer, detectOnLayers);

            if (numberDetected == maxDetectionNumber)
            {
                Debug.LogWarning($"{name} Range Detector: max number detected, possible miss");
            }

            var newDetectionList = new List<GameObject>();
            for (var i = 0; i < numberDetected; i++)
            {
                var detectedGameObject = _collidersBuffer[i].gameObject;
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
    }
}
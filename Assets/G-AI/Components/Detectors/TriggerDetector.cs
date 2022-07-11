using System;
using UnityEngine;

namespace Eadon.AI.Detectors
{
    public class TriggerDetector : Detector
    {
        private Collider _trigger;
        
        private void OnEnable()
        {
            _trigger = GetComponent<Collider>();
            if (_trigger == null)
            {
                Debug.LogError($"{name} TriggerDetector without a collider");
            }
            else
            {
                _trigger.isTrigger = true;
            }
        }

        public override void Detect()
        {
            
        }

        private void OnTriggerEnter(Collider other)
        {
            if (IsDetectionValid(other.gameObject))
            {
                detectedObjects.Add(other.gameObject);
                onDetected.Invoke(other.gameObject);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (detectedObjects.Contains(other.gameObject))
            {
                detectedObjects.Remove(other.gameObject);
                onLostDetection.Invoke(other.gameObject);
            }
        }
    }
}
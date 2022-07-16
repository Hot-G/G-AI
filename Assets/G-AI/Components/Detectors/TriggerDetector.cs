using UnityEngine;

namespace G_AI.Components.Detectors
{
    public class TriggerDetector : UpdateDetector
    {
#if UNITY_EDITOR
    
        private Collider trigger;

        private void OnEnable()
        {
            trigger = GetComponent<Collider>();
            if (trigger == null)
            {
                Debug.LogError($"{name} TriggerDetector without a collider");
            }
            else
            {
                trigger.isTrigger = true;
            }
        }
    
#endif

        public override void Detect()
        {
            for(var i = detectedObjects.Count - 1; i > -1; i--)
            {
                if (!detectedObjects[i] || !detectedObjects[i].activeSelf)
                {
                    var removedObject = detectedObjects[i];
                    detectedObjects.RemoveAt(i);
                    onLostDetection.Invoke(removedObject);
                }
            }
        }


        private void OnTriggerEnter(Collider other)
        {
            if (IsDetectionValid(other.gameObject))
            {
                detectedObjects.Add(other.gameObject);
                onDetected?.Invoke(other.gameObject);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (detectedObjects.Contains(other.gameObject))
            {
                detectedObjects.Remove(other.gameObject);
                onLostDetection?.Invoke(other.gameObject);
            }
        }
    }
}
using UnityEngine;

public class UpdateDetector : Detector
{
    public DetectionMode detectionMode;
    public float checkInterval = 1f;
    
    private float _checkTimer;
    
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
}
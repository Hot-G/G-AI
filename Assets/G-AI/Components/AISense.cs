using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISense : MonoBehaviour
{
    [SerializeField] private float radius;
    [Tooltip("How Many gameObject sensed")]
    [SerializeField] private int senseCount = 5;
    [SerializeField] private LayerMask seeMask;
    
    private Collider[] itemRaycastHits, oldRaycastHits;
    private int currentSenseCount, oldSenseCount;

    public delegate void OnSeeTargetDelegate(int seeCount, Collider[] sensingColliders);
    public OnSeeTargetDelegate OnSenseUpdated;

    private void Start()
    {
        itemRaycastHits = new Collider[senseCount];
        oldRaycastHits = new Collider[senseCount];
    }
    
    private void Update()
    {
        LookArea();
    }

    private void LookArea()
    {
        currentSenseCount = Physics.OverlapSphereNonAlloc(transform.position, radius, itemRaycastHits,
            seeMask);
        
        if (oldSenseCount == currentSenseCount)
            if (!CheckListIsChanged(currentSenseCount))
                return;

        OnSenseUpdated?.Invoke(currentSenseCount, itemRaycastHits);
        
        Array.Copy(itemRaycastHits, oldRaycastHits, currentSenseCount);
        oldSenseCount = currentSenseCount;
    }

    private bool CheckListIsChanged(int count)
    {
        for (var i = 0; i < count; i++)
        {
            if (Array.IndexOf(oldRaycastHits, itemRaycastHits[i]) == -1)
                return true;
        }

        return false;
    }

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        
        Gizmos.DrawWireSphere(transform.position, radius);
    }

#endif
}

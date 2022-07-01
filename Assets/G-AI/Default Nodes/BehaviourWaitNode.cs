 
using UnityEngine;

public class BehaviourWaitNode : BehaviourActionNode
{
    [Min(0)]
    public float duration = 1;
    [Tooltip("duration + Random.Range(-randomRange, randomRange)")]
    public float randomRange = 0;
    private float realDuration;
    private float startTime;

    public override void OnStart()
    {
        startTime = Time.time;
        realDuration = duration + Random.Range(-randomRange, randomRange);
        if (realDuration < 0)
            realDuration = 0;
    }

    public override State OnUpdate()
    {
        return Time.time - startTime > realDuration ? State.Success : State.Running;
    }
}
 
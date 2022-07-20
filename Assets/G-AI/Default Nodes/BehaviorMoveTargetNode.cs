using UnityEngine;

public class BehaviorMoveTargetNode : BehaviorActionNode
{
    public override string NodeName => "Move Target";

    private Transform targetTransform;
    private Vector3 oldPosition;
    public BlackboardKeySelector keySelector;
    public override void OnStart()
    {
        targetTransform = keySelector.GetTransformValue();
        oldPosition = targetTransform.position;
        blackboard.navMeshAgent.SetDestination(oldPosition);
    }

    public override State OnUpdate()
    {
        if (!targetTransform) return State.Failure;
        
        if (targetTransform != keySelector.GetTransformValue())
        {
            targetTransform = keySelector.GetTransformValue();
            oldPosition = targetTransform.position;
            blackboard.navMeshAgent.SetDestination(oldPosition);
        }

        if (Vector3.SqrMagnitude(oldPosition - targetTransform.position) > 0.1f)
        {
            oldPosition = targetTransform.position;
            blackboard.navMeshAgent.SetDestination(oldPosition);
        }
        
        if (!blackboard.navMeshAgent.pathPending)
        {
            if (blackboard.navMeshAgent.remainingDistance <= blackboard.navMeshAgent.stoppingDistance)
            {
                if (!blackboard.navMeshAgent.hasPath || blackboard.navMeshAgent.velocity.sqrMagnitude <= 0.2f)
                {
                    return State.Success;
                }
            }
        }

        return State.Running;
    }
}

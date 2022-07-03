using UnityEngine;
using UnityEngine.AI;

public class BehaviorMoveRandomPointNode : BehaviorActionNode
{
    public float moveRadius;

    private Vector3 movePosition;
    private float remainingDistance;

    public override string NodeName => "Move Random Point";

    public override void OnStart()
    {
        var randomDirection = Random.insideUnitSphere * moveRadius;
        NavMesh.SamplePosition(randomDirection, out var hit, moveRadius, 1);
        movePosition = hit.position;

        blackboard.navMeshAgent.SetDestination(movePosition);
    }

    public override State OnUpdate()
    {
        remainingDistance = blackboard.navMeshAgent.remainingDistance;
        if (float.IsPositiveInfinity(remainingDistance)) return State.Failure;

        return blackboard.navMeshAgent.pathStatus == NavMeshPathStatus.PathComplete
               && blackboard.navMeshAgent.remainingDistance == 0
            ? State.Success
            : State.Running;
    }
}

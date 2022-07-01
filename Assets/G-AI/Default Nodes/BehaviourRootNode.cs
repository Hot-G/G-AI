using UnityEngine;

public class BehaviourRootNode : BehaviourNode
{
    public BehaviourNode child;
    
    public override string NodeName => "ROOT";

    public override State OnUpdate()
    {
        child.Update();
        return State.Running;
    }

    public override void UpdateState(State newState)
    {
        child.UpdateState(newState);
    }

    public override BehaviourNode Clone()
    {
        BehaviourRootNode node = Instantiate(this);
        node.child = child.Clone();
        return node;
    }
}

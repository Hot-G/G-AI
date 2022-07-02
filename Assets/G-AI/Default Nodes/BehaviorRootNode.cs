using UnityEngine;

public class BehaviorRootNode : BehaviorNode
{
    public BehaviorNode child;
    
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

    public override BehaviorNode Clone()
    {
        BehaviorRootNode node = Instantiate(this);
        node.child = child.Clone();
        return node;
    }
}

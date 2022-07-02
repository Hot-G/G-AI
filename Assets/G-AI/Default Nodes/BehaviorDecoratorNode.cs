using UnityEngine;

public class BehaviorDecoratorNode : BehaviorNode
{
    [HideInInspector] public BehaviorNode child;
    
    public override void UpdateState(State newState)
    {
        base.UpdateState(newState);
        child.UpdateState(newState);
    }

    
    public override BehaviorNode Clone()
    {
        BehaviorDecoratorNode node = Instantiate(this);
        node.child = child.Clone();
        return node;
    }
}

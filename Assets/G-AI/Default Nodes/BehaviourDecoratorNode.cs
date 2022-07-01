using UnityEngine;

public class BehaviourDecoratorNode : BehaviourNode
{
    [HideInInspector] public BehaviourNode child;
    
    public override void UpdateState(State newState)
    {
        base.UpdateState(newState);
        child.UpdateState(newState);
    }

    
    public override BehaviourNode Clone()
    {
        BehaviourDecoratorNode node = Instantiate(this);
        node.child = child.Clone();
        return node;
    }
}

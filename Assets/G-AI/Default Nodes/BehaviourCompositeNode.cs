using System.Collections.Generic;
using UnityEngine;

public class BehaviourCompositeNode : BehaviourNode
{
    [HideInInspector] public List<BehaviourNode> children = new List<BehaviourNode>();

    public override void UpdateState(State newState)
    {
        base.UpdateState(newState);

        foreach (var child in children)
        {
            child.UpdateState(newState);
        }
    }

    public override BehaviourNode Clone()
    {
        BehaviourCompositeNode node = Instantiate(this);
        node.children = children.ConvertAll(c => c.Clone());
        return node;
    }
}

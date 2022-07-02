using System.Collections.Generic;
using UnityEngine;

public class BehaviorCompositeNode : BehaviorNode
{
    [HideInInspector] public List<BehaviorNode> children = new List<BehaviorNode>();

    public override void UpdateState(State newState)
    {
        base.UpdateState(newState);

        foreach (var child in children)
        {
            child.UpdateState(newState);
        }
    }

    public override BehaviorNode Clone()
    {
        BehaviorCompositeNode node = Instantiate(this);
        node.children = children.ConvertAll(c => c.Clone());
        return node;
    }
}

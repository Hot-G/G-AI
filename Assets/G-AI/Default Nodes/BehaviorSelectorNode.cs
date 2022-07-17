using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorSelectorNode : BehaviorCompositeNode
{
    private int currentIndex;

    public override string NodeName => "SELECTOR";

    public override void OnStart()
    {
        currentIndex = 0;
    }

    public override State OnUpdate()
    {
        var child = children[currentIndex];

        switch (child.Update())
        {
            case State.Running:
                return State.Running;
            case State.Failure:
                currentIndex++;
                return currentIndex == children.Count ? State.Success : State.Running;
            case State.Success:
                currentIndex++;
                break;
            default:
                throw new System.ArgumentOutOfRangeException();
        }

        return currentIndex == children.Count ? State.Success : State.Running;
    }
}

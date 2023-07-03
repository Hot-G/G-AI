using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorSelectorNode : BehaviorCompositeNode
{
    private int currentIndex;
    private State finishState;

    public override string NodeName => "SELECTOR";

    public override void OnStart()
    {
        currentIndex = 0;
        finishState = State.Failure;
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
                break;
            case State.Success:
                currentIndex++;
                finishState = State.Success;
                break;
            default:
                throw new System.ArgumentOutOfRangeException();
        }

        return currentIndex == children.Count ? finishState : State.Running;
    }
}

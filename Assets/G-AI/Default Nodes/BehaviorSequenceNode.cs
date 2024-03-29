﻿
using System;

public class BehaviorSequenceNode : BehaviorCompositeNode
{
    private int currentIndex;

    public override string NodeName => "SEQUENCE";

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
                return State.Failure;
            case State.Success:
                currentIndex++;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return currentIndex == children.Count ? State.Success : State.Running;
    }
}

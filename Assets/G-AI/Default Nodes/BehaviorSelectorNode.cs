using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
#endif


public class BehaviorSelectorNode : BehaviorCompositeNode
{
    public override string NodeName => "Selector";

    public override State OnUpdate()
    {
        //if ()
        return State.Running;
    }
}



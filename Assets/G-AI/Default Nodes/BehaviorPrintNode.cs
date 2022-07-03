
using UnityEngine;

public class BehaviorPrintNode : BehaviorActionNode
{
    public string printText;

    public override string NodeName => "Print Log";

    public override void OnStart()
    {
        Debug.Log(printText);
    }

    public override State OnUpdate()
    {
        return State.Success;
    }
}

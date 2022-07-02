
using UnityEngine;

public class BehaviorPrintNode : BehaviorActionNode
{
    public string printText;
    
    public override void OnStart()
    {
        Debug.Log(printText);
    }

    public override State OnUpdate()
    {
        return State.Success;
    }
}

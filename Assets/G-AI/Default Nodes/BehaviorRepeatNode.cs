
public class BehaviorRepeatNode : BehaviorDecoratorNode
{
    public override string NodeName => "REPEATER";

    public int repeatTime = 1;
    private int repeatCounter;

    public override State OnUpdate()
    {
        child.Update();
        return ++repeatCounter >= repeatTime ? State.Success : State.Running;
    }
}

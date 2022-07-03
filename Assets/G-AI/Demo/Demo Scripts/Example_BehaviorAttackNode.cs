namespace G_AI.Example
{
    
    public class Example_BehaviorAttackNode : BehaviorActionNode
    {
        public override string NodeName => "Attack";

        public override State OnUpdate()
        {
            blackboard.owner.GetComponent<PawnController>().AttackTarget();
            return State.Success;
        }
    }   
    
}

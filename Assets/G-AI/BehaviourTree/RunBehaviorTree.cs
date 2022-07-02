using UnityEngine;

public class RunBehaviorTree : MonoBehaviour
{
    public BehaviorTree tree;
    [HideInInspector] public Blackboard blackboard;

    private void Awake()
    {
        tree = tree.Clone();
        blackboard = tree.blackboard.Clone();
        blackboard.SetupBlackboard(blackboard.GetType());
        tree.blackboard = blackboard;
        tree.BindBlackboard();
    }

    private void Update()
    {
        tree.Update();
    }

    public void Stop()
    {
        tree.rootNode.UpdateState(BehaviorNode.State.Failure);
        enabled = false;
    }
    
    public void Start()
    {
        enabled = true;
    }
}

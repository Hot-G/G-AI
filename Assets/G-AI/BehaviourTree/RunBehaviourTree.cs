using UnityEngine;

public class RunBehaviourTree : MonoBehaviour
{
    public BehaviourTree tree;
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
        tree.rootNode.UpdateState(BehaviourNode.State.Failure);
        enabled = false;
    }
    
    public void Start()
    {
        enabled = true;
    }
}

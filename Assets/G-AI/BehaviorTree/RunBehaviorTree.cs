using G_AI.BehaviorTree;
using UnityEngine;
using UnityEngine.AI;

public class RunBehaviorTree : MonoBehaviour
{
    public BehaviorTree tree;
    [HideInInspector] public Blackboard blackboard;

    private void Awake()
    {
        if (tree == null) return;
        tree = tree.Clone();

        if (tree.blackboard == null)
        {
            tree.blackboard = ScriptableObject.CreateInstance<Blackboard>();
            blackboard = tree.blackboard;
        }
        else
        {
            blackboard = tree.blackboard.Clone();
            blackboard.SetupBlackboard(blackboard.GetType());
            tree.blackboard = blackboard;

        }

        tree.BindBlackboard();
        //ASSIGN BLACKBOARD DEFAULT VALUES
        blackboard.owner = transform;
        blackboard.navMeshAgent = GetComponent<NavMeshAgent>();
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

using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class BehaviorMoveTargetNode : BehaviorActionNode
{
    public override string NodeName => "Move Target";

    [HideInInspector] public string targetVariableName;

    private Transform targetTransform;
    private Vector3 oldPosition;
    
    public override void OnStart()
    {
        targetTransform = blackboard.GetTransformValue(targetVariableName);
        oldPosition = targetTransform.position;
        blackboard.navMeshAgent.SetDestination(oldPosition);
    }

    public override State OnUpdate()
    {
        if (targetTransform != blackboard.GetTransformValue(targetVariableName))
        {
            targetTransform = blackboard.GetTransformValue(targetVariableName);
            oldPosition = targetTransform.position;
            blackboard.navMeshAgent.SetDestination(oldPosition);
        }

        if (Vector3.SqrMagnitude(oldPosition - targetTransform.position) > 2f)
        {
            oldPosition = targetTransform.position;
            blackboard.navMeshAgent.SetDestination(oldPosition);
        }
        
        if (!blackboard.navMeshAgent.pathPending)
        {
            if (blackboard.navMeshAgent.remainingDistance <= blackboard.navMeshAgent.stoppingDistance)
            {
                if (!blackboard.navMeshAgent.hasPath || blackboard.navMeshAgent.velocity.sqrMagnitude <= 0.2f)
                {
                    return State.Success;
                }
            }
        }

        return State.Running;
    }

#if UNITY_EDITOR
    
    [CustomEditor(typeof(BehaviorMoveTargetNode))]
    public class BehaviorMoveTargetEditor : Editor
    {
        private SerializedProperty variableNameSerialized;
        private BehaviorMoveTargetNode selectorItem;
        private void OnEnable()
        {
            selectorItem = (BehaviorMoveTargetNode)target;
            variableNameSerialized = serializedObject.FindProperty("targetVariableName");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space(15);
            
            BlackboardKeyShower.ShowBlackboardKeyInPopup(typeof(UnityEngine.Transform), selectorItem, selectorItem.targetVariableName, (index, variableName) =>
            {
                variableNameSerialized.stringValue = variableName;
                            
                serializedObject.ApplyModifiedProperties();
            });
        }
    }
    
#endif
}

using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class BehaviorMoveTargetNode : BehaviorActionNode
{
    public override string NodeName => "Move Target";

    [HideInInspector] public string targetVariableName;

    private Transform targetTransform;
    private Vector3 oldPosition;
    public BlackboardKeySelector keySelector;
    public override void OnStart()
    {
        targetTransform = keySelector.GetTransformValue();
        oldPosition = targetTransform.position;
        blackboard.navMeshAgent.SetDestination(oldPosition);
    }

    public override State OnUpdate()
    {
        if (!targetTransform) return State.Failure;
        
        if (targetTransform != keySelector.GetTransformValue())
        {
            targetTransform = keySelector.GetTransformValue();
            oldPosition = targetTransform.position;
            blackboard.navMeshAgent.SetDestination(oldPosition);
        }

        if (Vector3.SqrMagnitude(oldPosition - targetTransform.position) > 0.1f)
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
            return;
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Node Settings");
            EditorGUILayout.Space(5);
            
            BlackboardKeyShower.ShowBlackboardKeyInPopup(typeof(UnityEngine.Transform), selectorItem.blackboard, selectorItem.targetVariableName, (index, variableName) =>
            {
                variableNameSerialized.stringValue = variableName;
                            
                serializedObject.ApplyModifiedProperties();
            });
        }
    }
    
#endif
}

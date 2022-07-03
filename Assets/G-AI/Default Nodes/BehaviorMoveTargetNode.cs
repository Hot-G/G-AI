using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class BehaviorMoveTargetNode : BehaviorActionNode
{
    public override string NodeName => "Move Target";

    [HideInInspector] public string targetVariableName;
    public float acceptRadius = 1f;

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
        if ((targetTransform.position - oldPosition).sqrMagnitude > 1f)
        {
            oldPosition = targetTransform.position;
            blackboard.navMeshAgent.SetDestination(oldPosition);
        }
        
        if (float.IsPositiveInfinity(blackboard.navMeshAgent.remainingDistance)) return State.Failure;
        
        return blackboard.navMeshAgent.remainingDistance < acceptRadius
            ? State.Success
            : State.Running;
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

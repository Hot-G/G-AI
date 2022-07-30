public class BehaviorBoolSelectorNode : BehaviorDecoratorNode
{
    public override string NodeName => "Bool Selector";
    //[HideInInspector] public string boolVariableName;
    public BlackboardKeySelector keySelector;
    public bool equalValue;

    private bool answerValue;

    public override State OnUpdate()
    {
        answerValue = keySelector.GetBoolValue() == equalValue;
        if (!answerValue)
        {
            UpdateState(State.Failure);
            return State.Failure;
        }
        
        return child.Update();
    }
    
/*    #if UNITY_EDITOR

    [CustomEditor(typeof(BehaviorBoolSelectorNode))]
    public class BehaviorBoolSelectorEditor : Editor
    {
        private SerializedProperty equalValueSerialized, variableNameSerialized;
        private BehaviorBoolSelectorNode selectorItem;
        private void OnEnable()
        {
            selectorItem = (BehaviorBoolSelectorNode)target;
            variableNameSerialized = serializedObject.FindProperty("boolVariableName");
            equalValueSerialized = serializedObject.FindProperty("equalValue");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            return;
            EditorGUILayout.Space(15);
            
            BlackboardKeyShower.ShowBlackboardKeyInPopup(typeof(bool), selectorItem.blackboard, selectorItem.boolVariableName, (index, variableName) =>
            {
                variableNameSerialized.stringValue = variableName;
            });

            EditorGUILayout.PropertyField(equalValueSerialized);
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif */
}
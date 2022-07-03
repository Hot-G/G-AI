using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class BehaviorBoolSelectorNode : BehaviorDecoratorNode
{
    public override string NodeName => "Bool Selector";
    [HideInInspector] public string boolVariableName;
    [HideInInspector] public bool equalValue;

    private bool answerValue;

    public override State OnUpdate()
    {
        answerValue = blackboard.GetBoolValue(boolVariableName) == equalValue;
        if (!answerValue)
            UpdateState(State.Failure);
        return answerValue ? child.Update() : State.Failure;
    }
    
    #if UNITY_EDITOR

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

            EditorGUILayout.Space(15);
            
            BlackboardKeyShower.ShowBlackboardKeyInPopup(typeof(bool), selectorItem, selectorItem.boolVariableName, (index, variableName) =>
            {
                variableNameSerialized.stringValue = variableName;
            });

            EditorGUILayout.PropertyField(equalValueSerialized);
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}




public class BlackboardKeySelector
{
    public string keyName;
    public Blackboard blackboard;

    public BlackboardKeySelector(Blackboard usedBlackboard)
    {
        blackboard = usedBlackboard;
    }

    /*public static void ShowBlackboardKeyInPopup(System.Type type, BehaviorNode node, string currentSelectedVariable,
        System.Action<int, string> onSelectionChanged)
    {
        var variableNameList = new List<string>();
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Blackboard Key", GUILayout.MaxWidth(150));

        if (node.blackboard == null)
        {
            EditorGUILayout.LabelField("Blackboard is not selected");
            return;
        }
        
        var fields = node.blackboard.GetType().GetFields()
            .Where(a => a.FieldType == type);

        variableNameList.Clear();
        
        if (!fields.Any())
        {
            EditorGUILayout.LabelField("No " + type.Name +" value");
            return;
        }

        variableNameList.AddRange(fields.Select(blackboardField => blackboardField.Name));

        var index = variableNameList.IndexOf(currentSelectedVariable);

        EditorGUI.BeginChangeCheck();

        index = EditorGUILayout.Popup(index, variableNameList.ToArray());

        if (EditorGUI.EndChangeCheck())
        {
            onSelectionChanged?.Invoke(index, variableNameList[index]);
        }
        
        EditorGUILayout.EndHorizontal();
    }*/

    //[CustomEditor(typeof(BlackboardKeySelector))]
    public class BehaviorKeyEditor : Editor
    {
        private SerializedProperty variableNameSerialized;
        private BlackboardKeySelector blackboardKey;
        private Blackboard blackboard;

        private void OnEnable()
        {
            //blackboardKey = (BlackboardKeySelector)target;
            variableNameSerialized = serializedObject.FindProperty("keyName");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space(10);

            var variableNameList = new List<string>();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Blackboard Key", GUILayout.MaxWidth(150));

            if (serializedObject.FindProperty("blackboard") == null)
            {
                EditorGUILayout.LabelField("Blackboard is not selected");
                return;
            }

            var fields = serializedObject.FindProperty("blackboard").GetType().GetFields()
                .Where(a => a.FieldType == typeof(bool));

            variableNameList.Clear();

            if (!fields.Any())
            {
                EditorGUILayout.LabelField("No " + "bool" + " value");
                return;
            }

            variableNameList.AddRange(fields.Select(blackboardField => blackboardField.Name));

            //var index = variableNameList.IndexOf(currentSelectedVariable);
            var index = 0;

            EditorGUI.BeginChangeCheck();

            index = EditorGUILayout.Popup(index, variableNameList.ToArray());

            if (EditorGUI.EndChangeCheck())
            {

            }

            EditorGUILayout.EndHorizontal();


            serializedObject.ApplyModifiedProperties();
        }
    }
}


public static class BlackboardKeyShower
{
    public static void ShowBlackboardKeyInPopup(System.Type type, BehaviorNode node, string currentSelectedVariable,
        System.Action<int, string> onSelectionChanged)
    {
        var variableNameList = new List<string>();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Blackboard Key", GUILayout.MaxWidth(150));

        if (node.blackboard == null)
        {
            EditorGUILayout.LabelField("Blackboard is not selected");
            return;
        }

        var fields = node.blackboard.GetType().GetFields()
            .Where(a => a.FieldType == type);

        variableNameList.Clear();

        if (!fields.Any())
        {
            EditorGUILayout.LabelField("No " + type.Name + " value");
            return;
        }

        variableNameList.AddRange(fields.Select(blackboardField => blackboardField.Name));

        var index = variableNameList.IndexOf(currentSelectedVariable);

        EditorGUI.BeginChangeCheck();

        index = EditorGUILayout.Popup(index, variableNameList.ToArray());

        if (EditorGUI.EndChangeCheck())
        {
            onSelectionChanged?.Invoke(index, variableNameList[index]);
        }

        EditorGUILayout.EndHorizontal();
    }
}
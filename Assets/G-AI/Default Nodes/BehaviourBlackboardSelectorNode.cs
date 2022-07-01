using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
#endif


public class BehaviourBlackboardSelectorNode : BehaviourDecoratorNode
{
    public override string NodeName => "BLACKBOARD SELECTOR";
    public List<Condition> conditions = new List<Condition>();

    public override State OnUpdate()
    {
        //if ()
        return State.Running;
    }
    
    public struct Condition
    {
        public Type conditionType;
        public string variableName;
        public CompareType compareType;
        public string compareStringValue;
        public bool compareBoolValue;
        public int compareIntValue;
        public float compareFloatValue;
    }

    public enum CompareType
    {
        IsGreater,
        IsEqual,
        IsLower
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(BehaviourBlackboardSelectorNode))]
    public class BehaviourBlackboardSelectorEditor : Editor
    {
        private BehaviourBlackboardSelectorNode selectorItem;
        private System.Type blackboardType;
        private List<Condition> compareList;
        private List<string> variableNameList;
        private FieldInfo[] fieldList;
        private int variableIndex;
        private void OnEnable()
        {
            selectorItem = (BehaviourBlackboardSelectorNode) target;
            compareList = new List<Condition>(selectorItem.conditions);
            //BLACKBOARD VARIABLES
            blackboardType = selectorItem.blackboard.GetType();
            variableNameList = new List<string>();
            fieldList = blackboardType.GetFields();
            variableIndex = 0;

            foreach (var fieldInfo in fieldList)
            {
                variableNameList.Add(fieldInfo.Name);
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            //DESIGN
            EditorGUILayout.Space(15);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Conditions");

            if (GUILayout.Button("+"))
            {
                compareList.Add(new Condition
                {
                    conditionType = fieldList[0].FieldType,
                    variableName = fieldList[0].Name,
                    compareType = CompareType.IsEqual
                });

                selectorItem.conditions.Add(new Condition
                {
                    conditionType = fieldList[0].FieldType,
                    variableName = fieldList[0].Name,
                    compareType = CompareType.IsEqual
                });
            }
            
            EditorGUILayout.EndHorizontal();
            

            foreach (var condition in compareList)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginChangeCheck();

                variableIndex = EditorGUILayout.Popup(variableNameList.IndexOf(condition.variableName), variableNameList.ToArray());

                if (EditorGUI.EndChangeCheck())
                {
                    //condition.variableName = variableNameList[variableIndex];
                }

                EditorGUILayout.EndHorizontal();
            }
        }

        private void DrawArea(Condition condition)
        {
            bool value = false;
            Debug.Log(condition.conditionType);
            if (condition.conditionType == typeof(bool))
            {
                condition.compareBoolValue = EditorGUILayout.Toggle(condition.compareBoolValue);
            }
            else if (condition.conditionType == typeof(string))
            {
                Debug.Log("THIS IS STRING");
                condition.compareType = (CompareType)EditorGUILayout.EnumPopup(condition.compareType);
                condition.compareStringValue = EditorGUILayout.TextField(condition.compareStringValue);
            }
            else if (condition.conditionType == typeof(int))
            {
                condition.compareType = (CompareType)EditorGUILayout.EnumPopup(condition.compareType);
                condition.compareIntValue = EditorGUILayout.IntField(condition.compareIntValue);
            }
            
        }
    }
#endif
}



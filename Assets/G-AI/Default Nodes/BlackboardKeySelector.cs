using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
[System.Serializable]
public class BlackboardKeySelector
{
    public string keyName;
    public Blackboard blackboard;

    public bool GetBoolValue()
    {
        return blackboard.GetBoolValue(keyName);
    }
    
    public string GetStringValue()
    {
        return blackboard.GetStringValue(keyName);
    }
    
    public int GetIntValue()
    {
        return blackboard.GetIntValue(keyName);
    }
    
    public Transform GetTransformValue()
    {
        return blackboard.GetTransformValue(keyName);
    }
    
    public T GetObjectValue<T>()
    {
        return blackboard.GetObjectValue<T>(keyName);
    }
    
    public void SetBoolValue(bool value)
    {
        blackboard.SetBoolValue(keyName, value);
    }
    
    public void SetStringValue(string value)
    {
        blackboard.SetStringValue(keyName, value);
    }
    
    public void SetIntValue(int value)
    {
        blackboard.SetIntValue(keyName, value);
    }
    
    public void SetTransformValue(Transform value)
    {
        blackboard.SetTransformValue(keyName, value);
    }

    public void SetObjectValue(object value)
    {
        blackboard.SetObjectValue(keyName, value);
    }

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(BlackboardKeySelector))]
    public class BehaviorKeySelectorDrawer : PropertyDrawer
    {
        private Blackboard blackboard;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var variableNameList = new List<string>();
            blackboard = (Blackboard)property.FindPropertyRelative("blackboard").objectReferenceValue;

            EditorGUI.BeginProperty(position, label, property);

            EditorGUILayout.BeginHorizontal();
            
            EditorGUI.LabelField(position, new GUIContent("Blackboard Key"));

            if (blackboard == null)
            {
                EditorGUILayout.LabelField("Blackboard is not selected");
                EditorGUILayout.EndHorizontal();
                return;
            }

            var fields = blackboard.GetType().GetFields();

            variableNameList.Clear();

            if (!fields.Any())
            {
                EditorGUILayout.LabelField("No blackboard value");
                return;
            }

            variableNameList.AddRange(fields.Select(blackboardField => blackboardField.Name));

            var index = variableNameList.IndexOf(property.FindPropertyRelative("keyName").stringValue);

            var popupPosition = new Rect(position.x + 150, position.y, position.width - 150, position.height);
            
            index = EditorGUI.Popup(popupPosition, index, variableNameList.ToArray());

            if (EditorGUI.EndChangeCheck())
            {
                property.FindPropertyRelative("keyName").stringValue = variableNameList[index];
                property.serializedObject.ApplyModifiedProperties();
            }

            EditorGUI.EndProperty();
            
            EditorGUILayout.EndHorizontal();
        }
    }
        
#endif
    
}
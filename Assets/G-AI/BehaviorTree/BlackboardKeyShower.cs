#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class BlackboardKeyShower
{
    public static void ShowBlackboardKeyInPopup(System.Type type, Blackboard blackboard, string currentSelectedVariable,
        System.Action<int, string> onSelectionChanged)
    {
        var variableNameList = new List<string>();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Blackboard Key", GUILayout.MaxWidth(150));

        if (blackboard == null)
        {
            EditorGUILayout.LabelField("Blackboard is not selected");
            return;
        }

        var fields = blackboard.GetType().GetFields()
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
#endif
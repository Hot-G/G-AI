using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
[CreateAssetMenu(menuName = "AI/Blackboard", order = 0)]
public class Blackboard : ScriptableObject
{
    public NavMeshAgent navMeshAgent;
    public Transform owner;

    private readonly Dictionary<string, bool> boolVariables = new Dictionary<string, bool>();
    private readonly Dictionary<string, string> stringVariables = new Dictionary<string, string>();
    private readonly Dictionary<string, int> intVariables = new Dictionary<string, int>();
    private readonly Dictionary<string, Transform> transformVariables = new Dictionary<string, Transform>();
    private readonly Dictionary<string, object> objectVariables = new Dictionary<string, object>();

    public void SetupBlackboard(System.Type blackboardType)
    {
        foreach (var fieldInfo in blackboardType.GetFields())
        {
            var fieldValue = fieldInfo.GetValue(this);

            if (fieldInfo.FieldType == typeof(bool))
                boolVariables.Add(fieldInfo.Name, (bool)fieldValue);
            else if (fieldInfo.FieldType == typeof(int))
                intVariables.Add(fieldInfo.Name, (int)fieldValue);
            else if (fieldInfo.FieldType == typeof(string))
                stringVariables.Add(fieldInfo.Name, (string)fieldValue);
            else if (fieldInfo.FieldType == typeof(Transform))
                transformVariables.Add(fieldInfo.Name, (Transform)fieldValue);
            else if (fieldInfo.FieldType == typeof(object))
                objectVariables.Add(fieldInfo.Name, fieldValue);
        }
    }

    public Blackboard Clone()
    {
        return Instantiate(this);
    }

    public bool GetBoolValue(string propertyName)
    {
        return boolVariables[propertyName];
    }
    
    public string GetStringValue(string propertyName)
    {
        return stringVariables[propertyName];
    }
    
    public int GetIntValue(string propertyName)
    {
        return intVariables[propertyName];
    }
    
    public Transform GetTransformValue(string propertyName)
    {
        return transformVariables[propertyName];
    }
    
    public T GetObjectValue<T>(string propertyName)
    {
        return (T)objectVariables[propertyName];
    }
    
    public void SetBoolValue(string propertyName, bool value)
    {
        boolVariables[propertyName] = value;
    }
    
    public void SetStringValue(string propertyName, string value)
    {
        stringVariables[propertyName] = value;
    }
    
    public void SetIntValue(string propertyName, int value)
    {
        intVariables[propertyName] = value;
    }
    
    public void SetTransformValue(string propertyName, Transform value)
    {
        transformVariables[propertyName] = value;
    }

    public void SetObjectValue(string propertyName, object value)
    {
        objectVariables[propertyName] = value;
    }
}

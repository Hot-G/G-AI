using System.Collections.Generic;

#if UNITY_EDITOR
using System.IO;
#endif

using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

[System.Serializable]
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

#if UNITY_EDITOR

    public string GetValue(string propertyName, System.Type propertyType)
    {
        if (propertyName == "owner")
            return owner.ToString();
        if (propertyName == "navMeshAgent")
            return navMeshAgent.ToString();
        
        if (typeof(bool) == propertyType)
            return GetBoolValue(propertyName).ToString();
        if (typeof(string) == propertyType)
            return GetStringValue(propertyName);
        if (typeof(int) == propertyType)
            return GetIntValue(propertyName).ToString();
        if (typeof(Transform) == propertyType)
            return GetTransformValue(propertyName)?.ToString();
        
        return GetObjectValue<string>(propertyName);
    }
    
#endif
    
    
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

#if UNITY_EDITOR

public class BlackboardCreateEditor : EditorWindow
{
    [MenuItem("Assets/Create/AI/Create Blackboard")]
    public static void OpenWindow()
    {
        BlackboardCreateEditor wnd = GetWindow<BlackboardCreateEditor>(true, string.Empty, true);
        wnd.maxSize = wnd.minSize = new Vector2(350, 110);
        wnd.titleContent = new GUIContent("Create Blackboard");
        wnd.ShowPopup();
    }

    private string fileNameField;
    private TextField textField;
    private Button createButton;

    private void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Import UXML
        var visualTree =
            AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/G-AI/BehaviourTree/CreateBlackboardEditor.uxml");
        visualTree.CloneTree(root);

        textField = root.Q<TextField>();
        createButton = root.Q<Button>();
        
        textField.Focus();
        
        createButton.RegisterCallback<MouseUpEvent>(evt =>
        {
            string path = AssetDatabase.GetAssetPath (Selection.activeObject);

            if (path == "")
            {
                path = "Assets";
            }
            else if (Path.GetExtension(path) != "")
            {
                path = path.Replace(Path.GetFileName (AssetDatabase.GetAssetPath (Selection.activeObject)), "");
            }
            //CREATE FILE
            CreateScriptFile(path, textField.text);
            //OPEN FILE
            //AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<MonoScript>(path + "/" + textField.text + ".cs"));
            this.Close();
        });
    }

    private void CreateScriptFile(string filePath, string fileName)
    {
        using (var streamWriter = new StreamWriter( filePath+ "/" + fileName + ".cs" ))
        {
            streamWriter.WriteLine( "public class " + fileName + " : Blackboard" );
            streamWriter.WriteLine( "{" );
            streamWriter.WriteLine( "" );
            streamWriter.WriteLine( "}" );
            
            streamWriter.Close();
        }
        
        AssetDatabase.Refresh();
    }
}

#endif

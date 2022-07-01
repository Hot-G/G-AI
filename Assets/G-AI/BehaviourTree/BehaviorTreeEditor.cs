#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Callbacks;

public class BehaviorTreeEditor : EditorWindow
{
    private BehaviorTreeView treeView;
    private InspectorView inspectorView;
    private IMGUIContainer blackboardView;
    private Label objectNameLabel;

    private SerializedObject treeObject, blackboardObject;
    private SerializedProperty blackboardProperty;
    private Blackboard blackboard;
    private BehaviourTree tree;
    
    [MenuItem("Behavior Tree/Behavior Tree Editor")]
    public static void OpenWindow()
    {
        BehaviorTreeEditor wnd = GetWindow<BehaviorTreeEditor>();
        wnd.titleContent = new GUIContent("Behavior Tree Editor");
    }

    [OnOpenAsset]
    public static bool OnOpenAsset(int instanceId, int line)
    {
        if (!(Selection.activeObject is BehaviourTree)) return false;
        OpenWindow();
        return true;
    }

    private void OnFocus()
    {
        if (tree == null) return;
        CreateSerializedVariables();
        ShowBlackboardProperty();
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/G-AI/BehaviourTree/BehaviorTreeEditor.uxml");
        visualTree.CloneTree(root);

        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/G-AI/BehaviourTree/BehaviorTreeEditor.uss");
        root.styleSheets.Add(styleSheet);

        treeView = root.Q<BehaviorTreeView>();
        inspectorView = root.Q<InspectorView>();
        blackboardView = root.Q<IMGUIContainer>();
        objectNameLabel = treeView.Q<Label>();

        treeView.OnNodeSelected = OnNodeSelectionChanged;
        treeView.OnNodeDeleted = OnNodeDeleted;
        OnSelectionChange();
    }

    private void ShowBlackboardProperty()
    {
        if (treeObject == null) return;
        if (blackboard == null) return;
        if (blackboardView == null) return;
        blackboardView.onGUIHandler = () =>
        {
            EditorGUI.BeginChangeCheck();
            foreach (var fieldInfo in blackboard.GetType().GetFields())
            {
                EditorGUILayout.PropertyField(blackboardObject.FindProperty(fieldInfo.Name));
            }
            
            if (EditorGUI.EndChangeCheck() && !Application.isPlaying)
                blackboardObject?.ApplyModifiedProperties();
        };
    }

    private void OnEnable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
    }

    private void OnPlayModeStateChanged(PlayModeStateChange obj)
    {
        switch (obj)
        {
            case PlayModeStateChange.EnteredEditMode:
                OnSelectionChange();
                break;
            case PlayModeStateChange.ExitingEditMode:
                break;
            case PlayModeStateChange.EnteredPlayMode:
                OnSelectionChange();
                break;
            case PlayModeStateChange.ExitingPlayMode:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(obj), obj, null);
        }
    }

    private void OnSelectionChange()
    {
        tree = Selection.activeObject as BehaviourTree;
        
        var objectName = "";
        if (tree == null)
        {
            if (Selection.activeGameObject)
            {
                var runTree = Selection.activeGameObject.GetComponent<RunBehaviourTree>();
                if (runTree)
                {
                    tree = runTree.tree;
                    objectName = " (" + Selection.activeObject.name + ")";
                }
            }
        }

        if (treeView != null)
        {
            treeView.ClearView();
            objectNameLabel.text = "";
        }

        if (tree == null)
        {
            
            return;
        }

        if (Application.isPlaying)
        {
            treeView?.PopulateView(tree);
        }
        else
        {
            treeView?.PopulateView(tree);
        }
        
        CreateSerializedVariables();

        if (objectNameLabel != null)
            objectNameLabel.text = tree.name + objectName;
        //SHOW BLACKBOARD
        ShowBlackboardProperty();
    }

    private void CreateSerializedVariables()
    {
        treeObject = new SerializedObject(tree);
        blackboard = tree.blackboard;
        if (blackboard != null)
            blackboardObject = new SerializedObject(blackboard);
    }

    private void OnNodeSelectionChanged(NodeView nodeView)
    {
        inspectorView.UpdateSelection(nodeView);
    }
    
    private void OnNodeDeleted()
    {
        inspectorView.UpdateSelection(null);
    }

    private void OnInspectorUpdate()
    {
        treeView?.UpdateNodeStates();
    }

}

#endif
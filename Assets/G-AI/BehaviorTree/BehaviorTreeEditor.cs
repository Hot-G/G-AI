#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace G_AI.BehaviorTree
{
    public class BehaviorTreeEditor : EditorWindow
    {
        private BehaviorTreeView treeView;
        private InspectorView inspectorView;
        private NodeSearchWindow searchWindow;
        private IMGUIContainer blackboardView;
        private Label objectNameLabel;
        private Button toolbarSaveButton;

        private SerializedObject treeObject;
        private SerializedProperty blackboardProperty;
        private Blackboard blackboard;
        private BehaviorTree tree;

        [MenuItem("Window/AI/Behavior Tree Editor")]
        public static void OpenWindow()
        {
            BehaviorTreeEditor wnd = GetWindow<BehaviorTreeEditor>();
            wnd.titleContent = new GUIContent("Behavior Tree Editor");
        }

        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceId, int line)
        {
            if (!(Selection.activeObject is BehaviorTree)) return false;
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
            var visualTree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/G-AI/BehaviorTree/StyleSheets/BehaviorTreeEditor.uxml");
            visualTree.CloneTree(root);

            // A stylesheet can be added to a VisualElement.
            // The style will be applied to the VisualElement and all of its children.
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/G-AI/BehaviorTree/StyleSheets/BehaviorTreeEditor.uss");
            root.styleSheets.Add(styleSheet);

            treeView = root.Q<BehaviorTreeView>();
            inspectorView = root.Q<InspectorView>();
            blackboardView = root.Q<IMGUIContainer>();
            objectNameLabel = treeView.Q<Label>();
            toolbarSaveButton = root.Q<Button>("saveButton");
            
            toolbarSaveButton.clicked += SaveChanges;
        
            AddSearchWindow();

            treeView.OnNodeSelected = OnNodeSelectionChanged;
            treeView.OnNodeDeleted = OnNodeDeleted;
            treeView.OnNodeAdded = OnNodeAdded;
            treeView.OnNodeMoved = OnNodeMoved;
            treeView.OnEdgeChanged = OnEdgeChanged;
            OnSelectionChange();

            Undo.undoRedoPerformed += () =>
            {
                treeView?.ClearView();
                treeView?.PopulateView(tree);
            };
        }

        public override void SaveChanges()
        {
            base.SaveChanges();
            
            tree.SaveTree();
            AssetDatabase.SaveAssets();
        }

        public override void DiscardChanges()
        {
            base.DiscardChanges();
            
            tree.DiscardTree();
        }

        private void ShowBlackboardProperty()
        {
            if (treeObject == null) return;
            if (blackboardView == null) return;

            blackboardView.onGUIHandler = () =>
            {
                if (tree == null) return;
                if (tree.blackboard == null) return;


                foreach (var fieldInfo in tree.blackboard.GetType().GetFields())
                {
                    var fieldName = fieldInfo.FieldType.ToString();
                    var pointIndex = fieldName.LastIndexOf('.') + 1;
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(fieldInfo.Name + " (" + fieldName.Substring(pointIndex) + ")");
                    if (Application.isPlaying && tree.blackboard != null)
                        EditorGUILayout.LabelField(tree.blackboard.GetValue(fieldInfo.Name, fieldInfo.FieldType));
                    EditorGUILayout.EndHorizontal();
                }
            };
        }
    
        private void AddSearchWindow()
        {
            searchWindow = ScriptableObject.CreateInstance<NodeSearchWindow>();
            searchWindow.Configure(this, treeView);
            treeView.nodeCreationRequest = context =>
            {
                SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchWindow);
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
            if (hasUnsavedChanges)
            {
                if (!PrivateRequestClose(this))
                {
                    return;
                }
            }
            
            var selectedTree = Selection.activeObject as BehaviorTree;

            var objectName = "";
            if (selectedTree == null)
            {
                if (Selection.activeGameObject)
                {
                    var runTree = Selection.activeGameObject.GetComponent<RunBehaviorTree>();
                    if (runTree)
                    {
                        tree = runTree.tree;
                        objectName = " (" + Selection.activeObject.name + ")";
                    }
                }
            }
            else
            {
                tree = selectedTree;
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
        }

        private void OnNodeSelectionChanged(NodeView nodeView)
        {
            inspectorView.UpdateSelection(nodeView);
        }

        private void OnNodeDeleted()
        {
            inspectorView.UpdateSelection(null);
            hasUnsavedChanges = true;
        }
        
        private void OnNodeAdded(BehaviorNode behaviorNode)
        {
            hasUnsavedChanges = true;
        }
        
        private void OnNodeMoved(NodeView behaviorNodeView)
        {
            hasUnsavedChanges = true;
        }
        
        private void OnEdgeChanged(Edge edge)
        {
            hasUnsavedChanges = true;
        }

        private void OnInspectorUpdate()
        {
            treeView?.UpdateNodeStates();
        }
        
        private bool PrivateRequestClose(EditorWindow unsavedWindow)
        {
            var text = unsavedWindow.titleContent.text;
            var num = EditorUtility.DisplayDialogComplex((string.IsNullOrEmpty(text) ? "" : text + " - ") + L10n.Tr("Unsaved Changes Detected"), unsavedWindow.saveChangesMessage, L10n.Tr("Save"), L10n.Tr("Cancel"), L10n.Tr("Discard"));
            try
            {
                switch (num)
                {
                    case 0:
                        bool flag = true;
                        unsavedWindow.SaveChanges();
                        flag &= !unsavedWindow.hasUnsavedChanges;
                        return flag;
                    case 1:
                        return false;
                    case 2:
                        unsavedWindow.DiscardChanges();
                        break;
                    default:
                        Debug.LogError((object) "Unrecognized option.");
                        goto case 1;
                }
            }
            catch (Exception ex)
            {
                EditorUtility.DisplayDialog(L10n.Tr("Save Changes Failed"), ex.Message, L10n.Tr("OK"));
                return false;
            }

            return true;
        }
    }
}

#endif
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Behaviour Tree", order = 0)]
public class BehaviourTree : ScriptableObject
{
    public BehaviourNode rootNode;
    public BehaviourNode.State treeState = BehaviourNode.State.Running;
    [HideInInspector] public Blackboard blackboard;

    public List<BehaviourNode> nodes = new List<BehaviourNode>();

    public BehaviourNode.State Update()
    {
        if (rootNode.state == BehaviourNode.State.Running)
        {
            treeState = rootNode.Update();
        }

        return treeState;
    }

#if UNITY_EDITOR

    public BehaviourNode CreateNode(System.Type type, Vector2 position)
    {
        BehaviourNode node = ScriptableObject.CreateInstance(type) as BehaviourNode;
        node.nodeName = node.NodeName;
        if (node.nodeName == "")
            node.nodeName = type.Name;
        node.name = node.nodeName;
        node.guid = GUID.Generate().ToString();
        node.blackboard = blackboard;
        node.position = position;
        
        //Undo.RecordObject(this, "Behaviour Tree (Create Node)");
        nodes.Add(node);

        if (!Application.isPlaying)
        {
            AssetDatabase.AddObjectToAsset(node, this);
        }
        
        //Undo.RegisterCreatedObjectUndo(node, "Behaviour Tree (Create Node)");
        AssetDatabase.SaveAssets();
        return node;
    }
    
    public void DeleteNode(BehaviourNode node)
    {
        //Undo.RecordObject(this, "Behaviour Tree (Remove Node)");
        nodes.Remove(node);
        AssetDatabase.RemoveObjectFromAsset(node);
        //Undo.DestroyObjectImmediate(node);
        AssetDatabase.SaveAssets();
    }

    public void AddChild(BehaviourNode parent, BehaviourNode child)
    {
        if (parent is BehaviourDecoratorNode decoratorNode)
        {
            //Undo.RecordObject(decoratorNode, "Behaviour Tree (Add Child)");
            decoratorNode.child = child;
            //EditorUtility.SetDirty(decoratorNode);
        }
        
        if (parent is BehaviourRootNode behaviourRootNode)
        {
            //Undo.RecordObject(behaviourRootNode, "Behaviour Tree (Add Child)");
            behaviourRootNode.child = child;
            //EditorUtility.SetDirty(behaviourRootNode);
        }

        if (parent is BehaviourCompositeNode compositeNode)
        {
            //Undo.RecordObject(compositeNode, "Behaviour Tree (Add Child)");
            compositeNode.children.Add(child);
            //EditorUtility.SetDirty(compositeNode);
        }
    }
    
    public void RemoveChild(BehaviourNode parent, BehaviourNode child)
    {
        if (parent is BehaviourDecoratorNode decoratorNode)
        {
            //Undo.RecordObject(decoratorNode, "Behaviour Tree (Add Child)");
            decoratorNode.child = null;
            //EditorUtility.SetDirty(decoratorNode);
        }
        
        if (parent is BehaviourRootNode behaviourRootNode)
        {
            //Undo.RecordObject(behaviourRootNode, "Behaviour Tree (Add Child)");
            behaviourRootNode.child = null;
            //EditorUtility.SetDirty(behaviourRootNode);
        }
        
        if (parent is BehaviourCompositeNode compositeNode)
        {
            //Undo.RecordObject(compositeNode, "Behaviour Tree (Add Child)");
            compositeNode.children.Remove(child);
            //EditorUtility.SetDirty(compositeNode);
        }
    }
    
#endif

    public List<BehaviourNode> GetChildren(BehaviourNode parent)
    {
        var decoratorNode = parent as BehaviourDecoratorNode;
        if (decoratorNode && decoratorNode.child != null)
        {
            return new List<BehaviourNode> { decoratorNode.child };
        }
        
        if (parent is BehaviourRootNode behaviourRootNode && behaviourRootNode.child != null)
        {
            return new List<BehaviourNode> { behaviourRootNode.child };
        }
        
        var compositeNode = parent as BehaviourCompositeNode;
        if (compositeNode)
        {
            return compositeNode.children;
        }

        return new List<BehaviourNode>();
    }

    private void Traverse(BehaviourNode node, System.Action<BehaviourNode> visiter)
    {
        if (node)
        {
            visiter.Invoke(node);
            var children = GetChildren(node);
            children.ForEach(n => Traverse(n, visiter));
        }
    }

    public BehaviourTree Clone()
    {
        BehaviourTree tree = Instantiate(this);
        tree.rootNode = tree.rootNode.Clone();
        tree.nodes = new List<BehaviourNode>();
        Traverse(tree.rootNode, n =>
        {
            tree.nodes.Add(n);
        });
        
        return tree;
    }

    public void BindBlackboard()
    {
        Traverse(rootNode, node =>
        {
            node.blackboard = blackboard;
        });
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(BehaviourTree))]
    public class BehaviourTreeEditor : Editor
    {
        List<string> variableNameList = new List<string>();
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var treeItem = (BehaviourTree) target;

            EditorGUILayout.Space(15);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Blackboard");


            var types = TypeCache.GetTypesDerivedFrom<Blackboard>();
            
            variableNameList.Clear();
            if (types.Count == 0)
            {
                EditorGUILayout.LabelField("is not created");
                return;
            }
            
            int index = types.IndexOf(treeItem.blackboard.GetType());

            foreach (var blackboardType in types)
            {
                variableNameList.Add(blackboardType.Name);
            }
            
            EditorGUI.BeginChangeCheck();

            index = EditorGUILayout.Popup(index, variableNameList.ToArray());
            
            if (EditorGUI.EndChangeCheck())
            {
                if (treeItem.blackboard != null)
                {
                    AssetDatabase.RemoveObjectFromAsset(treeItem.blackboard);
                    AssetDatabase.SaveAssets();
                    treeItem.blackboard = null;
                }
                
                var blackboardInstance = CreateInstance(types[index]) as Blackboard;
                blackboardInstance.name = "USED BLACKBOARD";
                treeItem.blackboard = blackboardInstance;

                foreach (var behaviourNode in treeItem.nodes)
                {
                    behaviourNode.blackboard = blackboardInstance;
                }
                
                AssetDatabase.AddObjectToAsset(blackboardInstance, treeItem);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            
            EditorGUILayout.EndHorizontal();

        }
    }
#endif
}

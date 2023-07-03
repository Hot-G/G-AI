using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace G_AI.BehaviorTree
{
    [CreateAssetMenu(menuName = "AI/Behavior Tree", order = 0)]
    public class BehaviorTree : ScriptableObject
    {
        public BehaviorNode rootNode;
        public BehaviorNode.State treeState = BehaviorNode.State.Running;
        public Blackboard blackboard;

        public List<BehaviorNode> nodes = new List<BehaviorNode>();

        public BehaviorNode.State Update()
        {
            if (rootNode.state == BehaviorNode.State.Running)
            {
                treeState = rootNode.Update();
            }

            return treeState;
        }

#if UNITY_EDITOR
        
        private List<BehaviorNode> cacheAddedNodes = new List<BehaviorNode>();
        private List<BehaviorNode> cacheRemovedNodes = new List<BehaviorNode>();

        public void SaveTree()
        {
            nodes.AddRange(cacheAddedNodes);
            for (var i = 0; i < cacheRemovedNodes.Count; i++)
            {
                var removedNode = cacheRemovedNodes[i];
                nodes.Remove(removedNode);
                i--;
            }

            cacheAddedNodes.Clear();
            cacheRemovedNodes.Clear();
        }
        
        public void DiscardTree()
        {
            cacheAddedNodes.Clear();
            cacheRemovedNodes.Clear();
        }

        public BehaviorNode CreateNode(System.Type type, Vector2 position)
        {
            BehaviorNode node = ScriptableObject.CreateInstance(type) as BehaviorNode;
            node.nodeName = node.NodeName;
            if (node.nodeName == "")
                node.nodeName = type.Name;
            node.name = node.nodeName;
            node.guid = GUID.Generate().ToString();
            node.position = position;

            Undo.RecordObject(this, "Behaviour Tree (Create Node)");
            cacheAddedNodes.Add(node);

            if (!Application.isPlaying)
            {
                AssetDatabase.AddObjectToAsset(node, this);
            }
            
            Undo.RegisterCreatedObjectUndo(node, "Behaviour Tree (Create Node)");
            //AssetDatabase.SaveAssets();
            node.SetBlackboard(blackboard);
            
            return node;
        }

        public void DeleteNode(BehaviorNode node)
        {
            Undo.RecordObject(this, "Behaviour Tree (Remove Node)");
            if (cacheAddedNodes.Contains(node))
            {
                cacheAddedNodes.Remove(node);
            }
            else
            {
                cacheRemovedNodes.Add(node);   
            }
            
            //AssetDatabase.RemoveObjectFromAsset(node);
            Undo.DestroyObjectImmediate(node);
            //AssetDatabase.SaveAssets();
        }

        public void AddChild(BehaviorNode parent, BehaviorNode child)
        {
            if (parent is BehaviorDecoratorNode decoratorNode)
            {
                Undo.RecordObject(decoratorNode, "Behaviour Tree (Add Child)");
                decoratorNode.child = child;
            }

            if (parent is BehaviorRootNode behaviourRootNode)
            {
                Undo.RecordObject(behaviourRootNode, "Behaviour Tree (Add Child)");
                behaviourRootNode.child = child;
            }

            if (parent is BehaviorCompositeNode compositeNode)
            {
                Undo.RecordObject(compositeNode, "Behaviour Tree (Add Child)");
                compositeNode.children.Add(child);
            }
        }

        public void RemoveChild(BehaviorNode parent, BehaviorNode child)
        {
            if (parent is BehaviorDecoratorNode decoratorNode)
            {
                Undo.RecordObject(decoratorNode, "Behaviour Tree (Add Child)");
                decoratorNode.child = null;
            }

            if (parent is BehaviorRootNode behaviourRootNode)
            {
                Undo.RecordObject(behaviourRootNode, "Behaviour Tree (Add Child)");
                behaviourRootNode.child = null;
            }

            if (parent is BehaviorCompositeNode compositeNode)
            {
                Undo.RecordObject(compositeNode, "Behaviour Tree (Add Child)");
                compositeNode.children.Remove(child);
            }
        }

#endif

        public List<BehaviorNode> GetChildren(BehaviorNode parent)
        {
            var decoratorNode = parent as BehaviorDecoratorNode;
            if (decoratorNode && decoratorNode.child != null)
            {
                return new List<BehaviorNode> { decoratorNode.child };
            }

            if (parent is BehaviorRootNode behaviourRootNode && behaviourRootNode.child != null)
            {
                return new List<BehaviorNode> { behaviourRootNode.child };
            }

            var compositeNode = parent as BehaviorCompositeNode;
            if (compositeNode)
            {
                return compositeNode.children;
            }

            return new List<BehaviorNode>();
        }

        private void Traverse(BehaviorNode node, System.Action<BehaviorNode> visiter)
        {
            if (node)
            {
                visiter.Invoke(node);
                var children = GetChildren(node);
                children.ForEach(n => Traverse(n, visiter));
            }
        }

        public BehaviorTree Clone()
        {
            BehaviorTree tree = Instantiate(this);
            tree.rootNode = tree.rootNode.Clone();
            tree.nodes = new List<BehaviorNode>();
            Traverse(tree.rootNode, n => { tree.nodes.Add(n); });

            return tree;
        }

        public void BindBlackboard()
        {
            Traverse(rootNode, node => { node.SetBlackboard(blackboard); });
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(BehaviorTree))]
        public class BehaviourTreeEditor : Editor
        {
            List<string> variableNameList = new List<string>();

            public override void OnInspectorGUI()
            {
                //base.OnInspectorGUI();

                var treeItem = (BehaviorTree)target;

                EditorGUILayout.Space(15);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Blackboard");


                var types = TypeCache.GetTypesDerivedFrom<Blackboard>();

                variableNameList.Clear();
                variableNameList.Add("None");
                
                if (types.Count == 0)
                {
                    EditorGUILayout.LabelField("is not created");
                    return;
                }

                foreach (var blackboardType in types)
                {
                    variableNameList.Add(blackboardType.Name);
                }
                
                int index = 0;
                if (treeItem.blackboard)
                    index = variableNameList.IndexOf(treeItem.blackboard.GetType().Name);

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

                    if (index == 0)
                    {
                        foreach (var behaviourNode in treeItem.nodes)
                        {
                            behaviourNode.SetBlackboard(null);
                        }
                        return;
                    }

                    var blackboardInstance = CreateInstance(types.First(t => t.Name == variableNameList[index])) as Blackboard;
                    blackboardInstance.name = "USED BLACKBOARD";
                    treeItem.blackboard = blackboardInstance;

                    foreach (var behaviourNode in treeItem.nodes)
                    {
                        behaviourNode.SetBlackboard(blackboardInstance);
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
}
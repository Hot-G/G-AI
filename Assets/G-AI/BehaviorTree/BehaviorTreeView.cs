#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace G_AI.BehaviorTree
{
    public class BehaviorTreeView : GraphView
    {
        public new class UxmlFactory : UxmlFactory<BehaviorTreeView, UxmlTraits> { }

        public Action<NodeView> OnNodeSelected;
        public Action OnNodeDeleted;
        public Action<BehaviorNode> OnNodeAdded;
        public Action<NodeView> OnNodeMoved;
        public Action<Edge> OnEdgeChanged;
    
        private BehaviorTree tree;
        private NodeView selectedNode;

        public BehaviorTreeView()
        {
            Insert(0, new GridBackground());

        
            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            var styleSheet =
                AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/G-AI/BehaviorTree/StyleSheets/BehaviorTreeEditor.uss");
            styleSheets.Add(styleSheet);
        
            //Undo.undoRedoPerformed += OnUndoRedo;
        }

        private void OnUndoRedo()
        {
            PopulateView(tree);
            AssetDatabase.SaveAssets();
        }

        NodeView FindNodeView(BehaviorNode node) => GetNodeByGuid(node.guid) as NodeView;

        public void ClearView()
        {
            graphViewChanged -= OnGraphViewChanged;
            graphElements.ForEach(RemoveElement);
            graphViewChanged += OnGraphViewChanged;
        }

        public void PopulateView(BehaviorTree populateTree)
        {
            if (populateTree != null)
                tree = populateTree;
            
            if (tree.rootNode == null)
            {
                tree.rootNode = populateTree.CreateNode(typeof(BehaviorRootNode), Vector2.zero) as BehaviorRootNode;
                //EditorUtility.SetDirty(tree);
                AssetDatabase.SaveAssets();
            }

            //CREATE NODE VIEW
            for (var i = 0; i < tree.nodes.Count; i++)
            {
                var behaviorNode = tree.nodes[i];
                
                if (behaviorNode == null)
                {
                    tree.nodes.Remove(behaviorNode);
                    i--;
                    continue;
                }

                CreateNodeView(behaviorNode);
            }

            //CREATE EDGES
            tree.nodes.ForEach(n =>
            {
                var children = tree.GetChildren(n);
                children.ForEach(c =>
                {
                    NodeView parentView = FindNodeView(n);
                    NodeView childView = FindNodeView(c);

                    Edge edge = parentView.output.ConnectTo(childView.input);
                    AddElement(edge);
                });
            });
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.ToList().Where(endPort => endPort.direction != startPort.direction &&
                                                   endPort.node != startPort.node).ToList();
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            graphViewChange.elementsToRemove?.ForEach(elem =>
            {
                if (elem is NodeView nodeView)
                {
                    tree.DeleteNode(nodeView.node);
                    OnNodeDeleted?.Invoke();
                }

                if (elem is Edge edge)
                {
                    NodeView parentView = edge.output.node as NodeView;
                    NodeView childView = edge.input.node as NodeView;
                    tree.RemoveChild(parentView.node, childView.node);
                    
                    OnEdgeChanged?.Invoke(edge);
                }
            });
            
            graphViewChange.edgesToCreate?.ForEach(edge =>
            {
                NodeView parentView = edge.output.node as NodeView;
                NodeView childView = edge.input.node as NodeView;
                tree.AddChild(parentView.node, childView.node);
                
                OnEdgeChanged?.Invoke(edge);
            });

            if (graphViewChange.movedElements != null)
            {
                nodes.ForEach(n =>
                {
                    var view = n as NodeView;
                    view.SortChildren();
                    
                    OnNodeMoved?.Invoke(view);
                });
            }

            return graphViewChange;
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            if (selectedNode != null)
            {
                base.BuildContextualMenu(evt);
                return;
            }
        
            {
                var types = TypeCache.GetTypesDerivedFrom<BehaviorActionNode>();
                foreach (var type in types)
                {
                    BehaviorNode node = ScriptableObject.CreateInstance(type) as BehaviorNode;
                    var nodeName = node.NodeName;
                    if (nodeName == "")
                        nodeName = type.Name;
                    evt.menu.AppendAction($"Actions/{nodeName}",
                        a => CreateBehaviorNode(type, a.eventInfo.localMousePosition));
                    ScriptableObject.DestroyImmediate(node);
                }
            }

            {
                var types = TypeCache.GetTypesDerivedFrom<BehaviorCompositeNode>();
                foreach (var type in types)
                {
                    BehaviorNode node = ScriptableObject.CreateInstance(type) as BehaviorNode;
                    var nodeName = node.NodeName;
                    if (nodeName == "")
                        nodeName = type.Name;
                    evt.menu.AppendAction($"Composites/{nodeName}",
                        a => CreateBehaviorNode(type, a.eventInfo.localMousePosition));
                    ScriptableObject.DestroyImmediate(node);
                }
            }

            {
                var types = TypeCache.GetTypesDerivedFrom<BehaviorDecoratorNode>();
                foreach (var type in types)
                {
                    BehaviorNode node = ScriptableObject.CreateInstance(type) as BehaviorNode;
                    var nodeName = node.NodeName;
                    if (nodeName == "")
                        nodeName = type.Name;
                    evt.menu.AppendAction($"Decorators/{nodeName}",
                        a => CreateBehaviorNode(type, a.eventInfo.localMousePosition));
                    ScriptableObject.DestroyImmediate(node);
                }
            }
        }

        public void CreateBehaviorNode(System.Type type, Vector2 position)
        {
            BehaviorNode node = tree.CreateNode(type, position);
            OnNodeAdded?.Invoke(node);
            CreateNodeView(node);
        }

        private void CreateNodeView(BehaviorNode node)
        {
            NodeView nodeView = new NodeView(node);
            nodeView.OnNodeSelected = OnNodeSelected;
            nodeView.OnNodeSelected -= OnNodeSelectionChanged;
            nodeView.OnNodeSelected += OnNodeSelectionChanged;
            AddElement(nodeView);
        }

        private void OnNodeSelectionChanged(NodeView node)
        {
            selectedNode = node;
        }

        public void UpdateNodeStates()
        {
            nodes.ForEach(n =>
            {
                var view = n as NodeView;
                view.UpdateState();
            });
        }
    }
}

#endif
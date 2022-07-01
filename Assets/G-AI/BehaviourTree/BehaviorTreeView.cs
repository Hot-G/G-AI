#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class BehaviorTreeView : GraphView
{
    public class UxmlFactory : UxmlFactory<BehaviorTreeView, UxmlTraits> { }
    public Action<NodeView> OnNodeSelected;
    public Action OnNodeDeleted;
    private BehaviourTree tree;
    public BehaviorTreeView()
    {
        Insert(0, new GridBackground());
        
        
        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/AI/BehaviorTreeEditor.uss");
        styleSheets.Add(styleSheet);

        //Undo.undoRedoPerformed += OnUndoRedo;
    }

    private void OnUndoRedo()
    {
        PopulateView(tree);
        AssetDatabase.SaveAssets();
    }

    NodeView FindNodeView(BehaviourNode node) => GetNodeByGuid(node.guid) as NodeView;

    public void ClearView()
    {
        graphViewChanged -= OnGraphViewChanged;
        graphElements.ForEach(RemoveElement);
        graphViewChanged += OnGraphViewChanged;
    }

    public void PopulateView(BehaviourTree tree)
    {
        this.tree = tree;

        if (tree.rootNode == null)
        {
            tree.rootNode = tree.CreateNode(typeof(BehaviourRootNode)) as BehaviourRootNode;
            //EditorUtility.SetDirty(tree);
            AssetDatabase.SaveAssets();
        }
        
        //CREATE NODE VIEW
        tree.nodes.ForEach(CreateNodeView);
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
            }
        });

        graphViewChange.edgesToCreate?.ForEach(edge =>
        {
            NodeView parentView = edge.output.node as NodeView;
            NodeView childView = edge.input.node as NodeView;
            tree.AddChild(parentView.node, childView.node);
        });

        if (graphViewChange.movedElements != null)
        {
            nodes.ForEach(n =>
            {
                var view = n as NodeView;
                view.SortChildren();
            });
        }
        
        return graphViewChange;
    }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        {
            var types = TypeCache.GetTypesDerivedFrom<BehaviourActionNode>();
            foreach (var type in types)
            {
                BehaviourNode node = ScriptableObject.CreateInstance(type) as BehaviourNode;
                var nodeName = node.NodeName;
                if (nodeName == "")
                    nodeName = type.Name;
                evt.menu.AppendAction($"Actions/{nodeName}", a => CreateBehaviourNode(type));
                ScriptableObject.DestroyImmediate(node);
            }
        }
        
        {
            var types = TypeCache.GetTypesDerivedFrom<BehaviourCompositeNode>();
            foreach (var type in types)
            {
                BehaviourNode node = ScriptableObject.CreateInstance(type) as BehaviourNode;
                var nodeName = node.NodeName;
                if (nodeName == "")
                    nodeName = type.Name;
                evt.menu.AppendAction($"Composites/{nodeName}", (a) => CreateBehaviourNode(type));
                ScriptableObject.DestroyImmediate(node);
            }
        }
        
        {
            var types = TypeCache.GetTypesDerivedFrom<BehaviourDecoratorNode>();
            foreach (var type in types)
            {
                BehaviourNode node = ScriptableObject.CreateInstance(type) as BehaviourNode;
                var nodeName = node.NodeName;
                if (nodeName == "")
                    nodeName = type.Name;
                evt.menu.AppendAction($"Decorators/{nodeName}", (a) => CreateBehaviourNode(type));
                ScriptableObject.DestroyImmediate(node);
            }
        }
    }
    
    private void CreateBehaviourNode(System.Type type)
    {
        BehaviourNode node = tree.CreateNode(type);
        CreateNodeView(node);
    }

    private void CreateNodeView(BehaviourNode node)
    {
        NodeView nodeView = new NodeView(node);
        nodeView.OnNodeSelected = OnNodeSelected;
        AddElement(nodeView);
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
#endif

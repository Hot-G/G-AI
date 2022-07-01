#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

public class NodeView : Node
{
    public Action<NodeView> OnNodeSelected;
    public BehaviourNode node;
    public Port input;
    public Port output;
    
    public NodeView(BehaviourNode node) : base("Assets/Scripts/AI/NodeView.uxml")
    {
        this.node = node;
        this.title = node.NodeName;
        this.viewDataKey = node.guid;
        
        style.left = node.position.x;
        style.top = node.position.y;

        CreateInputPorts();
        CreateOutputPorts();
        SetupClasses();

        var nodeName = this.Q<Label>("nodeName");
        nodeName.bindingPath = "nodeName";
        nodeName.Bind(new SerializedObject(node));
    }

    private void SetupClasses()
    {
        if (node is BehaviourActionNode)
        {
            AddToClassList("action");
        }
        else if (node is BehaviourCompositeNode)
        {
            AddToClassList("composite");
        }
        else if (node is BehaviourDecoratorNode)
        {
            AddToClassList("decorator");
        }
        else if (node is BehaviourRootNode)
        {
            AddToClassList("root");
        }
    }


    private void CreateInputPorts()
    {
        if (node is BehaviourActionNode)
        {
            input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
        }
        else if (node is BehaviourCompositeNode)
        {
            input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
        }
        else if (node is BehaviourDecoratorNode)
        {
            input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
        }
        else if (node is BehaviourRootNode)
        {
        }

        if (input != null)
        {
            input.portName = "";
            input.style.flexDirection = FlexDirection.Column;
            inputContainer.Add(input);
        }
    }

    private void CreateOutputPorts()
    {
        if (node is BehaviourActionNode)
        {
        }
        else if (node is BehaviourCompositeNode)
        {
            output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(bool));
        }
        else if (node is BehaviourDecoratorNode)
        {
            output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
        }
        else if (node is BehaviourRootNode)
        {
            output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
        }
        
        if (output != null)
        {
            output.portName = "";
            output.style.flexDirection = FlexDirection.ColumnReverse;
            outputContainer.Add(output);
        }
    }

    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);
        //Undo.RecordObject(node, "Behaviour Tree (Set Position)");
        node.position.x = newPos.xMin;
        node.position.y = newPos.yMin;
        //EditorUtility.SetDirty(node);
    }

    public override void OnSelected()
    {
        base.OnSelected();
        OnNodeSelected?.Invoke(this);
    }

    public override void OnUnselected()
    {
        base.OnUnselected();
        OnNodeSelected?.Invoke(null);
    }

    public void SortChildren()
    {
        BehaviourCompositeNode composite = node as BehaviourCompositeNode;
        if (composite)
        {
            composite.children.Sort(SortByHorizontalPosition);
        }
    }

    private int SortByHorizontalPosition(BehaviourNode left, BehaviourNode right)
    {
        return left.position.x < right.position.x ? -1 : 1;
    }

    public void UpdateState()
    {
        RemoveFromClassList("running");
        RemoveFromClassList("failure");
        RemoveFromClassList("success");
        
        if (!Application.isPlaying) return;
        
        switch (node.state)
        {
            case BehaviourNode.State.Running:
                if (!node.isStarted) break;
                AddToClassList("running");
                break;
            case BehaviourNode.State.Failure:
                AddToClassList("failure");
                break;
            case BehaviourNode.State.Success:
                AddToClassList("success");
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    

}

#endif

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
    public BehaviorNode node;
    public Port input;
    public Port output;

    public NodeView(BehaviorNode node) : base("Assets/G-AI/BehaviourTree/NodeView.uxml")
    {
        this.node = node;
        this.title = node.NodeName;
        this.viewDataKey = node.guid;

        //style.left = node.position.x;
        //style.top = node.position.y;
        SetPosition(new Rect(node.position, Vector2.zero));
        
        CreateInputPorts();
        CreateOutputPorts();
        SetupClasses();

        var nodeName = this.Q<Label>("nodeName");
        nodeName.bindingPath = "nodeName";
        nodeName.Bind(new SerializedObject(node));
    }

    private void SetupClasses()
    {
        if (node is BehaviorActionNode)
        {
            AddToClassList("action");
        }
        else if (node is BehaviorCompositeNode)
        {
            AddToClassList("composite");
        }
        else if (node is BehaviorDecoratorNode)
        {
            AddToClassList("decorator");
        }
        else if (node is BehaviorRootNode)
        {
            AddToClassList("root");
        }
    }


    private void CreateInputPorts()
    {
        if (node is BehaviorActionNode)
        {
            input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
        }
        else if (node is BehaviorCompositeNode)
        {
            input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
        }
        else if (node is BehaviorDecoratorNode)
        {
            input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
        }
        else if (node is BehaviorRootNode)
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
        if (node is BehaviorActionNode)
        {
        }
        else if (node is BehaviorCompositeNode)
        {
            output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(bool));
        }
        else if (node is BehaviorDecoratorNode)
        {
            output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
        }
        else if (node is BehaviorRootNode)
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
        BehaviorCompositeNode composite = node as BehaviorCompositeNode;
        if (composite)
        {
            composite.children.Sort(SortByHorizontalPosition);
        }
    }

    private int SortByHorizontalPosition(BehaviorNode left, BehaviorNode right)
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
            case BehaviorNode.State.Running:
                if (!node.isStarted) break;
                AddToClassList("running");
                break;
            case BehaviorNode.State.Failure:
                AddToClassList("failure");
                break;
            case BehaviorNode.State.Success:
                AddToClassList("success");
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}

#endif
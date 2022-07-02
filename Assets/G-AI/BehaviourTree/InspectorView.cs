#if UNITY_EDITOR

using UnityEditor;
using UnityEngine.UIElements;

public class InspectorView : VisualElement
{
    public new class UxmlFactory : UxmlFactory<InspectorView, UxmlTraits> { }

    private Editor editor;
    public InspectorView()
    {
        
    }

    public void UpdateSelection(NodeView nodeView)
    {
        Clear();
        
        UnityEngine.Object.DestroyImmediate(editor);
        if (nodeView is null) return;
        editor = Editor.CreateEditor(nodeView.node);
        var container = new IMGUIContainer(() => { editor.OnInspectorGUI(); });
        Add(container);
    }
}

#endif

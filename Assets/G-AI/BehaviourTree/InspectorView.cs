#if UNITY_EDITOR

using UnityEditor;
using UnityEngine.UIElements;

namespace G_AI.BehaviourTree
{
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
            if (nodeView == null) return;
            editor = Editor.CreateEditor(nodeView.node);
            var container = new IMGUIContainer(() =>
            {
                if (editor == null) return;
                editor.OnInspectorGUI();
            });
            Add(container);
        }
    }
}

#endif

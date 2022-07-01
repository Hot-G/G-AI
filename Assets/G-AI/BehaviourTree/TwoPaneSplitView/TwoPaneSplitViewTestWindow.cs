using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEditor.UIElements.Builder
{
    class TwoPaneSplitViewTestWindow : EditorWindow
    {
        [MenuItem("UIElementsExamples/TwoPaneSplitViewTest")]
        static void ShowWindow()
        {
            var window = GetWindow<TwoPaneSplitViewTestWindow>();
            window.titleContent = new GUIContent("TwoPaneSplitViewTest");
            window.Show();
        }

        private void OnEnable()
        {
            var root = rootVisualElement;

            root.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/GAI/BehaviourTree/TwoPaneSplitView/TwoPaneSplitViewTestWindow.uss"));

            var xmlAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/GAI/BehaviourTree/TwoPaneSplitView/TwoPaneSplitViewTestWindow.uxml");
            //xmlAsset.CloneTree(root);
        }
    }
}
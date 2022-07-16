using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace G_AI.BehaviourTree
{
    public class NodeSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        private EditorWindow _window;
        private BehaviorTreeView _graphView;

        private Texture2D _indentationIcon;

        public void Configure(EditorWindow window,BehaviorTreeView graphView)
        {
            _window = window;
            _graphView = graphView;
        
            //Transparent 1px indentation icon as a hack
            _indentationIcon = new Texture2D(1,1);
            _indentationIcon.SetPixel(0,0,new Color(0,0,0,0));
            _indentationIcon.Apply();
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var tree = new List<SearchTreeEntry>
            {
                new SearchTreeGroupEntry(new GUIContent("Create Node"), 0)
                /*new SearchTreeEntry(new GUIContent("Comment Block", _indentationIcon))
            {
                level = 1,
                userData = new Group()
            } */
            };

            {
                tree.Add(new SearchTreeGroupEntry(new GUIContent("Action Nodes"), 1));
                var types = TypeCache.GetTypesDerivedFrom<BehaviorActionNode>();
                foreach (var type in types)
                {
                    BehaviorNode node = ScriptableObject.CreateInstance(type) as BehaviorNode;
                    var nodeName = node.NodeName;
                    if (nodeName == "")
                        nodeName = type.Name;
                    tree.Add(new SearchTreeEntry(new GUIContent(nodeName, _indentationIcon))
                    {
                        level = 2,
                        userData = type
                    });
                    ScriptableObject.DestroyImmediate(node);
                }
            }

            {
                tree.Add(new SearchTreeGroupEntry(new GUIContent("Composite Nodes"), 1));
                var types = TypeCache.GetTypesDerivedFrom<BehaviorCompositeNode>();
                foreach (var type in types)
                {
                    BehaviorNode node = ScriptableObject.CreateInstance(type) as BehaviorNode;
                    var nodeName = node.NodeName;
                    if (nodeName == "")
                        nodeName = type.Name;
                    tree.Add(new SearchTreeEntry(new GUIContent(nodeName, _indentationIcon))
                    {
                        level = 2,
                        userData = type
                    });
                    ScriptableObject.DestroyImmediate(node);
                }
            }
        
            {
                tree.Add(new SearchTreeGroupEntry(new GUIContent("Decorator Nodes"), 1));
                var types = TypeCache.GetTypesDerivedFrom<BehaviorDecoratorNode>();
                foreach (var type in types)
                {
                    BehaviorNode node = ScriptableObject.CreateInstance(type) as BehaviorNode;
                    var nodeName = node.NodeName;
                    if (nodeName == "")
                        nodeName = type.Name;
                    tree.Add(new SearchTreeEntry(new GUIContent(nodeName, _indentationIcon))
                    {
                        level = 2,
                        userData = type
                    });
                    ScriptableObject.DestroyImmediate(node);
                }
            }

            return tree;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            //Editor window-based mouse position
            var mousePosition = _window.rootVisualElement.ChangeCoordinatesTo(_window.rootVisualElement.parent,
                context.screenMousePosition - _window.position.position);
            var graphMousePosition = _graphView.contentViewContainer.WorldToLocal(mousePosition);
            switch (SearchTreeEntry.userData)
            {
                case Group group:
                    //var rect = new Rect(graphMousePosition, _graphView.DefaultCommentBlockSize);
                    //_graphView.CreateCommentBlock(rect);
                    return true;
            }
        
            _graphView.CreateBehaviorNode((Type)SearchTreeEntry.userData, Vector2.zero);
            return true;
        }
    }
}
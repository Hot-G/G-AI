#if UNITY_EDITOR

using UnityEngine.UIElements;

namespace G_AI.BehaviourTree
{
    public class SplitView : TwoPaneSplitView
    {
        public new class UxmlFactory : UxmlFactory<SplitView, UxmlTraits> { }
    }
}

#endif

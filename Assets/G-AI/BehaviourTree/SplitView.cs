#if UNITY_EDITOR

using UnityEngine.UIElements;

public class SplitView : TwoPaneSplitView
{
    public class UxmlFactory : UxmlFactory<SplitView, TwoPaneSplitView.UxmlTraits> { }
}

#endif

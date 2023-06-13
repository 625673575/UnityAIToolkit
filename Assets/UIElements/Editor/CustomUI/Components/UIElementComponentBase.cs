using UnityEngine;
using UnityEngine.UIElements;

public abstract class UIElementComponentBase
{
    protected VisualElement root;
    public UIElementComponentBase(VisualElement root)
    {
         this.root = root;
    }
}

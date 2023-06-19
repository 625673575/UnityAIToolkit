using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class TabView : VisualElement
{
    public new class UxmlFactory : UxmlFactory<TabView, UxmlTraits>
    {
    }
    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        private UxmlFloatAttributeDescription m_TabButtonHeight = new UxmlFloatAttributeDescription
        {
            name = "tab-button-height",
            defaultValue = k_DefaultTabButtonHeight
        }; public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            TabView scrollView = (TabView)ve;
            scrollView.tabButtonHeight = m_TabButtonHeight.GetValueFromBag(bag, cc);
        }
    }
    public const float k_DefaultTabButtonHeight = 24;
    public float tabButtonHeight;
    private List<Button> m_TabButtons;
    public List<Button> tabButtons => m_TabButtons;
    private List<VisualElement> m_TabContents;
    public List<VisualElement> tabContents => m_TabContents;
    private VisualElement m_ButtonParent;

    public TabView() : base()
    {
        m_ButtonParent = new VisualElement();
        m_ButtonParent.style.flexDirection = FlexDirection.Row;
        m_ButtonParent.style.justifyContent = Justify.Center;
        Add(m_ButtonParent);
        style.flexDirection = FlexDirection.Column;
    }

    public void Add(string tabName, VisualElement view)
    {
        m_TabContents ??= new List<VisualElement>();
        m_TabButtons ??= new List<Button>();
        Button button = new() { text = tabName };
        button.style.height = tabButtonHeight;
        button.style.borderBottomColor = Color.white;
        button.style.minWidth = 128;
        m_TabButtons.Add(button);
        m_TabContents.Add(view);
        button.RegisterCallback<ClickEvent>(e =>
        {
            SelectTab(button, view);
        });
        SelectTab(m_TabButtons.First(), m_TabContents.First());

        // add Child UI
        m_ButtonParent.Add(button);
        Add(view);
    }
    private void SelectTab(Button b, VisualElement view)
    {
        foreach (var tab in m_TabButtons)
        {
            if (tab == b)
            {
                tab.style.borderRightWidth = 1;
                tab.style.borderLeftWidth = 1;
                tab.style.borderTopWidth = 1;
                tab.style.borderBottomWidth = 1;
            }
            else
            {
                tab.style.borderRightWidth = 0;
                tab.style.borderLeftWidth = 0;
                tab.style.borderTopWidth = 0;
                tab.style.borderBottomWidth = 0;
            }
        }

        foreach (var tab in m_TabContents)
        {
            if (tab == view)
            {
                tab.style.display = DisplayStyle.Flex;
            }
            else
            {
                tab.style.display = DisplayStyle.None;
            }
        }
    }
}

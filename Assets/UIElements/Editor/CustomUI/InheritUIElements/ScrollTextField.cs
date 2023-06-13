using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ScrollTextField : ScrollView
{
    public new class UxmlFactory : UxmlFactory<ScrollTextField, UxmlTraits>
    {
    }
    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        private static readonly float k_DefaultScrollDecelerationRate = 0.135f;
        private static readonly float k_DefaultElasticity = 0.1f;
        private UxmlEnumAttributeDescription<ScrollViewMode> m_ScrollViewMode = new UxmlEnumAttributeDescription<ScrollViewMode>
        {
            name = "mode",
            defaultValue = ScrollViewMode.Vertical
        };

        private UxmlEnumAttributeDescription<NestedInteractionKind> m_NestedInteractionKind = new UxmlEnumAttributeDescription<NestedInteractionKind>
        {
            name = "nested-interaction-kind",
            defaultValue = NestedInteractionKind.Default
        };

        private UxmlBoolAttributeDescription m_ShowHorizontal = new UxmlBoolAttributeDescription
        {
            name = "show-horizontal-scroller"
        };

        private UxmlBoolAttributeDescription m_ShowVertical = new UxmlBoolAttributeDescription
        {
            name = "show-vertical-scroller"
        };

        private UxmlEnumAttributeDescription<ScrollerVisibility> m_HorizontalScrollerVisibility = new UxmlEnumAttributeDescription<ScrollerVisibility>
        {
            name = "horizontal-scroller-visibility"
        };

        private UxmlEnumAttributeDescription<ScrollerVisibility> m_VerticalScrollerVisibility = new UxmlEnumAttributeDescription<ScrollerVisibility>
        {
            name = "vertical-scroller-visibility"
        };

        private UxmlFloatAttributeDescription m_HorizontalPageSize = new UxmlFloatAttributeDescription
        {
            name = "horizontal-page-size",
            defaultValue = -1f
        };

        private UxmlFloatAttributeDescription m_VerticalPageSize = new UxmlFloatAttributeDescription
        {
            name = "vertical-page-size",
            defaultValue = -1f
        };

        private UxmlFloatAttributeDescription m_MouseWheelScrollSize = new UxmlFloatAttributeDescription
        {
            name = "mouse-wheel-scroll-size",
            defaultValue = 18f
        };

        private UxmlEnumAttributeDescription<TouchScrollBehavior> m_TouchScrollBehavior = new UxmlEnumAttributeDescription<TouchScrollBehavior>
        {
            name = "touch-scroll-type",
            defaultValue = TouchScrollBehavior.Clamped
        };

        private UxmlFloatAttributeDescription m_ScrollDecelerationRate = new UxmlFloatAttributeDescription
        {
            name = "scroll-deceleration-rate",
            defaultValue = k_DefaultScrollDecelerationRate
        };

        private UxmlFloatAttributeDescription m_Elasticity = new UxmlFloatAttributeDescription
        {
            name = "elasticity",
            defaultValue = k_DefaultElasticity
        };
        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            ScrollView scrollView = (ScrollView)ve;
            scrollView.mode = m_ScrollViewMode.GetValueFromBag(bag, cc);
            ScrollerVisibility value = ScrollerVisibility.Auto;
            if (m_HorizontalScrollerVisibility.TryGetValueFromBag(bag, cc, ref value))
            {
                scrollView.horizontalScrollerVisibility = value;
            }

            ScrollerVisibility value2 = ScrollerVisibility.Auto;
            if (m_VerticalScrollerVisibility.TryGetValueFromBag(bag, cc, ref value2))
            {
                scrollView.verticalScrollerVisibility = value2;
            }

            scrollView.nestedInteractionKind = m_NestedInteractionKind.GetValueFromBag(bag, cc);
            scrollView.horizontalPageSize = m_HorizontalPageSize.GetValueFromBag(bag, cc);
            scrollView.verticalPageSize = m_VerticalPageSize.GetValueFromBag(bag, cc);
            scrollView.mouseWheelScrollSize = m_MouseWheelScrollSize.GetValueFromBag(bag, cc);
            scrollView.scrollDecelerationRate = m_ScrollDecelerationRate.GetValueFromBag(bag, cc);
            scrollView.touchScrollBehavior = m_TouchScrollBehavior.GetValueFromBag(bag, cc);
            scrollView.elasticity = m_Elasticity.GetValueFromBag(bag, cc);
        }
    }
    public TextField textField { get; private set; }
    public ScrollTextField() : base(ScrollViewMode.VerticalAndHorizontal)
    {
        textField = new TextField() { value = "", label = "", focusable = true, multiline = true, pickingMode = PickingMode.Position };
        textField.AddManipulator(new ContextualMenuManipulator((ContextualMenuPopulateEvent evt) =>
        {
            evt.menu.AppendAction("Copy All", (x) =>
            {
                Debug.Log("Copy Text");
                GUIUtility.systemCopyBuffer = textField.value;
            });
            evt.menu.AppendAction("Paste All", (x) =>
            {
                Debug.Log("Paste All");
                textField.value = GUIUtility.systemCopyBuffer;
            });
            evt.menu.AppendAction("Clear", (x) =>
            {
                Debug.Log("Clear Text");
                textField.value = string.Empty;
            });
            evt.menu.AppendAction("Save", (x) =>
            {
                string assetPath = EditorUtility.SaveFilePanelInProject("save", "JsonStruct", "cs", "Save the code");
                if (!string.IsNullOrEmpty(assetPath))
                {
                    Debug.Log("Save to " + assetPath);
                    File.WriteAllText(assetPath, textField.value);
                }
            });
        }));
        textField.RegisterCallback<KeyDownEvent>(OnKeyDown);
        textField.style.minHeight = 512;
        Add(textField);
    }

    private void OnKeyDown(KeyDownEvent evt)
    {
        var target = evt.target as TextField;
        if (evt.ctrlKey && evt.keyCode == KeyCode.A)
        {
            target.Focus();
            target.SelectAll();
        }
        if (evt.ctrlKey && evt.keyCode == KeyCode.C)
        {
            GUIUtility.systemCopyBuffer = target.value;
        }
    }
}
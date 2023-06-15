using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ScrollGroupBox : ScrollView
{
    public new class UxmlFactory : UxmlFactory<ScrollGroupBox, UxmlTraits>
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
    public GroupBox groupBox { get; private set; }
    public ScrollGroupBox() : base(ScrollViewMode.VerticalAndHorizontal)
    {
        groupBox = new GroupBox() { focusable = true};
        groupBox.AddManipulator(new ContextualMenuManipulator((ContextualMenuPopulateEvent evt) =>
        {
            evt.menu.AppendAction("Save", (x) =>
            {
                save();
            });
        }));
        groupBox.RegisterCallback<KeyDownEvent>(OnKeyDown);
        groupBox.style.minHeight = 512;
        Add(groupBox);
    }
    private void save()
    {
        string assetPath = EditorUtility.SaveFilePanel("save", "Image", "png", "Save the image");
        if (!string.IsNullOrEmpty(assetPath))
        {
            Debug.Log("Save image to " + assetPath);
            Texture2D texture = groupBox.style.backgroundImage.value.texture ?? groupBox.style.backgroundImage.value.sprite.texture;
            if(texture != null && texture.isReadable)
                File.WriteAllBytes(assetPath, texture.EncodeToPNG());
        }
    }
    private void OnKeyDown(KeyDownEvent evt)
    {
        if (evt.ctrlKey && evt.keyCode == KeyCode.S)
        {
            save();
        }
    }
}
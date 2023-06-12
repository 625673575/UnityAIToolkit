using UnityEditor;
using System;
using System.IO;
using UnityEngine.UIElements;
using UnityEngine;
using System.Linq;

public class DragDropImage : Button
{
    public new class UxmlFactory : UxmlFactory<DragDropImage, UxmlTraits>
    {
    }
    public new class UxmlTraits : TextElement.UxmlTraits
    {
        public UxmlTraits()
        {
            base.focusable.defaultValue = true;
        }
        private UxmlBoolAttributeDescription m_resizeToImageSize = new UxmlBoolAttributeDescription
        {
            name = "resize-to-image-size",
            defaultValue = true
        };

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            DragDropImage of = ((DragDropImage)ve);
            of.text = IndicateText;
            of.resizeToImageSize = m_resizeToImageSize.GetValueFromBag(bag, cc);
        }
    }
    private readonly DragAndDropManipulator m_manipulator;
    private Action<Texture2D> m_onTextureLoadedEvent;
    private StyleColor m_backgroundColor;
    private const string IndicateText = "drag&drop image here or click to select";
    private readonly string[] supportedFormat = new string[] { "png", "jpeg", "jpg" };
    public bool resizeToImageSize { get; set; }
    public Texture2D m_value;
    public Texture2D value
    {
        get => m_value; set
        {
            m_value = value;
            style.backgroundImage = m_value;
        }
    }
    public DragDropImage() : this(null, null) { }
    public DragDropImage(Action clickEvent, Action<Texture2D> onTextureLoadedEvent, bool autoResizeToImageSize = true) : base(clickEvent)
    {
        m_manipulator = new DragAndDropManipulator(this, OnPerformDrop);
        m_onTextureLoadedEvent = onTextureLoadedEvent;
        resizeToImageSize = autoResizeToImageSize;
        text = IndicateText;
        style.minWidth = 256;
        style.minHeight = 256;
        style.maxWidth = 512;
        style.maxHeight = 512;
        m_backgroundColor = style.backgroundColor;
        clicked += () =>
       {
           string path = EditorUtility.OpenFilePanel("Image", "", string.Join(",", supportedFormat));
           if (string.IsNullOrEmpty(path) || !File.Exists(path))
               return;
           OnPerformDrop(path);
       };
    }
    ~DragDropImage() { m_manipulator?.target.RemoveManipulator(m_manipulator); }
    private void setSizeLessThan(float width, float height)
    {
        while (width > style.maxWidth.value.value || height > style.maxHeight.value.value)
        {
            width = width / 2;
            height = height / 2;
        }
        style.width = width;
        style.height = height;
    }
    public void RegisterOnLoadImage(Action<Texture2D> action)
    {
        m_onTextureLoadedEvent += action;
    }
    private void OnPerformDrop(string assetPath)
    {
        bool supportFormat = supportedFormat.Any(supportedFormat => assetPath.EndsWith(supportedFormat));
        if (!supportFormat)
        {
            EditorUtility.DisplayDialog("Error File Format", $"{Path.GetExtension(assetPath)} is not supported image", "OK");
            return;
        }
        Texture2D tex = new Texture2D(1, 1);
        bool succ = tex.LoadImage(File.ReadAllBytes(assetPath));
        if (succ)
        {
            tex.Apply();

            value = tex;
            style.backgroundColor = Color.white;
            if (resizeToImageSize)
            {
                setSizeLessThan(tex.width, tex.height);
            }

            m_onTextureLoadedEvent?.Invoke(tex);
            text = "";
        }
        else
        {
            if (value == null)
            {
                text = IndicateText;
                style.backgroundColor = m_backgroundColor;
            }
        }
    }
}

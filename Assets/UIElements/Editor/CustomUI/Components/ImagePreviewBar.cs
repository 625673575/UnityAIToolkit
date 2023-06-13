using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ImagePreviewBar : UIElementComponentBase
{
    protected GroupBox imageBox;
    protected ScrollView scrollGrid;
    public List<Texture2D> images;
    protected List<Button> imageButtons;
    public float thumbnailWidth { protected set; get; }
    public float thumbnailHeight { protected set; get; }
    public void SetThumbnailSize(float width = 128, float height = 128)
    {
        thumbnailHeight = height;
        thumbnailWidth = width;
        foreach (var button in imageButtons)
        {
            button.style.width = thumbnailWidth;
            button.style.height = thumbnailHeight;
        }
    }
    public ImagePreviewBar(VisualElement root) : base(root)
    {
        thumbnailHeight = 128;
        thumbnailWidth = 128;
        imageBox = root.Q<GroupBox>(nameof(imageBox));
        scrollGrid = root.Q<ScrollView>(nameof(scrollGrid));
        imageButtons = new List<Button>();
        images = new List<Texture2D>();
    }
    private Button createButton(Texture2D image)
    {
        Button button = new Button() { focusable = true };
        button.userData = image;
        button.text = string.Empty;

        button.style.width = thumbnailWidth;
        button.style.height = thumbnailHeight;
        button.style.alignSelf = Align.Center;

        button.style.backgroundImage = image;
        button.RegisterCallback<ClickEvent>(OnButtonClicked);
        button.Focus();
        return button;
    }

    private void OnButtonClicked(ClickEvent evt)
    {
        Button target = evt.target as Button;
        Texture2D image = target.userData as Texture2D;
        if (image != null)
        {
            imageBox.style.backgroundImage = image;
        }
    }

    public void Add(Texture2D image)
    {
        if (image == null || images.Contains(image))
            return;
        images.Add(image);
        Button button = createButton(image);
        imageButtons.Add(button);
        scrollGrid.Add(button);
        imageBox.style.backgroundImage = image;
        scrollGrid.ScrollTo(button);
        button.Focus();
    }
    public void Remove(Texture2D image)
    {
        int index = images.IndexOf(image);
        if (index != -1)
            return;
        images.RemoveAt(index);
        imageButtons.RemoveAt(index);
        scrollGrid.RemoveAt(index);
    }
    public void Clear()
    {
        images.Clear();
        imageButtons.Clear();
        scrollGrid.Clear();
    }
}

using StableDiffusion;
using System;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class ExtraWindow : EditorWindow
{
    public VisualTreeAsset MainWindow;
    private GroupBox imageBox;
    private ObjectField inputTexture;
    private IntegerField resizeModeIntegerField;
    private FloatField upscalingFloatField;
    private Button img2ImgButton;

    private void CreateGUI()
    {
        var window = MainWindow.Instantiate();
        rootVisualElement.Add(window);
        imageBox = window.Q<GroupBox>(nameof(imageBox));

        inputTexture = window.Q<ObjectField>(nameof(inputTexture));
        img2ImgButton = window.Q<Button>(nameof(img2ImgButton));
        img2ImgButton.RegisterCallback<ClickEvent>(OnTxt2ImgClicked);

        resizeModeIntegerField = window.Q<IntegerField>(nameof(resizeModeIntegerField));
        upscalingFloatField = window.Q<FloatField>(nameof(upscalingFloatField));
    }

    private void OnTxt2ImgClicked(ClickEvent evt)
    {
        if (inputTexture.value == null)
        {
            EditorUtility.DisplayDialog("Error", "Please select input texture", "OK");
            return;
        }
        Texture2D image = inputTexture.value as Texture2D;
        ExtrasPayload payload = new()
        {
            image = Convert.ToBase64String(image.EncodeToPNG()),
            resize_mode = resizeModeIntegerField.value,
            upscaling_resize = upscalingFloatField.value,
            upscaler_1 = "None"
        };
        UnityEvent<Texture2D> receiveTexEvent = new();
        receiveTexEvent.AddListener(OnReceiveTexture2D);
        UnityEvent<Texture2D>[] receiveTexEvents = { receiveTexEvent };
        this.StartCoroutine(ImageExtra.ProcessExtraCoroutine(payload, new Texture2D[] { image }, receiveTexEvents));
    }

    void OnReceiveTexture2D(Texture2D texture)
    {
        imageBox.style.backgroundImage = texture;
        imageBox.style.backgroundSize = new StyleBackgroundSize(new BackgroundSize(texture.width,texture.height));
        imageBox.style.height = texture.height;
        imageBox.style.width = texture.width;
    }
    private void OnDestroy()
    {
        img2ImgButton.UnregisterCallback<ClickEvent>(OnTxt2ImgClicked);
    }
}

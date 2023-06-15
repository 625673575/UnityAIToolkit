using StableDiffusion;
using System;
using System.Linq;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class ExtraWindow : EditorWindow
{
    public VisualTreeAsset MainWindow;
    private ObjectField setupAssetObjectField;
    private ScrollGroupBox imageBox;
    private DragDropImage inputTexture;
    private IntegerField resizeModeIntegerField;
    private FloatField upscalingFloatField;
    private Button img2ImgButton;
    private DropdownField upscalingOptionsDropDownField;
    private Button refreshUpscalingOptionsButton;
    private ProgressBar progressBar;

    private void CreateGUI()
    {
        var window = MainWindow.Instantiate();
        rootVisualElement.Add(window);
        setupAssetObjectField = window.Q<ObjectField>(nameof(setupAssetObjectField));
        imageBox = window.Q<ScrollGroupBox>(nameof(imageBox));

        inputTexture = window.Q<DragDropImage>(nameof(inputTexture));
        img2ImgButton = window.Q<Button>(nameof(img2ImgButton));
        img2ImgButton.RegisterCallback<ClickEvent>(OnExtraClicked);

        resizeModeIntegerField = window.Q<IntegerField>(nameof(resizeModeIntegerField));
        upscalingFloatField = window.Q<FloatField>(nameof(upscalingFloatField));

        upscalingOptionsDropDownField = window.Q<DropdownField>(nameof(upscalingOptionsDropDownField));
        refreshUpscalingOptionsButton = window.Q<Button>(nameof(refreshUpscalingOptionsButton));
        refreshUpscalingOptionsButton.RegisterCallback<ClickEvent>(OnRefreshDownscalingOptionsClicked);
        progressBar = window.Q<ProgressBar>(nameof(progressBar));
    }

    private void OnRefreshDownscalingOptionsClicked(ClickEvent evt)
    {
        LaunchSetup setup = setupAssetObjectField.value as LaunchSetup;
        this.StartCoroutine(GetInfo.ProcessGetInfoCoroutine(setup.address, GetInfo.ApiInfo["upscalers"], "", (json) =>
        {
            try
            {
                if (json.StartsWith('['))
                {
                    json = "{\"array\":" + json + "}";
                }
                UpscalingOptions options = JsonUtility.FromJson<UpscalingOptions>(json);
                var optionsName = options.array.Select(x => x.name).ToList();
                upscalingOptionsDropDownField.choices = optionsName;
                if (optionsName.Count > 0)
                    upscalingOptionsDropDownField.value = optionsName.FirstOrDefault();
            }
            catch
            {
                Debug.LogError("Can't get upscaling options!");
            }
        }));
    }

    private void OnExtraClicked(ClickEvent evt)
    {
        if (setupAssetObjectField.value == null)
        {
            EditorUtility.DisplayDialog("Error", "Please select input texture", "OK");
            return;
        }
        var setupAsset = setupAssetObjectField.value as LaunchSetup;
        if (inputTexture.value == null)
        {
            EditorUtility.DisplayDialog("Error", "Please select input texture", "OK");
            return;
        }
        Texture2D image = inputTexture.value;
        ExtrasPayload payload = new()
        {
            image = Convert.ToBase64String(image.EncodeToPNG()),
            resize_mode = resizeModeIntegerField.value,
            upscaling_resize = upscalingFloatField.value,
            upscaler_1 = upscalingOptionsDropDownField.value
        };
        UnityEvent<Texture2D> receiveTexEvent = new();
        receiveTexEvent.AddListener(OnReceiveTexture2D);
        UnityEvent<Texture2D>[] receiveTexEvents = { receiveTexEvent };
        this.StartCoroutine(ImageExtra.ProcessExtraCoroutine(setupAsset, payload, new Texture2D[] { image }, receiveTexEvents));
        // this.StartCoroutine(GetInfo.GetProcessInfoCoroutine(setupAsset.address, (progress) => progressBar.value = (float)progress.progress));
    }

    void OnReceiveTexture2D(Texture2D texture)
    {
        imageBox.groupBox.style.backgroundImage = texture;
        imageBox.groupBox.style.backgroundSize = new StyleBackgroundSize(new BackgroundSize(texture.width, texture.height));
        imageBox.groupBox.style.height = texture.height;
        imageBox.groupBox.style.width = texture.width;
    }
    private void OnDestroy()
    {
        img2ImgButton?.UnregisterCallback<ClickEvent>(OnExtraClicked);
    }
}

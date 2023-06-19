using StableDiffusion;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class Txt2ImgWindow : EditorWindow
{
    public VisualTreeAsset MainWindow;
    private ObjectField setupAssetObjectField;
    public ImagePreviewBar imagePreviewBar;
    private Button txt2ImgButton;
    private TextField promptText, negativePromptText;
    private SliderInt stepSliderInt;
    private Slider denoisingStrengthSlider;
    private Toggle resolutionToggle;
    private ProgressBar progressBar;

    private void CreateGUI()
    {
        var window = MainWindow.Instantiate();
        rootVisualElement.Add(window);
        setupAssetObjectField = window.Q<ObjectField>(nameof(setupAssetObjectField));
        imagePreviewBar = new ImagePreviewBar(window.Q(nameof(imagePreviewBar)));
        txt2ImgButton = window.Q<Button>(nameof(txt2ImgButton));
        txt2ImgButton.RegisterCallback<ClickEvent>(OnTxt2ImgClicked);

        promptText = window.Q<TextField>(nameof(promptText));
        negativePromptText = window.Q<TextField>(nameof(negativePromptText));
        stepSliderInt = window.Q<SliderInt>(nameof(stepSliderInt));
        denoisingStrengthSlider = window.Q<Slider>(nameof(denoisingStrengthSlider));
        resolutionToggle = window.Q<Toggle>(nameof(resolutionToggle));
        progressBar = window.Q<ProgressBar>(nameof(progressBar));
    }
    private void OnTxt2ImgClicked(ClickEvent evt)
    {
        if (setupAssetObjectField.value == null)
        {
            EditorUtility.DisplayDialog("Error", "Please select input texture", "OK");
            return;
        }
        var setupAsset = setupAssetObjectField.value as LaunchSetup;
        Txt2ImgPayload payload = new()
        {
            prompt = promptText.value,
            negative_prompt = negativePromptText.value,
            steps = stepSliderInt.value,
            denoising_strength = denoisingStrengthSlider.value,
            height = resolutionToggle.value ? 768 :512,
            width = resolutionToggle.value ? 768 : 512,

        };
        UnityEvent<Texture2D> receiveTexEvent = new();
        receiveTexEvent.AddListener(OnReceiveTexture2D);
        UnityEvent<Texture2D>[] receiveTexEvents = { receiveTexEvent };
        this.StartCoroutine(Text2Image.GenerateImagesCoroutine(setupAsset, payload, receiveTexEvents));
        this.StartCoroutine(GetInfo.GetProcessInfoCoroutine(setupAsset.address,(progress)=>progressBar.value = (float)progress.progress));
    }

    void OnReceiveTexture2D(Texture2D texture)
    {
        imagePreviewBar.Add(texture);
    }
    private void OnDestroy()
    {
        txt2ImgButton?.UnregisterCallback<ClickEvent>(OnTxt2ImgClicked);
    }
}

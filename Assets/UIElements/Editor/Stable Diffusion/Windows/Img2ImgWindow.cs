using StableDiffusion;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class Img2ImgWindow : EditorWindow
{
    public VisualTreeAsset MainWindow;
    public ImagePreviewBar imagePreviewBar;
    private DragDropImage inputTexture;
    private Button img2ImgButton;
    private TextField promptText,negativePromptText;
    private SliderInt stepSliderInt;
    private Slider denoisingStrengthSlider;

    private void CreateGUI()
    {
        var window = MainWindow.Instantiate();
        rootVisualElement.Add(window);

        imagePreviewBar = new ImagePreviewBar(window.Q(nameof(imagePreviewBar)));
        inputTexture = window.Q<DragDropImage>(nameof(inputTexture));
        img2ImgButton = window.Q<Button>(nameof(img2ImgButton));
        img2ImgButton.RegisterCallback<ClickEvent>(OnTxt2ImgClicked);

        promptText = window.Q<TextField>(nameof(promptText));
        negativePromptText = window.Q<TextField>(nameof(negativePromptText));
        stepSliderInt = window.Q<SliderInt>(nameof(stepSliderInt));
        denoisingStrengthSlider = window.Q<Slider>(nameof(denoisingStrengthSlider));
    }

    private void OnTxt2ImgClicked(ClickEvent evt)
    {
        if (inputTexture.value == null)
        {
            EditorUtility.DisplayDialog("Error","Please select input texture","OK");
            return;
        }
        Img2ImgPayload payload = new()
        {
            images = new Texture2D[] { inputTexture.value},
            prompt = promptText.value,
            negative_prompt = negativePromptText.value,
            steps = stepSliderInt.value,
            denoising_strength = denoisingStrengthSlider.value,
        };
        UnityEvent<Texture2D> receiveTexEvent = new();
        receiveTexEvent.AddListener(OnReceiveTexture2D);
        UnityEvent<Texture2D>[] receiveTexEvents = { receiveTexEvent };
        this.StartCoroutine(Image2Image.GenerateImagesCoroutine(payload, receiveTexEvents));
    }

    void OnReceiveTexture2D(Texture2D texture)
    {
        imagePreviewBar.Add(texture);
    }
    private void OnDestroy()
    {
        img2ImgButton.UnregisterCallback<ClickEvent>(OnTxt2ImgClicked);
    }
}

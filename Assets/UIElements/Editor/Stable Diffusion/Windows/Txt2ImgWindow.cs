using StableDiffusion;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class Txt2ImgWindow : EditorWindow
{
    public VisualTreeAsset MainWindow;
    private GroupBox imageBox;
    private Button txt2ImgButton;
    private TextField promptText,negativePromptText;
    private SliderInt stepSliderInt;
    private Slider denoisingStrengthSlider;
    
    private void CreateGUI()
    {
        var window = MainWindow.Instantiate();
        rootVisualElement.Add(window);
        imageBox = window.Q<GroupBox>(nameof(imageBox));
        txt2ImgButton = window.Q<Button>(nameof(txt2ImgButton));
        txt2ImgButton.RegisterCallback<ClickEvent>(OnTxt2ImgClicked);

        promptText = window.Q<TextField>(nameof(promptText));
        negativePromptText = window.Q<TextField>(nameof(negativePromptText));
        stepSliderInt = window.Q<SliderInt>(nameof(stepSliderInt));
        denoisingStrengthSlider = window.Q<Slider>(nameof(denoisingStrengthSlider));
    }

    private void OnTxt2ImgClicked(ClickEvent evt)
    {
        Txt2ImgPayload payload = new()
        {
            prompt = promptText.value,
            negative_prompt = negativePromptText.value,
            steps = stepSliderInt.value,
            denoising_strength = denoisingStrengthSlider.value,
        };
        UnityEvent<Texture2D> receiveTexEvent = new();
        receiveTexEvent.AddListener(OnReceiveTexture2D);
        UnityEvent<Texture2D>[] receiveTexEvents = { receiveTexEvent };
        this.StartCoroutine(Text2Image.GenerateImagesCoroutine(payload, receiveTexEvents));
    }

    void OnReceiveTexture2D(Texture2D texture)
    {
        imageBox.style.backgroundImage = texture;
    }
    private void OnDestroy()
    {
        txt2ImgButton?.UnregisterCallback<ClickEvent>(OnTxt2ImgClicked);
    }
}

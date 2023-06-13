using System;
using System.Text;
using UnityEditor;
using UnityEngine.UIElements;
using Xamasoft.JsonClassGenerator.CodeWriters;
using Xamasoft.JsonClassGenerator;
using Xamasoft.JsonClassGenerator.Models;
using Xamasoft.JsonClassGenerator.CodeWriterConfiguration;

public class JsonToCSharpWindow : EditorWindow
{
    public VisualTreeAsset MainWindow;
    public ScrollTextField jsonInputTextField;
    public ScrollTextField csharpOutputTextField;
    public Toggle usePascalCase;
    public Toggle optMemberProps;
    public Toggle optTypesImmutablePoco;
    public Toggle optTypesMutablePoco;

    private bool preventReentrancy = false;
    private void CreateGUI()
    {
        var window = MainWindow.Instantiate();
        rootVisualElement.Add(window);

        jsonInputTextField = window.Q<ScrollTextField>(nameof(jsonInputTextField));
        csharpOutputTextField = window.Q<ScrollTextField>(nameof(csharpOutputTextField));
        usePascalCase = window.Q<Toggle>(nameof(usePascalCase));
        optMemberProps = window.Q<Toggle>(nameof(optMemberProps));
        optTypesImmutablePoco = window.Q<Toggle>(nameof(optTypesImmutablePoco));
        optTypesMutablePoco = window.Q<Toggle>(nameof(optTypesMutablePoco));
        jsonInputTextField.textField.RegisterValueChangedCallback(ev => { GenerateCode(); });
        var toggles = new Toggle[] { usePascalCase, optMemberProps, optTypesImmutablePoco, optTypesMutablePoco };
        foreach (var toggle in toggles)
        {
            toggle.RegisterValueChangedCallback(ev => { GenerateCode(); });
        }
    }

    private void GenerateCode()
    {
        if (preventReentrancy) return;
        preventReentrancy = true;
        try
        {
            jsonInputTextField.textField.value = jsonInputTextField.textField.value.RepairLineBreaks();

            GenerateCSharp();
        }
        finally
        {
            preventReentrancy = false;
        }
    }

    public CSharpCodeWriterConfig writerConfig { get; set; } = new CSharpCodeWriterConfig();
    private void ConfigureGenerator(JsonClassGenerator config)
    {
        writerConfig.UsePascalCase = usePascalCase.value;
        writerConfig.AttributeLibrary = JsonLibrary.NewtonsoftJson;

        if (optMemberProps.value)
        {
            writerConfig.OutputMembers = OutputMembers.AsProperties;
        }
        else
        {
            writerConfig.OutputMembers = OutputMembers.AsPublicFields;
        }

        if (optTypesImmutablePoco.value)
        {
            writerConfig.OutputType = OutputTypes.ImmutableClass;
        }
        else if (optTypesMutablePoco.value)
        {
            writerConfig.OutputType = OutputTypes.MutableClass;
        }
        else
        {
            writerConfig.OutputType = OutputTypes.ImmutableRecord;
        }
    }
    private void GenerateCSharp()
    {
        string jsonText = jsonInputTextField.textField.value;
        if (string.IsNullOrWhiteSpace(jsonText))
        {
            csharpOutputTextField.textField.value = string.Empty;
            return;
        }

        JsonClassGenerator generator = new JsonClassGenerator();
        generator.CodeWriter = new CSharpCodeWriter(writerConfig);
        ConfigureGenerator(generator);

        try
        {
            StringBuilder sb = generator.GenerateClasses(jsonText, errorMessage: out string errorMessage);
            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                csharpOutputTextField.textField.value = "Error:\r\n" + errorMessage;
            }
            else
            {
                csharpOutputTextField.textField.value = sb.ToString();
            }
        }
        catch (Exception ex)
        {
            csharpOutputTextField.textField.value = "Error:\r\n" + ex.ToString();
        }
    }
}

using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class FileDialogTextField : TextField
{
    public enum FileDialogType
    {
        OpenFile,
        OpenFolder,
        SaveFile,
        SaveFileInProject,
        SaveFolder
    }
    public new class UxmlFactory : UxmlFactory<FileDialogTextField, UxmlTraits>
    {
    }
    public new class UxmlTraits : TextInputBaseField<string>.UxmlTraits
    {
        private static readonly UxmlStringAttributeDescription k_Value = new UxmlStringAttributeDescription
        {
            name = "value",
            obsoleteNames = new string[1] { "text" }
        };

        private UxmlBoolAttributeDescription m_Multiline = new UxmlBoolAttributeDescription
        {
            name = "multiline"
        };
        private UxmlEnumAttributeDescription<FileDialogType> m_FileDialogType = new UxmlEnumAttributeDescription<FileDialogType>()
        {
            name = "file-dialog-type"
        };
        private static readonly UxmlStringAttributeDescription k_FileExtension = new UxmlStringAttributeDescription
        {
            name = "file-extension",
        };

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            FileDialogTextField textField = (FileDialogTextField)ve;
            textField.fileExtension = k_FileExtension.GetValueFromBag(bag, cc);
            textField.fileDialogType = m_FileDialogType.GetValueFromBag(bag, cc);
            textField.multiline = m_Multiline.GetValueFromBag(bag, cc);
            base.Init(ve, bag, cc);
            string value = string.Empty;
            if (k_Value.TryGetValueFromBag(bag, cc, ref value))
            {
                textField.SetValueWithoutNotify(value);
            }
        }
    }
    public FileDialogType fileDialogType { get; private set; }
    public string fileExtension { get; private set; }
    public const string editorPrefKey = "file-dialog-text-field";
    public FileDialogTextField()
    : this(null)
    {
    }
    public FileDialogTextField(int maxLength, bool multiline, bool isPasswordField, char maskChar)
            : this(null, maxLength, multiline, isPasswordField, maskChar)
    {
    }
    public FileDialogTextField(string label)
        : this(label, -1, multiline: false, isPasswordField: false, '*')
    {
    }
    public FileDialogTextField(string label, int maxLength, bool multiline, bool isPasswordField, char maskChar) : base(label, maxLength, multiline, isPasswordField, maskChar)
    {
        Button button = new Button() { text = "Choose" };
        button.style.width = 64;
        string defaultPath = EditorPrefs.GetString(editorPrefKey);
        button.RegisterCallback<ClickEvent>(e =>
        {
            switch (fileDialogType)
            {
                case FileDialogType.OpenFile:
                    value = EditorUtility.OpenFilePanel("open file", defaultPath, fileExtension);
                    if(!string.IsNullOrEmpty(value))
                        EditorPrefs.SetString(editorPrefKey,Path.GetDirectoryName(value));
                    break;
                case FileDialogType.OpenFolder:
                    value = EditorUtility.OpenFolderPanel("open file", defaultPath, fileExtension);
                    if (!string.IsNullOrEmpty(value))
                        EditorPrefs.SetString(editorPrefKey, value);
                    break;
                case FileDialogType.SaveFile:
                    value = EditorUtility.SaveFilePanel("save file", defaultPath, "", fileExtension);
                    if (!string.IsNullOrEmpty(value))
                        EditorPrefs.SetString(editorPrefKey, Path.GetDirectoryName(value));
                    break;
                case FileDialogType.SaveFileInProject:
                    value = EditorUtility.SaveFilePanelInProject("save file", defaultPath, "", fileExtension);
                    if (!string.IsNullOrEmpty(value))
                        EditorPrefs.SetString(editorPrefKey, Path.GetDirectoryName(value));
                    break;
                case FileDialogType.SaveFolder:
                    value = EditorUtility.SaveFolderPanel("save file", defaultPath, fileExtension);
                    if (!string.IsNullOrEmpty(value))
                        EditorPrefs.SetString(editorPrefKey, value);
                    break;
            }
        });
        Add(button);
    }
}

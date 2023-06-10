using System.Linq;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace StableDiffusion
{
    public class GetInfoWindow : EditorWindow
    {
        public VisualTreeAsset MainWindow;
        private GroupBox imageBox;
        private ObjectField setupAssetObjectField;
        private DropdownField apiDropdownField;
        private Button getInfoButton;
        private TextField infoTextField;

        private void CreateGUI()
        {
            var window = MainWindow.Instantiate();
            rootVisualElement.Add(window);
            imageBox = window.Q<GroupBox>(nameof(imageBox));

            setupAssetObjectField = window.Q<ObjectField>(nameof(setupAssetObjectField));
            apiDropdownField = window.Q<DropdownField>(nameof(apiDropdownField));
            apiDropdownField.choices = GetInfo.ApiInfo.Keys.ToList();
            apiDropdownField.value = apiDropdownField.choices.FirstOrDefault();
            getInfoButton = window.Q<Button>(nameof(getInfoButton));
            getInfoButton.RegisterCallback<ClickEvent>(OnGetInfoClicked);
            infoTextField = window.Q<TextField>(nameof(infoTextField));
        }

        private void OnGetInfoClicked(ClickEvent evt)
        {
            if (setupAssetObjectField.value == null)
            {
                EditorUtility.DisplayDialog("Error", "Please select input texture", "OK");
                return;
            }
            var setupAsset = setupAssetObjectField.value as LaunchSetup;
            this.StartCoroutine(GetInfo.ProcessGetInfoCoroutine(setupAsset.address, GetInfo.ApiInfo[apiDropdownField.value], "", OnGetInfo));
        }

        private void OnGetInfo(string json)
        {
            infoTextField.value = json;
        }

        private void OnDestroy()
        {
            getInfoButton.UnregisterCallback<ClickEvent>(OnGetInfoClicked);
        }
    }
}
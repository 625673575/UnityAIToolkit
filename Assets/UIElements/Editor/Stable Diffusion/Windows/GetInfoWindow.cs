using System.Linq;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System.Collections;
using JsonPrettyPrinterPlus;

namespace StableDiffusion
{
    public class GetInfoWindow : EditorWindow
    {
        public VisualTreeAsset MainWindow;
        private ObjectField setupAssetObjectField;
        private DropdownField apiDropdownField;
        private Button getInfoButton;
        private Button postInfoButton;
        private ScrollTextField infoTextField;
        private TreeView treeView;
        private void CreateGUI()
        {
            var window = MainWindow.Instantiate();
            rootVisualElement.Add(window);

            setupAssetObjectField = window.Q<ObjectField>(nameof(setupAssetObjectField));
            apiDropdownField = window.Q<DropdownField>(nameof(apiDropdownField));
            apiDropdownField.choices = GetInfo.ApiInfo.Keys.ToList();
            apiDropdownField.value = apiDropdownField.choices.FirstOrDefault();
            getInfoButton = window.Q<Button>(nameof(getInfoButton));
            postInfoButton = window.Q<Button>(nameof(postInfoButton));
            getInfoButton.RegisterCallback<ClickEvent>(OnGetInfoClicked);
            postInfoButton.RegisterCallback<ClickEvent>(OnPostInfoClicked);
            infoTextField = window.Q<ScrollTextField>(nameof(infoTextField));
            treeView = window.Q<TreeView>();
        }

        private void OnPostInfoClicked(ClickEvent evt)
        {
            if (setupAssetObjectField.value == null)
            {
                EditorUtility.DisplayDialog("Error", "Please select Launch Setup Asset", "OK");
                return;
            }
            var setupAsset = setupAssetObjectField.value as LaunchSetup;
            this.StartCoroutine(GetInfo.ProcessGetInfoCoroutine(setupAsset.address, GetInfo.ApiInfo[apiDropdownField.value], infoTextField.textField.value, OnGetInfo, true));

        }

        private void OnGetInfoClicked(ClickEvent evt)
        {
            if (setupAssetObjectField.value == null)
            {
                EditorUtility.DisplayDialog("Error", "Please select Launch Setup Asset", "OK");
                return;
            }
            var setupAsset = setupAssetObjectField.value as LaunchSetup;
            this.StartCoroutine(GetInfo.ProcessGetInfoCoroutine(setupAsset.address, GetInfo.ApiInfo[apiDropdownField.value], "", OnGetInfo));
        }

        private void OnGetInfo(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return;
            json = treeView.LoadJson(json, apiDropdownField.value, true, OnTreeItemSelect);
            try
            {
                json = json.PrettyPrintJson();
                GUIUtility.systemCopyBuffer = json;
                infoTextField.textField.value = json;
            }
            catch
            {
                infoTextField.textField.value = string.Empty;
            }

        }

        private void OnTreeItemSelect(object obj)
        {
            IList objList = (IList)obj;
            foreach (var item in objList) { Debug.Log(item); }

        }

        private void OnDestroy()
        {
            getInfoButton?.UnregisterCallback<ClickEvent>(OnGetInfoClicked);
        }
    }
}
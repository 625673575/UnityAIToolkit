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
        #region get
        private GroupBox getGroupBox;
        private DropdownField apiGetDropdownField;
        private Button getInfoButton;
        private ScrollTextField infoGetTextField;
        private TreeView getJsonTreeView;
        #endregion

        #region post
        private GroupBox postGroupBox;
        private DropdownField apiPostDropdownField;
        private Button postInfoButton;
        private ScrollTextField infoPostTextField;
        private TreeView postJsonTreeView;
        #endregion

        private TabView tabView;
        private void CreateGUI()
        {
            var window = MainWindow.Instantiate();
            rootVisualElement.Add(window);

            setupAssetObjectField = window.Q<ObjectField>(nameof(setupAssetObjectField));

            //get
            getGroupBox = window.Q<GroupBox>(nameof(getGroupBox));
            apiGetDropdownField = window.Q<DropdownField>(nameof(apiGetDropdownField));
            apiGetDropdownField.choices = GetInfo.ApiGet.Keys.ToList();
            apiGetDropdownField.value = apiGetDropdownField.choices.FirstOrDefault();
            getInfoButton = window.Q<Button>(nameof(getInfoButton));
            getInfoButton.RegisterCallback<ClickEvent>(OnGetInfoClicked);
            infoGetTextField = window.Q<ScrollTextField>(nameof(infoGetTextField));
            getJsonTreeView = window.Q<TreeView>(nameof(getJsonTreeView));

            //post
            postGroupBox = window.Q<GroupBox>(nameof(postGroupBox));
            apiPostDropdownField = window.Q<DropdownField>(nameof(apiPostDropdownField));
            apiPostDropdownField.choices = GetInfo.ApiPost.Keys.ToList();
            apiPostDropdownField.value = apiPostDropdownField.choices.FirstOrDefault();
            postInfoButton = window.Q<Button>(nameof(postInfoButton));
            postInfoButton.RegisterCallback<ClickEvent>(OnPostInfoClicked);
            infoPostTextField = window.Q<ScrollTextField>(nameof(infoPostTextField));
            postJsonTreeView = window.Q<TreeView>(nameof(postJsonTreeView));

            tabView = window.Q<TabView>(nameof(tabView));
            tabView.Add("Get", getGroupBox);
            tabView.Add("Post", postGroupBox);
        }

        private void OnPostInfoClicked(ClickEvent evt)
        {
            if (setupAssetObjectField.value == null)
            {
                EditorUtility.DisplayDialog("Error", "Please select Launch Setup Asset", "OK");
                return;
            }
            var setupAsset = setupAssetObjectField.value as LaunchSetup;
            this.StartCoroutine(GetInfo.ProcessGetInfoCoroutine(setupAsset.address, GetInfo.ApiPost[apiPostDropdownField.value], infoPostTextField.textField.value, OnGetInfo, true));

        }

        private void OnGetInfoClicked(ClickEvent evt)
        {
            if (setupAssetObjectField.value == null)
            {
                EditorUtility.DisplayDialog("Error", "Please select Launch Setup Asset", "OK");
                return;
            }
            var setupAsset = setupAssetObjectField.value as LaunchSetup;
            this.StartCoroutine(GetInfo.ProcessGetInfoCoroutine(setupAsset.address, GetInfo.ApiGet[apiGetDropdownField.value], "", OnGetInfo));
        }

        private void OnGetInfo(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return;
            json = getJsonTreeView.LoadJson(json, apiGetDropdownField.value, true, OnTreeItemSelect);
            try
            {
                json = json.PrettyPrintJson();
                GUIUtility.systemCopyBuffer = json;
                infoGetTextField.textField.value = json;
            }
            catch
            {
                infoGetTextField.textField.value = string.Empty;
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
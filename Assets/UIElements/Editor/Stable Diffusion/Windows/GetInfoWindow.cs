using System.Linq;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System;
using System.Collections;

namespace StableDiffusion
{
    public class GetInfoWindow : EditorWindow
    {
        public VisualTreeAsset MainWindow;
        private ObjectField setupAssetObjectField;
        private DropdownField apiDropdownField;
        private Button getInfoButton;
        private TextField infoTextField;
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
            getInfoButton.RegisterCallback<ClickEvent>(OnGetInfoClicked);
            infoTextField = window.Q<TextField>(nameof(infoTextField));
            treeView = window.Q<TreeView>();
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
            if(string.IsNullOrWhiteSpace(json))
                return;
            treeView.LoadJson(json, apiDropdownField.value,true, OnTreeItemSelect);
            infoTextField.value = json;
        }

        private void OnTreeItemSelect(object obj)
        {
            IList objList = (IList)obj;
            foreach(var item in objList) { Debug.Log(item); }
            
        }

        private void OnDestroy()
        {
            getInfoButton?.UnregisterCallback<ClickEvent>(OnGetInfoClicked);
        }
    }
}
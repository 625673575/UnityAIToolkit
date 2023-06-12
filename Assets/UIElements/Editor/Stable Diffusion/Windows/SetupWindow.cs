using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace StableDiffusion
{
    public class SetupWindow : EditorWindow
    {
        public VisualTreeAsset MainWindow;
        private ObjectField setupAssetObjectField;
        private Button runServeButton;
        #region
        private GroupBox setupAssetGroupBox;
        private TextField installationDirectory;
        private TextField launchFile;
        private TextField address;
        private static LaunchSetup _setup;
        public static LaunchSetup Setup { get { return _setup ?? ScriptableObject.CreateInstance<LaunchSetup>(); } set { _setup = value; } }
        #endregion
        private void CreateGUI()
        {
            var window = MainWindow.Instantiate();
            rootVisualElement.Add(window);
            setupAssetObjectField = window.Q<ObjectField>(nameof(setupAssetObjectField));
            setupAssetGroupBox = window.Q<GroupBox>(nameof(setupAssetGroupBox));
            runServeButton = window.Q<Button>(nameof(runServeButton));
            installationDirectory = window.Q<TextField>(nameof(installationDirectory));
            launchFile = window.Q<TextField>(nameof(launchFile));
            address = window.Q<TextField>(nameof(address));
            setupAssetObjectField.RegisterValueChangedCallback(OnAssetValueChange);
            OnAssetValueChange(setupAssetObjectField.value as LaunchSetup);
            runServeButton.RegisterCallback<ClickEvent>(OnRunServeClicked);
        }

        private void OnRunServeClicked(ClickEvent evt)
        {
            if (setupAssetObjectField != null)
            {
                var setupAsset = setupAssetObjectField.value as LaunchSetup;
                if (setupAsset == null)
                    return;
                string strCmdText = setupAsset.launchFile;
                Debug.Log($"execute {strCmdText}");
                string workingDirectory = Directory.GetCurrentDirectory();
                Directory.SetCurrentDirectory(setupAsset.installationDirectory);

                System.Diagnostics.Process.Start(strCmdText);

                Directory.SetCurrentDirectory(workingDirectory);
            }
        }

        private void OnAssetValueChange(ChangeEvent<UnityEngine.Object> evt)
        {
            if (evt.newValue != null)
            {
                LaunchSetup setup = evt.newValue as LaunchSetup;
                OnAssetValueChange(setup);
            }
            else
            {
                setupAssetGroupBox.visible = false;
            }
        }
        void OnAssetValueChange(LaunchSetup setup)
        {
            if (setup != null)
            {
                Setup = setup;
                setupAssetGroupBox.visible = true;
                installationDirectory.value = setup.installationDirectory;
                launchFile.value = setup.launchFile;
                address.value = setup.address;
            }
            else
            {
                setupAssetGroupBox.visible = false;
            }
        }
        private void OnDestroy()
        {
            setupAssetObjectField?.UnregisterValueChangedCallback(OnAssetValueChange);
            runServeButton?.UnregisterCallback<ClickEvent>(OnRunServeClicked);
        }
    }
}
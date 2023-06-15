using NaughtyAttributes;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace StableDiffusion
{
    [CreateAssetMenu(menuName = "Stable Diffusion Config", fileName = "Stable Diffusion Config", order = 0)]
    public class LaunchSetup : ScriptableObject
    {
        [Header("Environment")]
        [Tooltip("Where you install Stable Diffusion")]
        public string installationDirectory = "D:/novalAI";
        [Tooltip("How to run Stable Diffusion")]
        public string launchFile = "D:/novalAI/AÆô¶¯½Å±¾.bat";
        public List<string> commandlineArgs = new List<string>();
        [Header("Login")]
        [Tooltip("URL address, with http:// at the start and without the backslash at the end. eg:http://127.0.0.1:7861")]
        public string address = "http://127.0.0.1:7861";
        [Header("Storage")]
        [Tooltip("Temp directory to save images")]
        public string tempFileDirectory = "E:/temp/Stable Diffusion";
        public bool saveTempFile = true;
        [Space(10)]
        [Header("Extensions")]
        [SerializeField]
        public List<string> extensions = new List<string>();
        [Button]
        private void ChooseTempDirectory()
        {
            tempFileDirectory = EditorUtility.SaveFolderPanel("choose a location to save temp files","","");
        }

    }
}
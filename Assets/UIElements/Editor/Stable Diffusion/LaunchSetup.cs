using System.Collections.Generic;
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
        [Space(10)]
        [Header("Extensions")]
        [SerializeField]
        public List<string> extensions = new List<string>();
    }
}
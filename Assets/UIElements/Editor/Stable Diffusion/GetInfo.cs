using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace StableDiffusion
{
    public static class GetInfo
    {
        public static readonly Dictionary<string, string> ApiGet = new Dictionary<string, string>()
        {
            {"progress","/sdapi/v1/progress" },
            {"memory","/sdapi/v1/memory" },
            {"scripts" , "/sdapi/v1/scripts" },
            {"script-info","/sdapi/v1/script-info" },
            {"sd-models", "/sdapi/v1/sd-models"},
            {"samplers","/sdapi/v1/samplers" },
            {"upscalers", "/sdapi/v1/upscalers"},
            {"cmd-flags","/sdapi/v1/cmd-flags" },
            {"options","/sdapi/v1/options" },
            {"loras","/sdapi/v1/loras" },
            {"prompt-styles","/sdapi/v1/prompt-styles" },
            {"embeddings","/sdapi/v1/embeddings" },
            {"realesrgan-models","/sdapi/v1/realesrgan-models" },
            {"face-restorers","/sdapi/v1/face-restorers" },
            {"controlnet-version","/controlnet/version" },
            {"controlnet-model_list","/controlnet/model_list" },
            {"controlnet-module_list","/controlnet/module_list" },
        }; 
        public static readonly Dictionary<string, string> ApiPost = new Dictionary<string, string>()
        {
            {"options","/sdapi/v1/options" },
            {"interrupt","/sdapi/v1/interrupt" },
            {"refresh-checkpoints","/sdapi/v1/refresh-checkpoints" },
            {"create-embedding","/sdapi/v1/create/embedding" },
            {"create-hypernetwork","/sdapi/v1/create/hypernetwork" },
            {"preprocess","/sdapi/v1/preprocess" },
            {"train-embedding","/sdapi/v1/train/embedding" },
            {"train-hypernetwork","/sdapi/v1/train/hypernetwork" },
            {"unload-checkpoint","/sdapi/v1/unload-checkpoint" },
            {"reload-checkpoint","/sdapi/v1/reload-checkpoint" },
        };
        public static IEnumerator ProcessGetInfoCoroutine(string url, string api, string jsonParams, UnityAction<string> responseEvents, bool postMethod = false)
        {
            using UnityWebRequest getExtras = UnityWebRequest.Put(url + api, jsonParams);
            {
                getExtras.method = postMethod ? "POST" : "GET";
                getExtras.SetRequestHeader("Content-Type", "application/json");

                Debug.Log($"{api} request Sent!");
                yield return getExtras.SendWebRequest();

                if (getExtras.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log($"{api} request Failed: {getExtras.result} {getExtras.error}");
                }
                else
                {
                    Debug.Log($"{api} request Complete!");
                    string responseJsonData = getExtras.downloadHandler.text;

                    responseEvents.Invoke(responseJsonData);
                }
            }
            yield return null;
        }
        public static IEnumerator GetProcessInfoCoroutine(string url, System.Action<Progress> action)
        {
            string api = "/sdapi/v1/progress";
            float percent = 0;
            int percentZeroCount = 0;
            while (percent < 1.0 && percentZeroCount < 5)
            {
                yield return new WaitForSeconds(0.2f);
                using UnityWebRequest getExtras = UnityWebRequest.Put(url + api, new byte[0] { });
                {
                    getExtras.method = "GET";
                    getExtras.SetRequestHeader("Content-Type", "application/json");

                    yield return getExtras.SendWebRequest();

                    if (getExtras.result != UnityWebRequest.Result.Success)
                    {
                        Debug.LogError($"{api} request Failed: {getExtras.result} {getExtras.error}");
                        break;
                    }
                    else
                    {
                        string responseJsonData = getExtras.downloadHandler.text;

                        Progress progress = JsonUtility.FromJson<Progress>(responseJsonData);
                        percent = (float)progress.progress;
                        action(progress);
                        if (percent == 0)
                        {
                            percentZeroCount++;
                            yield return new WaitForSeconds(0.5f);
                        }
                    }
                }
            }
        }
    }
}
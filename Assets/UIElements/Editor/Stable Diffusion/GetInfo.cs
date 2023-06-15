using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace StableDiffusion
{
    public static class GetInfo
    {
        public static readonly Dictionary<string, string> ApiInfo = new Dictionary<string, string>()
        {
            {"memory","/sdapi/v1/memory" },
            {"scripts" , "/sdapi/v1/scripts" },
            {"script-info","/sdapi/v1/script-info" },
            {"sd-models", "/sdapi/v1/sd-models"},
            {"samplers","/sdapi/v1/samplers" },
            {"upscalers", "/sdapi/v1/upscalers"},
            {"cmd-flags","/sdapi/v1/cmd-flags" },
            {"options","/sdapi/v1/options" },
            {"progress","/sdapi/v1/progress" },
            {"loras","/sdapi/v1/loras" },
            {"interrupt","/sdapi/v1/interrupt" }
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
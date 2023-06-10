using StableDiffusion;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace StableDiffusion
{
    public static class GetInfo
    {
        public static readonly Dictionary<string, string> ApiInfo = new Dictionary<string, string>(){
        {"memory","/sdapi/v1/memory" },
        {"scripts" , "/sdapi/v1/scripts" },
        {"script-info","/sdapi/v1/script-info" },
        {"sd-models", "/sdapi/v1/sd-models"},
        {"samplers","/sdapi/v1/samplers" },
        {"upscalers", "/sdapi/v1/upscalers"},
        {"cmd-flags","/sdapi/v1/cmd-flags" },
        {"options","/sdapi/v1/options" },
        {"progress","/sdapi/v1/progress" }
        };
        public static IEnumerator ProcessGetInfoCoroutine(string url, string api, string jsonParams, UnityAction<string> responseEvents)
        {
            using UnityWebRequest getExtras = UnityWebRequest.Put(url + api, jsonParams);
            {
                getExtras.method = "GET";
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

    }
}
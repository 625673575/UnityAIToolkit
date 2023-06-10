using NaughtyAttributes;
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace StableDiffusion
{

    public static class ImageExtra
    {
        private static LaunchSetup configInstance => SetupWindow.Setup;
        private const string logPrefix = "SD: extra-single-image";
        public static IEnumerator ProcessExtraCoroutine(ExtrasPayload extraInput, Texture2D[] textures, UnityEvent<Texture2D>[] responseEvents)
        {
            Texture2D[] upscalingTextures = new Texture2D[textures.Length];
            for (int i = 0; i < textures.Length; i++)
            {
                string url = configInstance.address;

                string json = JsonUtility.ToJson(extraInput);
                using UnityWebRequest getExtras = UnityWebRequest.Put($"{url}/sdapi/v1/extra-single-image", json);
                {
                    getExtras.method = "POST";
                    getExtras.SetRequestHeader("Content-Type", "application/json");

                    Debug.Log($"{logPrefix} request Sent!");
                    yield return getExtras.SendWebRequest();

                    if (getExtras.result != UnityWebRequest.Result.Success)
                    {
                        Debug.Log($"{logPrefix} request Failed: {getExtras.result} {getExtras.error}");
                    }
                    else
                    {
                        Debug.Log($"{logPrefix} request Complete!");
                        string responseJsonData = getExtras.downloadHandler.text;
                        upscalingTextures[i] = GetTextureFromExtra(getExtras.downloadHandler.text);

                        Functions.ApplyTexture2dToOutputs(upscalingTextures[i], responseEvents[i]);
                    }
                }
            }
            yield return null;
        }

        public async static Task<Texture2D> ProcessExtraTask(ExtrasPayload extraInput, Texture2D texture)
        {
            string url = configInstance.address;

            extraInput.image = Convert.ToBase64String(texture.EncodeToPNG());
            string json = JsonUtility.ToJson(extraInput);
            using UnityWebRequest getExtras = UnityWebRequest.Put($"{url}/sdapi/v1/extra-single-image", json);
            {
                getExtras.method = "POST";
                getExtras.SetRequestHeader("Content-Type", "application/json");

                Debug.Log($"{logPrefix} request Sent!");
                UnityWebRequestAsyncOperation requestTask = getExtras.SendWebRequest();
                while (!requestTask.isDone)
                    await Task.Yield();

                if (getExtras.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log($"{logPrefix} request Failed: {getExtras.result} {getExtras.error}");
                    return null;
                }
                else
                {
                    Debug.Log($"{logPrefix} request Complete!");
                    string responseJsonData = getExtras.downloadHandler.text;

                    return GetTextureFromExtra(getExtras.downloadHandler.text);
                }
            }
        }

        private static Texture2D GetTextureFromExtra(string json)
        {
            ExtraContainer container = JsonUtility.FromJson<ExtraContainer>(json);
            byte[] b64_bytes = Convert.FromBase64String(container.image);

            Texture2D tex = new Texture2D(1, 1);
            tex.LoadImage(b64_bytes);

            tex.Apply();

            return tex;
        }


        [Serializable]
        private class ExtraInputImage
        {
            public string data;
            public string name;
        }

        [Serializable]
        private class ExtraContainer
        {
            public string html_info;
            public string image;
        }
    }

    [Serializable]
    public class ExtrasPayload
    {
        [HideInInspector]
        public string image;

        public bool show_extras_results = true;
        public int resize_mode = 1;
        [Label("Resize")]
        public float upscaling_resize = 2;
        public float upscaling_resize_w = 0;
        public float upscaling_resize_h = 0;
        public string upscaler_1 = "None";

        #region rembg extension (Remove Background)
        //Currently not possible, commented out.
        /*
        //[HideInInspector]
        //public string model { get { return modelEnum.GetStringValue(); } }
        public BackgroundModels modelEnum = BackgroundModels.u2net;
        public string model = "None";
        public bool return_mask = false;
        public bool alpha_matting = false;
        public float alpha_matting_foreground_threshold = 240;
        public float alpha_matting_background_threshold = 10;
        public float alpha_matting_erode_size = 10;
        */
        #endregion
    }
}



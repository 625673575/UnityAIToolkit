using NaughtyAttributes;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;
using System.IO;

namespace StableDiffusion
{
    [Serializable]
    public class Img2ImgContainer
    {
        public string[] images;
        public string parameters;
        public string info;
    }
    [Serializable]
    public class Img2ImgPayload
    {
        #region Default Settings
        [Tooltip("Images to convert via img2img")]
        public Texture2D[] images;
        [HideInInspector, SerializeField]
        private string[] init_images = new string[0];

        [TextArea(1, 50)]
        public string prompt;
        [Label("Negative Prompt"), AllowNesting, TextArea(1, 50), Tooltip("exclude this prompt from the generation")]
        public string negative_prompt;

        public int resize_mode = 0;


        [Label("Sampling Method"), AllowNesting, Tooltip("Which algorithm to use to produce the image")]
        public SamplerMethods samplerMethod = SamplerMethods.Euler_a;
        [HideInInspector, SerializeField]
        private string sampler_name;

        [Label("Sampling steps"), AllowNesting, Range(1, 150), Tooltip("How many times to improve the generated image iteratively;higher values take longer; very low values can produce bad results")]
        public int steps = 20;

        [Label("Batch Count"), AllowNesting, Range(1, 100), Tooltip("How many batches of images to create (has no impact on generation performance or VRAM usage)")]
        public int n_iter = 1;

        [Label("Batch Size"), AllowNesting, Range(1, 6), Tooltip("How many images to create in a single batch (increases generation performance at cost of higher VRAM usage)")]
        public int batch_size = 1;

        [Label("Restore Faces"), AllowNesting]
        public bool restore_faces = false;
        [Tooltip("Produces an image that can be tiled")]
        public bool tiling = false;

        [Range(64, 2048)]
        public int width = 512;
        [Range(64, 2048)]
        public int height = 512;
        [Label("CFG Scale"), AllowNesting, Range(1, 30), Tooltip("Classifier Free Guidance Scale - how strongly the image should conform to prompt - lower values produce more creative results")]
        public float cfg_scale = 7;

        [Tooltip("-1 for random seed every time")]
        public int seed = -1;

        [Range(0, 1), Tooltip("Determines how little respect the algorithm should have for image's content. At 0, nothing will change, and at 1 you'll get an unrelated image. With values below 1.0, processing will take less steps than the Sampling Steps slider specifies.")]
        public float denoising_strength = 0.75f;
        #endregion

        [Label("Rotate by 180?), AllowNesting, Tooltip(Should we rotate the resulting image(s) by 180? This is a bugfix since the images are loaded upside down into unity.")]
        public bool rotate180 = false;


        public bool saveImageToFile = false;

        public Img2ImgPayload()
        {
            Initialize();
        }

        public void Initialize()
        {
            //set the upscaler string to the dropdown enum
            //hr_upscaler = upscalerModel.GetStringValue();
            //sampler_name = samplerMethod.GetStringValue();

            if (images != null)
                init_images = Functions.GetStringsFromTextures(images);
        }

        //Returns a copy of this class
        public Img2ImgPayload Copy()
        {
            return (Img2ImgPayload)this.MemberwiseClone();
        }
    }
    public static class Image2Image
    {
        private static LaunchSetup configInstance => SetupWindow.Setup;

        public static IEnumerator GenerateImagesCoroutine(Txt2ImgPayload txt2imgInput, UnityEvent<Texture2D>[] responseEvents)
        {
            GenerateImagesCoroutine(txt2imgInput, null, responseEvents);
            yield return null;
        }

        public static IEnumerator GenerateImagesCoroutine(Txt2ImgPayload txt2imgInput, Renderer[] renderers)
        {
            GenerateImagesCoroutine(txt2imgInput, renderers, null);
            yield return null;
        }

        public static IEnumerator GenerateImagesCoroutine(Txt2ImgPayload txt2imgInput, Renderer[] renderers, UnityEvent<Texture2D>[] responseEvents)
        {
            if (configInstance == null)
            {
                Debug.LogError("Stable Diffusion Config doesn't exist! Please create one.");
                yield break;
            }

            string url = configInstance.address;

            txt2imgInput.Initialize();
            Texture2D[] textures = new Texture2D[0];

            //Send request to server to generate a stable diffusion image
            //Note:
            //Normally we would be using Unity's UnityWebRequest.Post command like this:
            //using (UnityWebRequest getReq = UnityWebRequest.Post($"http://{username}:{password}@141.100.233.171:4000/sdapi/v1/txt2img", postData))
            //however, "For some reason UnityWebRequest applies URL encoding to POST message payloads." as seen here: https://forum.unity.com/threads/unitywebrequest-post-url-jsondata-sending-broken-json.414708/
            //The solution is to first create it as a UnityWebRequest.Put request and to then change it to Post We then specify that it's a json and magically, it now works!

            using UnityWebRequest getReq = UnityWebRequest.Put($"{url}/sdapi/v1/txt2img", JsonUtility.ToJson(txt2imgInput));
            {
                getReq.method = "POST";
                getReq.SetRequestHeader("Content-Type", "application/json");

                Debug.Log("SD: txt2img request Sent!");
                yield return getReq.SendWebRequest();

                //Handle HTTP error
                if (getReq.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log($"SD: txt2img request Failed: {getReq.result} {getReq.error}");
                }
                //Handle successful HTTP request
                else
                {
                    Debug.Log("SD: txt2img request Complete!");
                    // Access the response data from getReq.downloadHandler
                    //Task<Texture2D[]> task = GetTexturesFromtxt2imgAsync(getReq.downloadHandler.text, txt2imgInput.rotate180);
                    //yield return new WaitUntil(() => task.IsCompleted);
                    //textures = task.Result;
                    textures = GetTexturesFromtxt2img(getReq.downloadHandler.text, txt2imgInput);

                    //if (!txt2imgInput.useExtra || (txt2imgInput.showExtra && txt2imgInput.useExtra && txt2imgInput.showSteps)) //set texture to output if we are not using extra, or if we are using extra and showing the progress steps
                    Functions.ApplyTexture2dToOutputs(textures, renderers, responseEvents);
                }
            }

            //if (txt2imgInput.showExtra && txt2imgInput.useExtra)
            //{
            //    yield return Img2Extras.ProcessExtraCoroutine(txt2imgInput.extraInput, textures, renderers, responseEvents);
            //}
        }

        public async static Task<Texture2D[]> GenerateImagesTask(Txt2ImgPayload txt2imgInput)
        {
            if (configInstance == null)
            {
                Debug.LogError("Stable Diffusion Config doesn't exist! Please create one.");
                return null;
            }

            string url = configInstance.address;

            txt2imgInput.Initialize(); //update hidden values to their true values
            Texture2D[] textures;

            //Send request to server to generate a stable diffusion image
            //Note:
            //Normally we would be using Unity's UnityWebRequest.Post command like this:
            //using (UnityWebRequest getReq = UnityWebRequest.Post($"http://{username}:{password}@141.100.233.171:4000/sdapi/v1/txt2img", postData))
            //however, "For some reason UnityWebRequest applies URL encoding to POST message payloads." as seen here: https://forum.unity.com/threads/unitywebrequest-post-url-jsondata-sending-broken-json.414708/
            //The solution is to first create it as a UnityWebRequest.Put request and to then change it to Post We then specify that it's a json and magically, it now works!

            using UnityWebRequest getReq = UnityWebRequest.Put($"{url}/sdapi/v1/txt2img", JsonUtility.ToJson(txt2imgInput));
            {
                getReq.method = "POST";
                getReq.SetRequestHeader("Content-Type", "application/json");

                Debug.Log("SD: txt2img request Sent!");
                UnityWebRequestAsyncOperation requestTask = getReq.SendWebRequest();

                while (!requestTask.isDone)
                    await Task.Yield();

                //Handle HTTP error
                if (getReq.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log($"SD: txt2img request Failed: {getReq.result} {getReq.error}");
                    return null;
                }
                //Handle successful HTTP request
                else
                {
                    Debug.Log("SD: txt2img request Complete!");
                    // Access the response data from getReq.downloadHandler
                    //Task<Texture2D[]> task = GetTexturesFromtxt2imgAsync(getReq.downloadHandler.text, txt2imgInput.rotate180);
                    //yield return new WaitUntil(() => task.IsCompleted);
                    //textures = task.Result;
                    textures = GetTexturesFromtxt2img(getReq.downloadHandler.text, txt2imgInput);
                }
            }

            return textures;
        }




        //Convert a json string to Sprite
        private static Texture2D[] GetTexturesFromtxt2img(string json, Txt2ImgPayload input)
        {
            List<Texture2D> texture2Ds = new List<Texture2D>();
            Txt2ImgContainer container = JsonUtility.FromJson<Txt2ImgContainer>(json);

            for (int i = 0; i < container.images.Length; i++)
            {
                byte[] b64_bytes = Convert.FromBase64String(container.images[i]); //convert the image's strings to bytes.

                if (input.saveImageToFile)
                {
                    string path = $"{Application.persistentDataPath}/StableDiffusion/";
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    path += $"{DateTime.Now.ToString("yyyy-dd-M-HHmmss")}_{i}.png";
                    Debug.Log($"SD: Saving image to {path}");
                    File.WriteAllBytes(path, b64_bytes);
                }

                //load bytes into a new texture
                Texture2D tex = new Texture2D(1, 1);
                tex.LoadImage(b64_bytes);

                if (input.rotate180)
                {
                    //reverse the array to rotate the image 180?as it is otherwise imported upside down
                    Color[] pix = tex.GetPixels();
                    Array.Reverse(pix, 0, pix.Length);
                    tex.SetPixels(pix);
                }
                tex.Apply();
                texture2Ds.Add(tex);
            }

            return texture2Ds.ToArray();
        }

        //Convert a json string to Sprite
        private static async Task<Texture2D[]> GetTexturesFromtxt2imgAsync(string json, bool rotate180)
        {
            Txt2ImgContainer container = JsonUtility.FromJson<Txt2ImgContainer>(json);
            Texture2D[] texture2Ds = new Texture2D[container.images.Length];

            for (int i = 0; i < texture2Ds.Length; i++)
                texture2Ds[i] = new Texture2D(1, 1);

            byte[][] bytesArray = new byte[container.images.Length][];
            await Task.Run(() =>
            {
                for (int i = 0; i < bytesArray.Length; i++)
                {
                    bytesArray[i] = Convert.FromBase64String(container.images[i]); //convert theimage's strings to bytes.
                }
            });

            for (int i = 0; i < bytesArray.Length; i++)
            {
                texture2Ds[i].LoadImage(bytesArray[i]);

                if (rotate180)
                {
                    //reverse the array to rotate the image 180?as it is otherwise imported upside down
                    Color[] pix = texture2Ds[i].GetPixels();
                    Array.Reverse(pix, 0, pix.Length);
                    texture2Ds[i].SetPixels(pix);
                }
            }
            return texture2Ds;
        }

    }
}
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

namespace StableDiffusion
{
    public class Functions
    {
        public static void ApplyTexture2dToOutputs(Texture2D[] textures, UnityEvent<Texture2D>[] responseEvents)
        {
            if (responseEvents != null)
                for (int i = 0; i < responseEvents.Length; i++)
                    responseEvents[i]?.Invoke(textures[i]);
        }

        public static void ApplyTexture2dToOutputs(Texture2D texture, UnityEvent<Texture2D> responseEvent)
        {
            if (responseEvent != null)
                responseEvent?.Invoke(texture);
        }

        public static string[] GetStringsFromTextures(Texture2D[] textures)
        {
            List<string> stringList = new List<string>();

            for (int i = 0; i < textures.Length; i++)
            {
                stringList.Add(Convert.ToBase64String(textures[i].EncodeToPNG()));
            }

            return stringList.ToArray();
        }
    }
}
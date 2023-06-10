using UnityEditor;
using UnityEngine;
using StableDiffusion;
public static class OpenConfigWindow 
{
    [MenuItem("AI Toolkit/Stable Diffusion/Config")]
    public static void ShowExample()
    {
        var wnd = EditorWindow.GetWindow<SetupWindow>();
        wnd.titleContent = new GUIContent("StableDiffusion Setup");
    }
    [MenuItem("AI Toolkit/Stable Diffusion/Txt2Img")]
    public static void Txt2Img()
    {
        var wnd = EditorWindow.GetWindow<Txt2ImgWindow>();
        wnd.titleContent = new GUIContent("StableDiffusion Txt2Img");
    }
}

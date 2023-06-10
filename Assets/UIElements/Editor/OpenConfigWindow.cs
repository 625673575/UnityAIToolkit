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
    [MenuItem("AI Toolkit/Stable Diffusion/Img2Img")]
    public static void Img2Img()
    {
        var wnd = EditorWindow.GetWindow<Img2ImgWindow>();
        wnd.titleContent = new GUIContent("StableDiffusion Img2Img");
    }
    [MenuItem("AI Toolkit/Stable Diffusion/Extra")]
    public static void Extra()
    {
        var wnd = EditorWindow.GetWindow<ExtraWindow>();
        wnd.titleContent = new GUIContent("StableDiffusion Extra");
    }
    [MenuItem("AI Toolkit/Stable Diffusion/Info")]
    public static void Info()
    {
        var wnd = EditorWindow.GetWindow<GetInfoWindow>();
        wnd.titleContent = new GUIContent("StableDiffusion Info");
    }
}
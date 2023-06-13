using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public static class NetworkError
{
    public delegate void OnWebRequestFailed(UnityWebRequest.Result code, string error);
    public static event OnWebRequestFailed onWebRequestFailed;
    [InitializeOnLoadMethod]
    static void EditorInitializeOnLoadMethod()
    {
        onWebRequestFailed += (c, e) => {Debug.Log(e); };
        Debug.Log("EditorInitializeOnLoadMethod");
    }
}

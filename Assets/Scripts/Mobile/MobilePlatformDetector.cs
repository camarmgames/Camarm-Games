using UnityEngine;
using System.Runtime.InteropServices;

public class MobilePlatformDetector : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern bool WebGLDetectMobile();

    public static bool IsMobile()
    {
#if UNITY_EDITOR
        return false;

#elif UNITY_ANDROID || UNITY_IOS
        return true;

#elif UNITY_WEBGL
        try
        {
            return WebGLDetectMobile();
        }
        catch
        {
            return false;
        }

#else
        return false;
#endif
    }
}

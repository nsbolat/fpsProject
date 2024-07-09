using UnityEditor;
using UnityEngine;
using System.IO;

public class TakeScreenShot : MonoBehaviour
{
    public Camera camera;
    public string screenshotFileName = "screenshot.png";
    
    void Update()
    {
        // Screenshot almak için belirli bir tuşa (örneğin F12) basıldığında TakeScreenshot fonksiyonunu çağırıyoruz
        if (Input.GetKeyDown(KeyCode.F12))
        {
            string directoryPath = Path.Combine(Application.dataPath, "icon");
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            string fullPath = Path.Combine(directoryPath, screenshotFileName);
            TakeScreenshot(fullPath);
            Debug.Log("Screenshot saved to: " + fullPath);
        }
    }

    void TakeScreenshot(string fullPath)
    {
        if (camera == null)
        {
            camera = GetComponent<Camera>();
        }

        RenderTexture rt = new RenderTexture(256, 256, 24);
        camera.targetTexture = rt;
        Texture2D screenShot = new Texture2D(256, 256, TextureFormat.RGBA32, false);
        camera.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, 256, 256), 0, 0);
        camera.targetTexture = null;
        RenderTexture.active = null;

        if (Application.isEditor)
        {
            DestroyImmediate(rt);
        }
        else
        {
            Destroy(rt);
        }

        byte[] bytes = screenShot.EncodeToPNG();
        System.IO.File.WriteAllBytes(fullPath, bytes);
#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
    }
}
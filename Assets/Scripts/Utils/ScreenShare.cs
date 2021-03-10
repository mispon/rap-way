using System.Collections;
using System.IO;
using Core;
using Localization;
using UnityEngine;

namespace Utils
{
    /// <summary>
    /// Поделиться экраном в соц. сетях
    /// </summary>
    public class ScreenShare : MonoBehaviour
    {
        public void ShareSocials()
        {
            SoundManager.Instance.PlayClick();
            StartCoroutine(TakeScreenshotAndShare());
        }

        private static IEnumerator TakeScreenshotAndShare()
        {
            string message = LocalizationManager.Instance.Get("screen_share");

            yield return new WaitForEndOfFrame();

            Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            ss.Apply();

            string filePath = Path.Combine(Application.temporaryCachePath, "shared img.png");
            File.WriteAllBytes(filePath, ss.EncodeToPNG());

            // To avoid memory leaks
            Destroy(ss);

            new NativeShare().AddFile(filePath)
                .SetSubject("Rap Way")
                .SetText(message)
                .Share();
        }
    }
}
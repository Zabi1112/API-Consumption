using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public static class ImageDowloadUtility 
{  //

    private static string SaveImagesFolderName = "ApiBaseImages";
    public static void DownloadAndSaveOrRetrieveImage(MonoBehaviour coroutineHost, string url, Action<Sprite> cd)
    {
        var persistentDataPath = Application.persistentDataPath;
        string fileName = NetworkUtility.GetFileNameFromUrl(url);
        string filePath = Path.Combine(persistentDataPath,SaveImagesFolderName, fileName);

        if (File.Exists(filePath))
        {
            //  Debug.Log("File " + fileName + " exist on local space");
            Texture2D texture = new Texture2D(1024, 1024, TextureFormat.RGBA32, false);
            byte[] imageData = File.ReadAllBytes(filePath);

            if (texture.LoadImage(imageData))
            {
                texture.name = fileName;
                Sprite pic = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f));
                cd?.Invoke(pic);
            }
        }
        else
        {
            DownloadAndSaveImage(coroutineHost, url, cd);
        }
    }

    //
    public static void DownloadAndSaveImage(MonoBehaviour coroutineHost, string url, Action<Sprite> cd)
    {
        coroutineHost.StopCoroutine(DownloadAndSaveImageCoroutine(url, cd));
        coroutineHost.StartCoroutine(DownloadAndSaveImageCoroutine(url, cd));
    }

    private static IEnumerator DownloadAndSaveImageCoroutine(string url, Action<Sprite> cd)
    {
        //  Debug.Log("Hitting  DownloadAndSaveImage Urls: " + url);
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
        {
            yield return www.SendWebRequest();

            // Set the persistent data path
            var persistentDataPath = Application.persistentDataPath;
            string fileName = NetworkUtility.GetFileNameFromUrl(url);

            // Debug.Log("Downloading image with name : " + fileName);

            if (www.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                byte[] bytes = texture.EncodeToPNG();

                string folderPath = Path.Combine(persistentDataPath,SaveImagesFolderName);

                try
                {
                    // Ensure the directory exists
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                        //    Debug.Log("Created directory: " + folderPath);
                    }

                    string filePath = Path.Combine(folderPath, fileName);

                    // Ensure the file doesn't already exist
                    if (File.Exists(filePath))
                    {
//                        Debug.Log("File already exists: " + filePath);
                        Texture2D texture1 = new Texture2D(1024, 1024, TextureFormat.RGBA32, false);
                        byte[] imageData = File.ReadAllBytes(filePath);

                        if (texture1.LoadImage(imageData))
                        {
                            texture1.name = fileName;
                            Sprite pic = Sprite.Create(texture1, new Rect(0, 0, texture1.width, texture1.height),
                                new Vector2(0.5f, 0.5f));
                            cd?.Invoke(pic);
                        }
                    }
                    else
                    {
                        File.WriteAllBytes(filePath, bytes);
                        //  Debug.Log("File saved successfully: " + filePath);

                        texture.name = fileName;
                        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                            new Vector2(0.5f, 0.5f));
                        cd?.Invoke(sprite);
                    }
                }
                catch (Exception ex)
                {
                    //   Debug.LogError("Error saving file: " + ex.Message);
                    cd?.Invoke(null);
                }
            }
            else
            {
                //  Debug.LogError("Error downloading image file: " + www.error);
                cd?.Invoke(null);
            }
        }
    }

    private static void DownloadImage(MonoBehaviour coroutineHost, string url, Action<Sprite> cd)
    {
        coroutineHost.StopCoroutine(DownloadImageCoroutine(url, cd));
        coroutineHost.StartCoroutine(DownloadImageCoroutine(url, cd));
    }

    private static IEnumerator DownloadImageCoroutine(string url, Action<Sprite> cd)
    {
        //  Debug.Log("Hitting DownloadImage Urls: " + url);
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
        {
            yield return www.SendWebRequest();


            if (www.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;

                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f));

                cd?.Invoke(sprite);
            }
            else
            {
                Debug.LogError("Error downloading image file: " + www.error);
                cd?.Invoke(null);
            }
        }
    }
}

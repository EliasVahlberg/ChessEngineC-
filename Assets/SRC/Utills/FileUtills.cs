#if UNITY_STANDALONE_WIN
using System.IO;

using UnityEngine;
using AnotherFileBrowser.Windows;
using System.Collections;
using UnityEngine.Networking;
using System;

namespace Utills
{

    public class FileUtills : MonoBehaviour
    {
        public static FileUtills instance;
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Debug.Log("SAMEINSTACE ");
                Destroy(this);
            }
        }


        public string[] data;
        private FileBrowser fileBrowser;
        private string[] target;

        public string[] GetAllFilesWithFileExtension(string path, string fileExtention)
        {

            return Directory.GetFiles(path, "*." + fileExtention, SearchOption.TopDirectoryOnly);
        }



        public void GetPathFromFileExplorer(string filter, Action<string> callback)
        {
            if (SystemInfo.operatingSystem.Contains("Windows") || SystemInfo.operatingSystem.Contains("windows"))
            {
                BrowserProperties bp = new BrowserProperties("Get Directory");
                bp.filter = filter;
                fileBrowser = new FileBrowser();
                fileBrowser.OpenFileBrowser(bp, path => { callback(path); });

            }
            else
                Debug.LogError("This feature only works for windows, sorry.");
        }


        //* filter EXAMPLE : "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
        public void GetFilesFromFileExplorer(string filter, Action<string[]> callback)
        {
            if (SystemInfo.operatingSystem.Contains("Windows") || SystemInfo.operatingSystem.Contains("windows"))
            {
                BrowserProperties bp = new BrowserProperties("Get Directory");
                bp.filter = filter;
                fileBrowser = new FileBrowser();
                fileBrowser.OpenMultiFileBrowser(bp, p => { StartCoroutine(LoadMultiData(p, callback)); });

            }
            else
                Debug.LogError("This feature only works for windows, sorry.");


        }
        IEnumerator LoadData(string path)
        {
            Debug.Log(path);
            using (UnityWebRequest webRequest = UnityWebRequest.Get(path))
            {
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();

                string[] pages = path.Split('/');
                int page = pages.Length - 1;


                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                        Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.Success:
                        Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                        break;
                }
            }

        }

        IEnumerator LoadMultiData(string[] paths, Action<String[]> callback) //TODO LoadMultiData(string[] paths, Action<string[]>) 
        {
            target = new string[paths.Length];
            for (int ii = 0; ii < paths.Length; ii++)
            {
                string path = paths[ii];
                using (UnityWebRequest webRequest = UnityWebRequest.Get(path))
                {
                    yield return webRequest.SendWebRequest();
                    switch (webRequest.result)
                    {
                        case UnityWebRequest.Result.ConnectionError:
                        case UnityWebRequest.Result.DataProcessingError:
                            Debug.LogError(": Error: " + webRequest.error);
                            break;
                        case UnityWebRequest.Result.ProtocolError:
                            Debug.LogError(": HTTP Error: " + webRequest.error);
                            break;
                        case UnityWebRequest.Result.Success:
                            target[ii] = webRequest.downloadHandler.text;

                            break;
                    }
                }
            }
            callback(target);
        }

        public void SaveToFile(string filter, string content, Action<string> assyncCallbackTwo)
        {
            if (SystemInfo.operatingSystem.Contains("Windows") || SystemInfo.operatingSystem.Contains("windows"))
            {
                BrowserProperties bp = new BrowserProperties("Set Save Path");
                bp.filter = filter;
                fileBrowser = new FileBrowser();
                fileBrowser.SaveFileBrowser(bp, content, p => { StartCoroutine(SaveToFileCallback(p, assyncCallbackTwo)); });

            }
            else
                Debug.LogError("This feature only works for windows, sorry.");
        }

        IEnumerator SaveToFileCallback(string path, Action<string> assyncCallbackTwo)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(path))
            {
                yield return webRequest.SendWebRequest();
                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                        Debug.LogError(": Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError(": HTTP Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.Success:
                        assyncCallbackTwo(path);

                        break;
                }
            }
        }

    }
}
#endif
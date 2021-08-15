using System.IO;
using System;
//using UnityEngine;

namespace Utills
{

    public static class FileUtills
    {
        public static string[] GetAllFilesWithFileExtension(string path, string fileExtention)
        {

            return Directory.GetFiles(path, "*." + fileExtention, SearchOption.TopDirectoryOnly);
        }
        public static string GetDirectoryFromFileExplorer()
        {
            //string s = UnityEditor.EditorUtility.OpenFolderPanel("OPEN FOLDER", Application.dataPath + "/Resources/", "folder");
            //Debug.Log(s);
            return "";
        }
    }
}
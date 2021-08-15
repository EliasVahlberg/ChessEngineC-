using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class FileWriter
{

    public static void WriteToTextAsset_EditorOnly(TextAsset textAsset, string text, bool append)
    {
        //? is this C-Style import guards?
#if UNITY_EDITOR
        string outputPath = UnityEditor.AssetDatabase.GetAssetPath(textAsset);
        StreamWriter writer = new StreamWriter(outputPath, append);
        Debug.Log("BL = " + text.Length);
        writer.Write(text);
        writer.Close();
#endif
    }


}
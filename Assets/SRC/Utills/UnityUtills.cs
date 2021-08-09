//using UnityEditor;
using UnityEngine;

namespace Utills
{
    public static class UnityUtills
    {
        public static T[] GetAllInstances<T>() where T : ScriptableObject
        {
            Object[] rawObjs = Resources.LoadAll("AIs/", typeof(T));
            T[] objs = new T[rawObjs.Length];
            for (int ii = 0; ii < rawObjs.Length; ii++)
            {
                objs[ii] = (T)rawObjs[ii];
            }
            return objs;

        }
        //public static T[] GetAllInstances<T>() where T : ScriptableObject
        //{
        //    string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);
        //    T[] a = new T[guids.Length];
        //    for (int i = 0; i < guids.Length; i++)
        //    {
        //        string path = AssetDatabase.GUIDToAssetPath(guids[i]);
        //        a[i] = AssetDatabase.LoadAssetAtPath<T>(path);
        //    }
        //
        //    return a;
        //
        //}
    }
}
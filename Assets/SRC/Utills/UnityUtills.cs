//using UnityEditor;
using System.Linq;
using UnityEngine;

/*
    @File UnityUtills.cs
    @author Elias Vahlberg 
    @Date 2021-07
*/
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
        //public static void getAll<T>()
        //{
        //    var type = typeof(T);
        //    var types = System.AppDomain.CurrentDomain.GetAssemblies()
        //                    .Where(x => x.FullName.StartsWith("YourNamespace"))
        //                    .SelectMany(x => x.GetTypes())
        //                    .Where(x => x.IsClass && type.IsAssignableFrom(x));
        //
        //    foreach (System.Type t in types)
        //    {
        //        T obj = Container.Resolve(t) as T;
        //    }
        //}
        public static System.Collections.Generic.List<string> GetAllEntities<T>()
        {
            return System.AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                 .Where(x => typeof(T).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                 .Select(x => x.FullName).ToList();
        }
        public static System.Collections.Generic.List<T> GetAllEntitiesAsClasses<T>()
        {
            return System.AppDomain.CurrentDomain.GetAssemblies().
                Where(obj => obj.GetTypes().
                        Any(x => typeof(T).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)).
                Select(obj =>
                    ((T)obj.CreateInstance(
                        obj.GetTypes().
                        First(x => typeof(T).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract).FullName
                    )
                )
            ).ToList();
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
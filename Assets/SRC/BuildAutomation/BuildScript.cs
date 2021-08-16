using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class BuildScript
{
    private const string BUILDS_FOLDER_PATH = @"C:\Users\elias\SimpleChessV2\Builds\";
    private const string BUILDS_FOLDER_PATH_UNITY = "./Builds/";
    private const string BUILD_FOLDER_NAME = "Build_V1.5";
    private const string PLATFORM_SUBFOLDER_MAC = "/Windows_x86_64";
    private const string PLATFORM_SUBFOLDER_LINUX = "/macOS_64";
    private const string PLATFORM_SUBFOLDER_WINDOWS = "/Linux_x86_64";

    static void PreformBuild()
    {
        string[] scenes = { "Assets/Scenes/MainScene.unity" };
        string path = BUILDS_FOLDER_PATH + BUILD_FOLDER_NAME;
        string unityPath = BUILDS_FOLDER_PATH_UNITY + BUILD_FOLDER_NAME;
        System.IO.Directory.CreateDirectory(path);
        System.IO.Directory.CreateDirectory(path + PLATFORM_SUBFOLDER_MAC);
        System.IO.Directory.CreateDirectory(path + PLATFORM_SUBFOLDER_WINDOWS);
        System.IO.Directory.CreateDirectory(path + PLATFORM_SUBFOLDER_LINUX);
        BuildPipeline.BuildPlayer(
            scenes,
            unityPath + PLATFORM_SUBFOLDER_MAC + "/" + PLATFORM_SUBFOLDER_MAC + ".app",
            BuildTarget.StandaloneOSX,
            BuildOptions.None);

        BuildPipeline.BuildPlayer(
        scenes,
        unityPath + PLATFORM_SUBFOLDER_WINDOWS + "/" + PLATFORM_SUBFOLDER_WINDOWS + ".exe",
        BuildTarget.StandaloneWindows64,
        BuildOptions.None);

        BuildPipeline.BuildPlayer(
            scenes,
            unityPath + PLATFORM_SUBFOLDER_LINUX + "/" + PLATFORM_SUBFOLDER_LINUX + ".x86_64",
            BuildTarget.StandaloneLinux64,
            BuildOptions.None);
    }
}

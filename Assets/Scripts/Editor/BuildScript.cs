using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BuildScript
{
    private const string BUILDS_FOLDER_PATH = @"C:\Users\elias\SimpleChessV2\Builds\";
    private const string GAME_NAME = "SimpleChess";
    private const string BUILDS_FOLDER_PATH_UNITY = "./Builds/";
    private const string BUILD_FOLDER_NAME = "Build_V1.5";
    private const string PLATFORM_SUBFOLDER_MAC =
    "/macOS_64";
    private const string PLATFORM_SUBFOLDER_LINUX =
    "/Linux_x86_64";
    private const string PLATFORM_SUBFOLDER_WINDOWS =
    "/Windows_x86_64";

    static readonly string[] scenes = { @"C:\Users\elias\SimpleChessV2\Assets\Scenes\MainScene.unity" };
    static string name = "MyGame";
    [MenuItem("Build/Build Mac")]
    static void BuildMac()
    {
        BuildPipeline.BuildPlayer(scenes, BUILDS_FOLDER_PATH_UNITY + BUILD_FOLDER_NAME + PLATFORM_SUBFOLDER_MAC + "/" + GAME_NAME + ".app", BuildTarget.StandaloneOSX, BuildOptions.None);
    }
    [MenuItem("Build/Build Windows")]
    static void BuildWindows()
    {
        BuildPipeline.BuildPlayer(scenes, BUILDS_FOLDER_PATH_UNITY + BUILD_FOLDER_NAME + PLATFORM_SUBFOLDER_WINDOWS + "/" + GAME_NAME + ".exe", BuildTarget.StandaloneWindows64, BuildOptions.None);
    }

    [MenuItem("Build/Build Linux")]
    static void BuildLinux()
    {
        BuildPipeline.BuildPlayer(scenes, BUILDS_FOLDER_PATH_UNITY + BUILD_FOLDER_NAME + PLATFORM_SUBFOLDER_LINUX + "/" + GAME_NAME + ".x86_64", BuildTarget.StandaloneLinux64, BuildOptions.None);
    }

    [MenuItem("Build/Build All")]
    static void BuildAll()
    {
        string[] scenes = { @"C:\Users\elias\SimpleChessV2\Assets\Scenes\MainScene.unity" };
        string path = BUILDS_FOLDER_PATH + BUILD_FOLDER_NAME;
        string unityPath = BUILDS_FOLDER_PATH_UNITY + BUILD_FOLDER_NAME;
        System.IO.Directory.CreateDirectory(path);
        System.IO.Directory.CreateDirectory(path + PLATFORM_SUBFOLDER_MAC);
        System.IO.Directory.CreateDirectory(path + PLATFORM_SUBFOLDER_WINDOWS);
        System.IO.Directory.CreateDirectory(path + PLATFORM_SUBFOLDER_LINUX);
        BuildLinux();
        BuildWindows();
        BuildMac();
    }
}
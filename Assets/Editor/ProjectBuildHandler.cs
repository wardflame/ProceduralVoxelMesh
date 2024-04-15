using Cinemachine;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;


[InitializeOnLoad]
public class ProjectBuildHandler : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    [SaveDuringPlay]
    public static string Version
    {
        get { return PlayerSettings.bundleVersion; }
        set { PlayerSettings.bundleVersion = value; }
    }

    static ProjectBuildHandler()
    {
        EditorApplication.playModeStateChanged += IncrementBuildNumber;
    }

    public void OnPreprocessBuild(BuildReport report)
    {
        IncrementBuildNumber();
    }

    private static void IncrementBuildNumber(PlayModeStateChange state)
    {
        if (state != PlayModeStateChange.EnteredEditMode) return;

        IncrementBuildNumber();
    }

    private static void IncrementBuildNumber()
    {
        string versionStr = PlayerSettings.bundleVersion;
        string buildStr = versionStr.ToString().Split('.').Last();

        if (!int.TryParse(buildStr, out int buildNum))
        {
            throw new System.Exception("Failed to parse PlayerSettings.bundleVersion to int!");
        }
        string versionWOBuild = versionStr.Remove(versionStr.LastIndexOf('.'));

        Debug.Log(versionWOBuild);

        buildNum++;

        string newVersion = versionWOBuild + "." + buildNum.ToString();

        Version = newVersion;

        Debug.Log("BUILD UPDATE: " + newVersion);
    }
}

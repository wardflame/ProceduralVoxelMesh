using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;


public class ProjectBuildPreprocess : IPostprocessBuildWithReport
{
    public int callbackOrder => 1;

    public void OnPostprocessBuild(BuildReport report)
    {
        if (report.summary.result == BuildResult.Succeeded)
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

            PlayerSettings.bundleVersion = versionWOBuild + "." + buildNum.ToString();
        }
    }
}

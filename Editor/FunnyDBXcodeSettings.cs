#if UNITY_IOS
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.iOS.Xcode;
using UnityEditor.iOS.Xcode.Extensions;
using UnityEditor.Callbacks;
using System.IO;
using System.Linq;

namespace SoFunny.FunnyDB.Editor
{

    public static class FunnyDBXcodeSettings
    {
        
        private const string FRAMEWORK_ORIGIN_PATH = "Packages/com.sofunny.funnydb/Plugins/iOS/iOSDynamicSDK";

        private const string FRAMEWORK_TARGET_PATH = "FunnyDBFrameworks";

        private static string[] SDKNames = new string[] { "FunnyDBDebugTools.framework" };

        [PostProcessBuild(999)]
        public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
        {
            if (target != BuildTarget.iOS)
            {
                return;
            }

            string pbxProjectPath = PBXProject.GetPBXProjectPath(pathToBuiltProject);
            PBXProject proj = new PBXProject();
            proj.ReadFromFile(pbxProjectPath);

            string unityPackageTargetGUID = proj.GetUnityFrameworkTargetGuid();
            string projectTargetGUID = proj.GetUnityMainTargetGuid();

            proj.SetBuildProperty(unityPackageTargetGUID, "ENABLE_BITCODE", "NO");
            proj.SetBuildProperty(projectTargetGUID, "ENABLE_BITCODE", "NO");

            proj.SetBuildProperty(unityPackageTargetGUID, "CLANG_ENABLE_MODULES", "YES");
            proj.SetBuildProperty(projectTargetGUID, "CLANG_ENABLE_MODULES", "YES");

            proj.SetBuildProperty(unityPackageTargetGUID, "CLANG_MODULES_AUTOLINK", "YES");
            proj.SetBuildProperty(projectTargetGUID, "CLANG_MODULES_AUTOLINK", "YES");

            proj.SetBuildProperty(unityPackageTargetGUID, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "YES");
            proj.SetBuildProperty(projectTargetGUID, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "YES");

            //string otherFlag = proj.GetBuildPropertyForConfig(projectTargetGUID, "OTHER_LDFLAGS");
            //if (otherFlag == null || !otherFlag.Contains("ObjC")) {
            //    proj.SetBuildProperty(projectTargetGUID, "OTHER_LDFLAGS", "-ObjC");
            //}

#if UNITY_2021_2_OR_NEWER
            if (EditorUserBuildSettings.development || EditorUserBuildSettings.iOSXcodeBuildConfig == XcodeBuildConfig.Debug)
            {
#else
            if (EditorUserBuildSettings.development || EditorUserBuildSettings.iOSBuildConfigType == iOSBuildType.Debug)
            {             
#endif
                proj.AddBuildProperty(projectTargetGUID, "FRAMEWORK_SEARCH_PATHS", $"$(PROJECT_DIR)/{FRAMEWORK_TARGET_PATH}");

                foreach (var framework in SDKNames)
                {
                    var groupFolder = Path.Combine(FRAMEWORK_TARGET_PATH, framework);
                    var fullPath = Path.Combine(FRAMEWORK_ORIGIN_PATH, framework);

                    Copy(fullPath, Path.Combine(pathToBuiltProject, groupFolder));

                    var folderGuid = proj.AddFile(groupFolder, groupFolder);
                    var mainLinkPhaseGuid = proj.GetFrameworksBuildPhaseByTarget(projectTargetGUID);

                    proj.AddFileToEmbedFrameworks(projectTargetGUID, folderGuid);
                    proj.AddFileToBuildSection(projectTargetGUID, mainLinkPhaseGuid, folderGuid);
                }
            }

            proj.WriteToFile(pbxProjectPath);
        }


        /// <summary>
        /// Copies the path from source to destination and removes any .meta files from the destination
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        private static void Copy(string source, string destination)
        {
            // Clean up any existing unity plugins or old from previous build...
            var dirPath = Path.GetDirectoryName(destination);
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            if (Directory.Exists(destination))
            {
                Directory.Delete(destination, true);
            }

            // Copy raw from source...
            FileUtil.CopyFileOrDirectory(source, destination);

            // Recursively cleanup meta files...
            RecursiveCleanupMetaFiles(new DirectoryInfo(destination));
        }

        /// <summary>
        /// Private recursive method to remove .meta files from a directory and all of it's sub directories.
        /// </summary>
        private static void RecursiveCleanupMetaFiles(DirectoryInfo directory)
        {

            var directories = directory.GetDirectories();
            var files = directory.GetFiles();

            foreach (var file in files)
            {
                // File is a Unity meta file, clean it up...
                if (file.Extension == ".meta")
                {
                    FileUtil.DeleteFileOrDirectory(file.FullName);
                }
            }

            // Recurse...
            foreach (var subdirectory in directories)
            {
                RecursiveCleanupMetaFiles(subdirectory);
            }
        }

    }

}

#endif
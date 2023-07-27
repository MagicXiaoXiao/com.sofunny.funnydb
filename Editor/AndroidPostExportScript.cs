#if UNITY_ANDROID

using UnityEditor.Android;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SoFunny.FunnyDB.Editor {



    partial class AndroidGradleProcess : IPostGenerateGradleAndroidProject
    {

        private const string AAR_ORIGIN_PATH = "Packages/com.sofunny.funnydb/Editor/Android";

        private const string AAR_TARGET_PATH = "libs";

        public int callbackOrder
        {
            get
            {
                return 680;
            }
        }

        public void OnPostGenerateGradleAndroidProject(string path)
        {

            Start(path);
        }
    }

    partial class AndroidGradleProcess {

        internal static void Start(string projectPath) {
            SetupGradleProperties(projectPath);
            SetupBuildGradle(projectPath);
            SetOptionalAARFile(projectPath);
        }

        private static void SetOptionalAARFile(string projectPath) {
            DirectoryInfo androidPath = new DirectoryInfo(projectPath);
            var files = androidPath.GetFiles("build.gradle");
            var file = files.First();

            var gradle = new GradleConfig(file.FullName);
            var depNode = gradle.ROOT.FindChildNodeByName("dependencies");
            var isDebug = EditorUserBuildSettings.development || EditorUserBuildSettings.androidBuildType == AndroidBuildType.Debug;
            var allAARFiles = Directory.GetFiles(AAR_ORIGIN_PATH).Where((dirPath) => {
                return isDebug ? dirPath.EndsWith("-debug.aar") : (!dirPath.EndsWith("-debug.aar") && dirPath.EndsWith(".aar"));
            }).Select((dirPath) => {
                return new FileInfo(dirPath);
            });

            foreach (var aarFile in allAARFiles) {
                var targetPath = Path.Combine(Path.Combine(projectPath, AAR_TARGET_PATH), aarFile.Name);
                Debug.Log("aar path: " + aarFile.FullName + " lib path: " + targetPath);

                var targetDirPath = Path.GetDirectoryName(targetPath);
                if (!Directory.Exists(targetDirPath)) {
                    Directory.CreateDirectory(targetDirPath);
                }

                if (File.Exists(targetPath)) {
                    FileUtil.DeleteFileOrDirectory(targetPath);
                }
                // Copy raw from source...
                File.Copy(aarFile.FullName, targetPath);
                depNode.AppendContentNode("implementation(name: '" + Path.GetFileNameWithoutExtension(aarFile.FullName) + "', ext:'aar')");
            }
            gradle.Save();
        }

        private static void SetupGradleProperties(string projectPath) {
            DirectoryInfo androidPath = new DirectoryInfo(projectPath);
            // 获取 gradle.properties 文件
            var files = androidPath.Parent.GetFiles("gradle.properties");

            // 不存在则创建
            if (files.Length <= 0) {
                var propPath = Path.Combine(androidPath.Parent.FullName, "gradle.properties");
                var propFile = File.Create(propPath);
                propFile.Flush();
                propFile.Close();
            }
            // 存在则获取首个文件并转为 FileInfo 对象
            var file = files.First();

            // 创建临时文件对象
            var tempFile = new FileInfo(projectPath + "/properties.temp");

            // 创建临时解析数据对象
            Dictionary<string, string> properties = new Dictionary<string, string>();

            // 解析文件内容并添加到解析对象
            using (StreamReader sr = file.OpenText()) {
                string line;
                while ((line = sr.ReadLine()) != null) {
                    var props = line.Split('=');
                    if (props.Length >= 2) {
                        properties.Add(props[0], props[1]);
                    }
                }
            }

            // 配置相关参数
            properties["android.useAndroidX"] = "true";

            // 写入临时配置文件
            using (StreamWriter sw = tempFile.CreateText()) {

                foreach (var item in properties) {
                    sw.WriteLine($"{item.Key}={item.Value}");
                }
            }

            // 删除源文件
            file.Delete();
            // 移动临时文件到目标位置
            tempFile.MoveTo(file.FullName);

        }

        private static void SetupBuildGradle(string projectPath) {
            DirectoryInfo androidPath = new DirectoryInfo(projectPath);
            var files = androidPath.GetFiles("build.gradle");
            var file = files.First();

            var tempFile = new FileInfo(projectPath + "/build.temp");

            using (StreamWriter sw = tempFile.CreateText()) {

                foreach (var line in File.ReadLines(file.FullName)) {

                    if (line.Contains("dependencies")) {
                        sw.WriteLine(line);
                        sw.WriteLine("    implementation('com.google.android.gms:play-services-ads-identifier:18.0.1') {");
                        sw.WriteLine("        exclude group: 'androidx.annotation'");
                        sw.WriteLine("        exclude group: 'androidx.core'");
                        sw.WriteLine("        exclude group: 'androidx.fragment'");
                        sw.WriteLine("        exclude group: 'androidx.collection'");
                        sw.WriteLine("    }");
                        sw.WriteLine("    implementation 'androidx.constraintlayout:constraintlayout:1.1.3'");
                        sw.WriteLine("    implementation 'org.jetbrains.kotlin:kotlin-stdlib-jdk7:1.4.10'");
                        sw.WriteLine("    implementation 'org.jetbrains.kotlinx:kotlinx-coroutines-core:1.3.9'");
                        sw.WriteLine("    implementation 'org.jetbrains.kotlinx:kotlinx-coroutines-android:1.3.9'");
                        sw.WriteLine("    implementation 'com.google.android.material:material:1.1.0'");
                        sw.WriteLine();
                    }
                    else {
                        sw.WriteLine(line);
                    }
                }
            }

            file.Delete();
            tempFile.MoveTo(tempFile.Directory.FullName + "/build.gradle");
        }
    }
}

#endif

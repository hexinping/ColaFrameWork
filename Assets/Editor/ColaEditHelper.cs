//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using ColaFramework.Foundation;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Plugins.XAsset;

namespace ColaFramework.ToolKit
{
    /// <summary>
    /// ColaFramework 编辑器助手类
    /// </summary>
    public class ColaEditHelper
    {
        public static string overloadedDevelopmentServerURL = "";

        /// <summary>
        /// 编辑器会用到的一些临时目录
        /// </summary>
        public static string TempCachePath
        {
            get { return Path.Combine(Application.dataPath, "../ColaCache"); }
        }

        public static string ProjectRoot
        {
            get { return Path.GetDirectoryName(Application.dataPath); }
        }

        /// <summary>
        /// 打开指定文件夹(编辑器模式下)
        /// </summary>
        /// <param name="path"></param>
        public static void OpenDirectory(string path)
        {
            if (string.IsNullOrEmpty(path)) return;

            path = path.Replace("/", "\\");
            if (!Directory.Exists(path))
            {
                Debug.LogError("No Directory: " + path);
                return;
            }
            if (!path.StartsWith("file://"))
            {
                path = "file://" + path;
            }

            Application.OpenURL(path);
        }

        public static T GetScriptableObjectAsset<T>(string path) where T : ScriptableObject
        {
            var asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<T>();
                AssetDatabase.CreateAsset(asset, path);
                AssetDatabase.SaveAssets();
            }

            return asset;
        }



        #region 打包相关方法实现

        public static void CopyAssetBundlesTo(string outputPath)
        {
            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);
            var source = Path.Combine(Environment.CurrentDirectory, Utility.AssetBundles, GetPlatformName());
            if (!Directory.Exists(source))
                Debug.Log("No assetBundle output folder, try to build the assetBundles first.");
            if (Directory.Exists(outputPath))
                FileUtil.DeleteFileOrDirectory(outputPath);
            FileUtil.CopyFileOrDirectory(source, outputPath);
        }

        public static string GetPlatformName()
        {
            return GetPlatformForAssetBundles(EditorUserBuildSettings.activeBuildTarget);
        }

        private static string GetPlatformForAssetBundles(BuildTarget target)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (target)
            {
                case BuildTarget.Android:
                    return "Android";
                case BuildTarget.iOS:
                    return "iOS";
                case BuildTarget.WebGL:
                    return "WebGL";
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    return "Windows";
#if UNITY_2017_3_OR_NEWER
                case BuildTarget.StandaloneOSX:
                    return "OSX";
#else
                case BuildTarget.StandaloneOSXIntel:
                case BuildTarget.StandaloneOSXIntel64:
                case BuildTarget.StandaloneOSX:
                    return "OSX";
#endif
                default:
                    return null;
            }
        }

        private static string[] GetLevelsFromBuildSettings()
        {
            return EditorBuildSettings.scenes.Select(scene => scene.path).ToArray();
        }

        private static string GetAssetBundleManifestFilePath()
        {
            var relativeAssetBundlesOutputPathForPlatform = Path.Combine(Utility.AssetBundles, GetPlatformName());
            return Path.Combine(relativeAssetBundlesOutputPathForPlatform, GetPlatformName()) + ".manifest";
        }

        public static void BuildStandalonePlayer()
        {
            var outputPath = EditorUtility.SaveFolderPanel("Choose Location of the Built Game", "", "");
            if (outputPath.Length == 0)
                return;

            var levels = GetLevelsFromBuildSettings();
            if (levels.Length == 0)
            {
                Debug.Log("Nothing to build.");
                return;
            }

            var targetName = GetBuildTargetName(EditorUserBuildSettings.activeBuildTarget);
            if (targetName == null)
                return;
#if UNITY_5_4 || UNITY_5_3 || UNITY_5_2 || UNITY_5_1 || UNITY_5_0
			BuildOptions option = EditorUserBuildSettings.development ? BuildOptions.Development : BuildOptions.None;
			BuildPipeline.BuildPlayer(levels, outputPath + targetName, EditorUserBuildSettings.activeBuildTarget, option);
#else
            var buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = levels,
                locationPathName = outputPath + targetName,
                assetBundleManifestPath = GetAssetBundleManifestFilePath(),
                target = EditorUserBuildSettings.activeBuildTarget,
                options = EditorUserBuildSettings.development ? BuildOptions.Development : BuildOptions.None
            };
            BuildPipeline.BuildPlayer(buildPlayerOptions);
#endif
        }

        public static string CreateAssetBundleDirectory()
        {
            // Choose the output path according to the build target.
            var outputPath = Path.Combine(Utility.AssetBundles, GetPlatformName());
            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);

            return outputPath;
        }

        private static Dictionary<string, string> GetVersions(AssetBundleManifest manifest)
        {
            var items = manifest.GetAllAssetBundles();
            return items.ToDictionary(item => item, item => manifest.GetAssetBundleHash(item).ToString());
        }

        private static void LoadVersions(string versionsTxt, string path, IDictionary<string, ABFileInfo> versions)
        {
            if (versions == null)
                throw new ArgumentNullException("versions");
            if (!File.Exists(versionsTxt))
                return;
            using (var s = new StreamReader(versionsTxt))
            {
                string line;
                while ((line = s.ReadLine()) != null)
                {
                    if (line == string.Empty)
                        continue;
                    var fields = line.Split(':');
                    if (fields.Length > 1)
                    {
                        var abInfo = new ABFileInfo();
                        abInfo.filename = fields[0];
                        abInfo.md5 = fields[1];
                        abInfo.rawSize = FileHelper.GetFileSizeKB(path + "/" + fields[0]);
                        abInfo.compressSize = abInfo.rawSize;
                        versions.Add(fields[0], abInfo);
                    }
                }
            }
        }

        private static void SaveVersions(string versionsTxt, string path, Dictionary<string, string> versions)
        {
            if (File.Exists(versionsTxt))
                File.Delete(versionsTxt);
            using (var s = new StreamWriter(versionsTxt))
            {
                foreach (var item in versions)
                {
                    var fileSize = FileHelper.GetFileSizeKB(path + "/" + item.Key);
                    s.WriteLine(item.Key + ':' + item.Value + ":" + fileSize + ":" + fileSize);
                }
                s.Flush();
                s.Close();
            }
        }

        public static void RemoveUnusedAssetBundleNames()
        {
            AssetDatabase.RemoveUnusedAssetBundleNames();
        }

        public static void SetAssetBundleNameAndVariant(string assetPath, string bundleName, string variant)
        {
            var importer = AssetImporter.GetAtPath(assetPath);
            if (importer == null) return;
            importer.assetBundleName = bundleName;
            importer.assetBundleVariant = variant;
        }

        /// <summary>
        /// 生成asset与abpath映射Manifest
        /// </summary>
        public static void BuildManifest()
        {
            var manifest = GetManifest();

            var assetPath = TrimedAssetDirName(AssetDatabase.GetAssetPath(manifest));
            var bundleName = Path.GetFileNameWithoutExtension(assetPath).ToLower();
            SetAssetBundleNameAndVariant(assetPath, bundleName, null);

            AssetDatabase.RemoveUnusedAssetBundleNames();
            var bundles = AssetDatabase.GetAllAssetBundleNames();

            List<string> dirs = new List<string>();
            List<AssetData> assets = new List<AssetData>();

            for (int i = 0; i < bundles.Length; i++)
            {
                if (bundles[i].StartsWith(AppConst.LuaBundlePrefix))
                {
                    //Lua AssetBundle不需要生成映射Manifest
                    continue;
                }
                var paths = AssetDatabase.GetAssetPathsFromAssetBundle(bundles[i]);
                foreach (var path in paths)
                {
                    var dir = TrimedAssetDirName(path);
                    var index = dirs.FindIndex((o) => o.Equals(dir));
                    if (index == -1)
                    {
                        index = dirs.Count;
                        dirs.Add(dir);
                    }

                    var asset = new AssetData();
                    asset.bundle = i;
                    asset.dir = index;
                    asset.name = Path.GetFileName(path);

                    assets.Add(asset);
                }
            }

            manifest.bundles = bundles;
            manifest.dirs = dirs.ToArray();
            manifest.assets = assets.ToArray();

            EditorUtility.SetDirty(manifest);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static string TrimedAssetDirName(string assetDirName)
        {
            assetDirName = assetDirName.Replace("\\", "/");
            assetDirName = assetDirName.Replace(Constants.GameAssetBasePath, "");
            return Path.GetDirectoryName(assetDirName).Replace("\\", "/"); ;
        }

        public static void BuildAssetBundles()
        {
            // Choose the output path according to the build target.
            var outputPath = CreateAssetBundleDirectory();

            const BuildAssetBundleOptions options = BuildAssetBundleOptions.ChunkBasedCompression;

            var manifest =
                BuildPipeline.BuildAssetBundles(outputPath, options,
                    EditorUserBuildSettings.activeBuildTarget);
            var versionsTxt = outputPath + "/versions.txt";
            var versions = new Dictionary<string, ABFileInfo>();
            LoadVersions(versionsTxt, outputPath, versions);

            var buildVersions = GetVersions(manifest);

            var updates = new List<string>();

            foreach (var item in buildVersions)
            {
                ABFileInfo abInfo;
                var isNew = true;
                if (versions.TryGetValue(item.Key, out abInfo))
                {
                    string hash = abInfo.md5;
                    if (hash.Equals(item.Value))
                        isNew = false;
                }
                if (isNew)
                    updates.Add(item.Key);
            }

            if (updates.Count > 0)
            {
                using (var s = new StreamWriter(File.Open(outputPath + "/updates.txt", FileMode.Append)))
                {
                    s.WriteLine(DateTime.Now.ToFileTime() + ":");
                    foreach (var item in updates)
                        s.WriteLine(item);
                    s.Flush();
                    s.Close();
                }

                SaveVersions(versionsTxt, outputPath, buildVersions);
            }
            else
            {
                Debug.Log("nothing to update.");
            }

            string[] ignoredFiles = { GetPlatformName(), "versions.txt", "updates.txt", "manifest" };

            var files = Directory.GetFiles(outputPath, "*", SearchOption.AllDirectories);

            var deletes = (from t in files
                           let file = t.Replace('\\', '/').Replace(outputPath.Replace('\\', '/') + '/', "")
                           where !file.EndsWith(".manifest", StringComparison.Ordinal) && !Array.Exists(ignoredFiles, s => s.Equals(file))
                           where !buildVersions.ContainsKey(file)
                           select t).ToList();

            foreach (var delete in deletes)
            {
                if (!File.Exists(delete))
                    continue;
                File.Delete(delete);
                File.Delete(delete + ".manifest");
            }

            deletes.Clear();
        }

        private static string GetBuildTargetName(BuildTarget target)
        {
            var name = PlayerSettings.productName + "_" + PlayerSettings.bundleVersion;
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (target)
            {
                case BuildTarget.Android:
                    return "/" + name + PlayerSettings.Android.bundleVersionCode + ".apk";

                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    return "/" + name + PlayerSettings.Android.bundleVersionCode + ".exe";

#if UNITY_2017_3_OR_NEWER
                case BuildTarget.StandaloneOSX:
                    return "/" + name + ".app";

#else
                    case BuildTarget.StandaloneOSXIntel:
                    case BuildTarget.StandaloneOSXIntel64:
                    case BuildTarget.StandaloneOSX:
                                        return "/" + name + ".app";

#endif

                case BuildTarget.WebGL:
                case BuildTarget.iOS:
                    return "";
                // Add more build targets for your own.
                default:
                    Debug.Log("Target not implemented.");
                    return null;
            }
        }

        private static T GetAsset<T>(string path) where T : ScriptableObject
        {
            var asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<T>();
                AssetDatabase.CreateAsset(asset, path);
                AssetDatabase.SaveAssets();
            }

            return asset;
        }

        public static AssetsManifest GetManifest()
        {
            return GetAsset<AssetsManifest>(Utility.AssetsManifestAsset);
        }

        public static string GetServerURL()
        {
            string downloadURL;
            if (string.IsNullOrEmpty(overloadedDevelopmentServerURL) == false)
            {
                downloadURL = overloadedDevelopmentServerURL;
            }
            else
            {
                IPHostEntry host;
                string localIP = "";
                host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (IPAddress ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        localIP = ip.ToString();
                        break;
                    }
                }

                downloadURL = "http://" + localIP + ":7888/";
            }

            return downloadURL;
        }

        #region 处理Lua代码
        public static void BuildLuaBundle()
        {
            //合并Lua代码，并复制到临时目录中准备打包
            FileHelper.RmDir(LuaConst.luaTempDir);
            FileHelper.EnsureParentDirExist(LuaConst.luaTempDir);

            string[] srcDirs = { LuaConst.toluaDirWithSpliter, LuaConst.luaDirWithSpliter };
            for (int i = 0; i < srcDirs.Length; i++)
            {
                if (AppConst.LuaByteMode)
                {
                    string sourceDir = srcDirs[i];
                    string[] files = Directory.GetFiles(sourceDir, "*.lua", SearchOption.AllDirectories);
                    int len = sourceDir.Length;

                    if (sourceDir[len - 1] == '/' || sourceDir[len - 1] == '\\')
                    {
                        --len;
                    }
                    for (int j = 0; j < files.Length; j++)
                    {
                        string str = files[j].Remove(0, len);
                        string dest = LuaConst.luaTempDir + str + ".bytes";
                        string dir = Path.GetDirectoryName(dest);
                        Directory.CreateDirectory(dir);
                        EncodeLuaFile(files[j], dest);
                    }
                }
                else
                {
                    string[] files = FileHelper.GetAllChildFiles(srcDirs[i], "lua");

                    foreach (var fileName in files)
                    {
                        var reltaFileName = fileName.Replace(srcDirs[i], "");
                        var dirName = Path.GetDirectoryName(reltaFileName);
                        if (!string.IsNullOrEmpty(dirName))
                        {
                            dirName = dirName.Replace("\\", "/");
                            if (!dirName.EndsWith("/"))
                            {
                                dirName += "/";
                            }
                            dirName = dirName.Replace("/", ".");
                        }
                        var dest = LuaConst.luaTempDir + dirName + Path.GetFileName(reltaFileName) + ".bytes";
                        File.Copy(fileName, dest, true);
                    }
                }
            }
            //标记ABName
            MarkAssetsToOneBundle(LuaConst.luaTempDir, AppConst.LuaBaseBundle);
            AssetDatabase.Refresh();
        }

        public static void BuildLuaFile()
        {
            //合并Lua代码，并复制到StreamingAsset目录中准备打包
            FileHelper.RmDir(LuaConst.streamingAssetLua);
            FileHelper.EnsureParentDirExist(LuaConst.streamingAssetLua);

            string[] luaPaths = { LuaConst.toluaDirWithSpliter, LuaConst.luaDirWithSpliter };

            var paths = new List<string>();
            var files = new List<string>();

            for (int i = 0; i < luaPaths.Length; i++)
            {
                paths.Clear(); files.Clear();
                string luaDataPath = luaPaths[i].ToLower();
                FileHelper.Recursive(luaDataPath, files, paths);
                foreach (string f in files)
                {
                    if (f.EndsWith(".meta")) continue;
                    string newfile = f.Replace(luaDataPath, "");
                    string newpath = LuaConst.streamingAssetLuaWithSpliter + newfile;
                    string path = Path.GetDirectoryName(newpath);
                    if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                    if (File.Exists(newpath))
                    {
                        File.Delete(newpath);
                    }
                    if (AppConst.LuaByteMode)
                    {
                        EncodeLuaFile(f, newpath);
                    }
                    else
                    {
                        File.Copy(f, newpath, true);
                    }
                    EditorUtility.DisplayProgressBar("玩命处理中", string.Format("正在处理第{0}Lua文件目录 {1}/{2}", i, i, luaPaths.Length), i * 1.0f / luaPaths.Length);
                }
            }
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }

        public static void EncodeLuaFile(string srcFile, string outFile)
        {
            //if (!srcFile.ToLower().EndsWith(".lua"))
            //{
            //    File.Copy(srcFile, outFile, true);
            //    return;
            //}
            //bool isWin = true;
            //string luaexe = string.Empty;
            //string args = string.Empty;
            //string exedir = string.Empty;
            //string currDir = Directory.GetCurrentDirectory();
            //if (Application.platform == RuntimePlatform.WindowsEditor)
            //{
            //    isWin = true;
            //    luaexe = "luajit.exe";
            //    args = "-b -g " + srcFile + " " + outFile;
            //    exedir = AppDataPath.Replace("assets", "") + "LuaEncoder/luajit/";
            //}
            //else if (Application.platform == RuntimePlatform.OSXEditor)
            //{
            //    isWin = false;
            //    luaexe = "./luajit";
            //    args = "-b -g " + srcFile + " " + outFile;
            //    exedir = AppDataPath.Replace("assets", "") + "LuaEncoder/luajit_mac/";
            //}
            //Directory.SetCurrentDirectory(exedir);
            //ProcessStartInfo info = new ProcessStartInfo();
            //info.FileName = luaexe;
            //info.Arguments = args;
            //info.WindowStyle = ProcessWindowStyle.Hidden;
            //info.UseShellExecute = isWin;
            //info.ErrorDialog = true;
            //Util.Log(info.FileName + " " + info.Arguments);

            //Process pro = Process.Start(info);
            //pro.WaitForExit();
            //Directory.SetCurrentDirectory(currDir);
        }

        #endregion
        /// <summary>
        /// 清除所有的AB Name
        /// </summary>
        public static void ClearAllAssetBundleName()
        {
            string[] oldAssetBundleNames = AssetDatabase.GetAllAssetBundleNames();
            var length = oldAssetBundleNames.Length;
            for (int i = 0; i < length; i++)
            {
                EditorUtility.DisplayProgressBar("清除AssetBundleName", "正在清除AssetBundleName", i * 1f / length);
                AssetDatabase.RemoveAssetBundleName(oldAssetBundleNames[i], true);
            }
            EditorUtility.ClearProgressBar();
        }

        /// <summary>
        /// 标记一个文件夹下所有文件为一个BundleName
        /// </summary>
        /// <param name="path"></param>
        /// <param name="bundleName"></param>
        public static void MarkAssetsToOneBundle(string path, string bundleName)
        {
            if (Directory.Exists(path))
            {
                bundleName = bundleName.ToLower();
                var files = FileHelper.GetAllChildFiles(path);
                var projRoot = FileHelper.FormatPath(ProjectRoot) + "/";
                var length = files.Length;
                for (int i = 0; i < length; i++)
                {
                    EditorUtility.DisplayProgressBar("玩命处理中", string.Format("正在标记第{0}个文件... {1}/{2}", i, i, length), i * 1.0f / length);
                    var assetPath = files[i].Replace(projRoot, "");
                    SetAssetBundleNameAndVariant(assetPath, bundleName, null);
                }
                EditorUtility.ClearProgressBar();
            }
        }
        #endregion
    }
}

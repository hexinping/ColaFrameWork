//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Text;
using Plugins.XAsset;
using ColaFramework.Foundation;
using LitJson;

namespace ColaFramework.ToolKit
{
    public enum EnvOption
    {
        MOTHER_PKG,
        HOT_UPDATE_BUILD,
    }

    /// <summary>
    /// ColaFramework框架的打包脚本
    /// </summary>
    public static class ColaBuildTool
    {
        private const string AppVersionPath = "Assets/Editor/Settings/AppVersion.asset";
        private const string Resource_AppVersionPath = "Assets/Resources/app_version.json";
        private const string AppVersionFileName = "app_version.json";
        private const string Resource_VersionPath = "Assets/Resources/versions.txt";
        private const string CDNVersionControlUrl = "CDN/versioncontrol/{0}/{1}";
        private const string CDNResourceUrl = "CDN/cdn/";

        private static Dictionary<EnvOption, string> internalEnvMap = new Dictionary<EnvOption, string>();

        #region BuildPlayer接口
        public static string BuildPlayer(BuildTarget buildTarget)
        {
            StringBuilder buildReport = new StringBuilder();

            //0.根据buildTarget区分BuildGroup
            BuildTargetGroup buildTargetGroup = HandleBuildGroup(buildTarget);
            if (BuildTargetGroup.Unknown == buildTargetGroup)
            {
                throw new System.Exception(string.Format("{0} is Unknown Build Platform ! Build Failture!", buildTarget));
            }
            try
            {
                //1.首先确认各种环境变量和配置到位
                InitBuildEnvironment(buildTargetGroup);

                //2.设置参数
                SetBuildParams(buildTargetGroup);

                //3.自动化接入SDK
                BuildSDK(buildTargetGroup);

                //4.处理Lua文件
                BuildLua(buildTargetGroup);

                //5.打Bundle
                BuildAssetBundle(buildTargetGroup);

                //6.自动处理AppVersion
                BuildAppVersion();

                //7.UpLoadCDN
                UpLoadCDN(buildTargetGroup);

                //8.出包
                InternalBuildPkg(buildTargetGroup);
            }
            catch
            {
                throw;
            }
            finally
            {
                //8.清理工作，恢复工作区
                CleanUp(buildTargetGroup);
            }


            return buildReport.ToString();
        }

        /// <summary>
        /// 初始化各种基本的路径和SDK、JDK等必要的打包配置环境
        /// </summary>
        /// <param name="buildTargetGroup"></param>
        private static void InitBuildEnvironment(BuildTargetGroup buildTargetGroup)
        {

        }

        /// <summary>
        /// 处理非侵入式SDK的接入
        /// </summary>
        /// <param name="buildTargetGroup"></param>
        private static void BuildSDK(BuildTargetGroup buildTargetGroup)
        {

        }

        /// <summary>
        /// 用来设置一些编译的宏和参数等操作
        /// </summary>
        /// <param name="buildTargetGroup"></param>
        private static void SetBuildParams(BuildTargetGroup buildTargetGroup)
        {

        }

        /// <summary>
        /// 清理与恢复工作区
        /// </summary>
        /// <param name="buildTargetGroup"></param>
        private static void CleanUp(BuildTargetGroup buildTargetGroup)
        {

        }

        /// <summary>
        /// 最后的出包环节
        /// </summary>
        /// <param name="buildTargetGroup"></param>
        private static void InternalBuildPkg(BuildTargetGroup buildTargetGroup)
        {
            var beginTime = System.DateTime.Now;
            if (!ContainsEnvOption(EnvOption.HOT_UPDATE_BUILD))
            {
                ColaEditHelper.BuildStandalonePlayer(ColaEditHelper.ProjectRoot + "/Build");
            }
            Debug.Log("=================Build Pkg Time================ : " + (System.DateTime.Now - beginTime).TotalSeconds);
        }

        /// <summary>
        /// Build AssetBunle
        /// </summary>
        /// <param name="buildTargetGroup"></param>
        private static void BuildAssetBundle(BuildTargetGroup buildTargetGroup)
        {
            var beginTime = System.DateTime.Now;
            AssetBundleAnalyzer.AutoAnalyzeAssetBundleName();
            Debug.Log("=================Build AutoAnalyzeAssetBundleName Time================ : " + (System.DateTime.Now - beginTime).TotalSeconds);

            beginTime = System.DateTime.Now;
            ColaEditHelper.BuildManifest();
            ColaEditHelper.BuildAssetBundles();

            var isHotUpdateBuild = ContainsEnvOption(EnvOption.HOT_UPDATE_BUILD);
            if (!isHotUpdateBuild)
            {
                ColaEditHelper.CopyAssetBundlesTo(Path.Combine(Application.streamingAssetsPath, Utility.AssetBundles));
                AssetDatabase.Refresh();

                BuildVideoFiles();
            }
            Debug.Log("=================Build BuildAssetBundle Time================ : " + (System.DateTime.Now - beginTime).TotalSeconds);
        }

        /// <summary>
        /// 处理视频文件
        /// </summary>
        private static void BuildVideoFiles()
        {
            FileHelper.CopyDir("Assets/RawAssets/Videos/", Application.streamingAssetsPath + "/Videos/");
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Build Lua
        /// </summary>
        /// <param name="buildTargetGroup"></param>
        private static void BuildLua(BuildTargetGroup buildTargetGroup)
        {
            var beginTime = System.DateTime.Now;
            var isMotherPkg = ContainsEnvOption(EnvOption.MOTHER_PKG);
            var isHotUpdateBuild = ContainsEnvOption(EnvOption.HOT_UPDATE_BUILD);
            if (AppConst.LuaBundleMode)
            {
                ColaEditHelper.BuildLuaBundle(isMotherPkg, isHotUpdateBuild);
            }
            else
            {
                ColaEditHelper.BuildLuaFile(isMotherPkg, isHotUpdateBuild);
            }
            Debug.Log("=================Build Lua Time================ : " + (System.DateTime.Now - beginTime).TotalSeconds);
        }

        /// <summary>
        /// UpLoadCDN
        /// </summary>
        /// <param name="buildTargetGroup"></param>
        private static void UpLoadCDN(BuildTargetGroup buildTargetGroup)
        {
            var beginTime = System.DateTime.Now;
            var isMotherPkg = ContainsEnvOption(EnvOption.MOTHER_PKG);
            var isHotUpdateBuild = ContainsEnvOption(EnvOption.HOT_UPDATE_BUILD);

            if (isHotUpdateBuild || isMotherPkg)
            {
                //upload appversion.json
                var cachePath = ColaEditHelper.TempCachePath + "/" + AppVersionFileName;
                var CDN_AppVersionPath = string.Format(CDNVersionControlUrl, ColaEditHelper.GetPlatformName(), "app_version.json");
                FileHelper.CopyFile(cachePath, CDN_AppVersionPath, true);

                //upload version.txt and assets
                var reltaRoot = ColaEditHelper.CreateAssetBundleDirectory();
                var updateFilePath = reltaRoot + "/updates.txt";
                using (var sr = new StreamReader(updateFilePath))
                {
                    var content = sr.ReadLine();
                    while (null != content)
                    {
                        var reltaPath = reltaRoot + "/" + content;
                        var destPath = CDNResourceUrl + content;
                        FileHelper.CopyFile(reltaPath, destPath, true);
                        content = sr.ReadLine();
                    }
                }
                FileHelper.CopyFile(reltaRoot + "/versions.txt", CDNResourceUrl + "versions.txt", true);
            }
            Debug.Log("=================UpLoadCDN Time================ : " + (System.DateTime.Now - beginTime).TotalSeconds);
        }

        /// <summary>
        /// 自动处理AppVersion
        /// </summary>
        private static void BuildAppVersion()
        {
            var isMotherPkg = ContainsEnvOption(EnvOption.MOTHER_PKG);
            var isHotUpdateBuild = ContainsEnvOption(EnvOption.HOT_UPDATE_BUILD);

            var appAsset = ColaEditHelper.GetScriptableObjectAsset<AppVersion>(AppVersionPath);
            if (isHotUpdateBuild)
            {
                appAsset.HotUpdateVersion += 1;
            }
            else if (isMotherPkg)
            {
                appAsset.HotUpdateVersion = 0;
                appAsset.StoreVersion += 1;
                appAsset.BuildVersion = 0;
            }
            if (!isMotherPkg && !isHotUpdateBuild)
            {
                appAsset.BuildVersion += 1;
            }
            appAsset.OnValueChanged();
            EditorUtility.SetDirty(appAsset);
            AssetDatabase.SaveAssets();

            if (null != appAsset)
            {
                var jsonStr = JsonMapper.ToJson(appAsset);
                var cachePath = ColaEditHelper.TempCachePath + "/" + AppVersionFileName;
                FileHelper.DeleteFile(cachePath);
                FileHelper.WriteString(cachePath, jsonStr);
                if (!isHotUpdateBuild)
                {
                    FileHelper.CopyFile(cachePath, Resource_VersionPath, true);
                }
            }

            if (!isHotUpdateBuild)
            {
                FileHelper.DeleteFile(Resource_VersionPath);
                var outputPath = ColaEditHelper.CreateAssetBundleDirectory();
                var versionsTxt = outputPath + "/versions.txt";
                FileHelper.CopyFile(versionsTxt, Resource_VersionPath, true);
            }

            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 对打包平台进行分组
        /// </summary>
        /// <param name="buildTarget"></param>
        /// <returns></returns>
        private static BuildTargetGroup HandleBuildGroup(BuildTarget buildTarget)
        {
            var buildTargetGroup = BuildTargetGroup.Unknown;
            if (BuildTarget.Android == buildTarget)
            {
                buildTargetGroup = BuildTargetGroup.Android;
            }
            else if (BuildTarget.iOS == buildTarget)
            {
                buildTargetGroup = BuildTargetGroup.iOS;
            }
            else if (BuildTarget.StandaloneWindows == buildTarget || BuildTarget.StandaloneWindows64 == buildTarget)
            {
                buildTargetGroup = BuildTargetGroup.Standalone;
            }
            return buildTargetGroup;
        }
        #endregion

        #region 环境变量
        public static string GetEnvironmentVariable(EnvOption envOption)
        {
            return internalEnvMap.ContainsKey(envOption) ? internalEnvMap[envOption] : Environment.GetEnvironmentVariable(envOption.ToString()) ?? string.Empty;
        }

        public static void SetEnvironmentVariable(EnvOption envOption, string value, bool isAppend)
        {
            string oldValue = GetEnvironmentVariable(envOption);
            if (!isAppend)
            {
                oldValue = value;
            }
            else
            {
                oldValue = string.IsNullOrEmpty(oldValue) ? value : (oldValue + ";" + value);
            }
            if (!internalEnvMap.ContainsKey(envOption))
            {
                internalEnvMap.Add(envOption, oldValue);
            }
            else
            {
                internalEnvMap[envOption] = oldValue;
            }
        }

        public static bool ContainsEnvOption(EnvOption envOption)
        {
            string envVar = GetEnvironmentVariable(envOption);
            if (string.IsNullOrEmpty(envVar) || 0 == string.Compare(envVar, "false", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            return true;
        }

        public static void ClearEnvironmentVariable()
        {
            internalEnvMap.Clear();
        }
        #endregion
    }
}
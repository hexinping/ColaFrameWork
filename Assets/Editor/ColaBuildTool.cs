//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEditor;
using ColaFramework;

/// <summary>
/// ColaFramework框架的打包脚本
/// </summary>
public class ColaBuildTool
{

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

            //4.打Bundle
            BuildAssetBundle(buildTargetGroup);

            //5.出包
            InternalBuildPkg(buildTargetGroup);
        }
        catch
        {
            throw;
        }
        finally
        {
            //6.清理工作，恢复工作区
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

    }

    /// <summary>
    /// Build AssetBunle
    /// </summary>
    /// <param name="buildTargetGroup"></param>
    private static void BuildAssetBundle(BuildTargetGroup buildTargetGroup)
    {

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
}

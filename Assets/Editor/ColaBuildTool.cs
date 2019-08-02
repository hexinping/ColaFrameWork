using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEditor;
using ColaFramework;

/// <summary>
/// ColaFramework框架的打包脚本
/// </summary>
public class ColaBuildTool {

    public static string BuildPlayer(BuildTarget buildTarget)
    {
        StringBuilder buildReport = new StringBuilder();
        return buildReport.ToString();
    }

    /// <summary>
    /// 初始化各种基本的路径和SDK、JDK等必要的打包配置环境
    /// </summary>
    /// <param name="buildTargetGroup"></param>
    private void InitBuildEnvironment(BuildTargetGroup buildTargetGroup)
    {

    }

    /// <summary>
    /// 处理非侵入式SDK的接入
    /// </summary>
    /// <param name="buildTargetGroup"></param>
    private void BuildSDK(BuildTargetGroup buildTargetGroup)
    {

    }

    /// <summary>
    /// 清理与恢复工作区
    /// </summary>
    /// <param name="buildTargetGroup"></param>
    private void CleanUp(BuildTargetGroup buildTargetGroup)
    {

    }

    /// <summary>
    /// 最后的出包环节
    /// </summary>
    /// <param name="buildTargetGroup"></param>
    private void InternalBuildPkg(BuildTargetGroup buildTargetGroup)
    {

    }

    /// <summary>
    /// Build AssetBunle
    /// </summary>
    /// <param name="buildTargetGroup"></param>
    private void BuildAssetBundle(BuildTargetGroup buildTargetGroup)
    {

    }
}

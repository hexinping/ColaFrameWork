//using UnityEngine;
//using System.Collections.Generic;
//using System.IO;
//using ColaFramework.Foundation.DownLoad;

//namespace ColaFramework
//{
//    /// <summary>
//    /// 下载热更补丁的Patcher
//    /// </summary>
//    public class DownloadPatcher : UpdateTaskBase
//    {
//        #region Instance

//        static DownloadPatcher m_instance;
//        public static DownloadPatcher Instance
//        {
//            get
//            {
//                if (m_instance == null)
//                {
//                    m_instance = new DownloadPatcher();
//                }
//                return m_instance;
//            }
//        }
//        #endregion
//        private enum CheckState
//        {
//            None,
//            Checking,
//            Done,
//        }

//        public DownloadPatcher()
//        {
//            m_StopWhenFail = true;  //下载失败之后不继续其他任务
//        }

//        private System.Action<bool> m_onDownPatchDone;      //参数bool： 是否需要解压文件
//        private string m_strNewVersion = "";

//        Dictionary<string, ABFileInfo> m_dicSvrMd5;
//        Dictionary<string, ABFileInfo> m_dicLocalMd5;
//        List<ABFileInfo> m_lstDiffMd5;
//        int m_totalSize = 0;    // 总下载大小
//        int m_totalUnpackSize = 0;  // 解压后所需大小
//        int m_haveDownedSize = 0; // 已下载大小
//        int m_lastDownedSize = 0;
//        Object m_lockProgress;
//        Object m_lockMsg;
//        string m_strErrMsg = "";
//        string m_strDownloadUrl;        //热更下载url；
//        string m_strMainDownloadUrl;    //主热更下载地址
//        string m_strBakDownloadUrl;     //备用热更下载url
//        string m_strVersionInfoUrl;     //下载版本信息URL
//        float m_fStartDowndTime = 0;

//        public override void Reset()
//        {
//            m_haveDownedSize = 0;
//            m_onDownPatchDone = null;
//            m_strErrMsg = "";
//            if (m_dicSvrMd5 != null)
//            {
//                m_dicSvrMd5.Clear();
//            }
//            if (m_dicLocalMd5 != null)
//            {
//                m_dicLocalMd5.Clear();
//            }
//            if (m_lstDiffMd5 != null)
//            {
//                m_lstDiffMd5.Clear();
//            }

//            base.Reset();
//        }

//        // 检查是否有更新
//        // 这个接口外部会调用两次
//        public void StartUpdate(System.Action<bool> callback)
//        {
//            m_onDownPatchDone = callback;
//            if (!Config.Instance.CheckUpdate)
//            {
//                DoneWithNoDownload();
//                return;
//            }
//            m_strVersionInfoUrl = PathDef.URL_UPDATE_VERSION_URL;
//            CheckNeedUpdate();
//        }


//        private void CheckNeedUpdate()
//        {
//            EventMgr.onProgressTips.Invoke("检查版本信息中", false);
//            EventMgr.onProgressChange.Invoke(0);


//            string strVersion = "1.5.0.0";
//            string strURL = string.Format(m_strVersionInfoUrl, Config.PlatformName, strVersion);
//            Debug.LogFormat("Request Version URL:{0}", strURL);
//            m_fStartDowndTime = Time.time;
//            DownloadMgr.DownloadText(strURL, OnDownloadVersion, 2f);
//            // 没有服务器可以用下面这个结构做个测试
//            // string strContent="{\"v\": \"1.2.0.1\",\"data\": {\"url\": \"http://xxxxxxxx/update/ios/1.2.0.1\"},\"miniV\": \"1.2.1.2\",\"tipsV\": \"1.2.1.2\",\"content\":\"更新公告\",\"switch\":0}";
//            // OnDownloadVersion(ErrorCode.SUCCESS, "", strContent);
//        }

//        // 下载版本文件
//        private void OnDownloadVersion(ErrorCode code, string msg, string strContent)
//        {
//            EventMgr.onProgressChange.Invoke(0.3f);
//            if (code != ErrorCode.SUCCESS)  //失败的情况
//            {
//                if (code == ErrorCode.TIME_OUT)
//                {
//                    // 超时切换地址重试重试
//                    if (m_strVersionInfoUrl == PathDef.URL_UPDATE_VERSION_BAK_URL)
//                    {
//                        m_strVersionInfoUrl = PathDef.URL_UPDATE_VERSION_URL;
//                    }
//                    else
//                    {
//                        m_strVersionInfoUrl = PathDef.URL_UPDATE_VERSION_BAK_URL;
//                    }
//                    Debug.LogFormat("超时，切换地址重试，url:{0}", m_strVersionInfoUrl);
//                    CheckNeedUpdate();
//                    return;
//                }
//                else
//                {
//                    Debug.LogWarningFormat("Download version info fail, error:{0}", msg);
//                    if (m_strVersionInfoUrl != PathDef.URL_UPDATE_VERSION_BAK_URL)
//                    {
//                        Debug.Log("使用备用地址获取版本更新信息");
//                        m_strVersionInfoUrl = PathDef.URL_UPDATE_VERSION_BAK_URL;
//                        CheckNeedUpdate();
//                        return;
//                    }
//                }

//                string strTips = "获取最新版本信息失败，请检查网络后重试。";
//                string strBtn = "重试";
//                EventMgr.onLaunchConfirmTips(strTips, strBtn, () =>
//                {
//                    CheckNeedUpdate();
//                }, true);
//            }
//            else // 成功的情况
//            {
//                Debug.LogFormat("OnDownloadVersion success ：{0}", strContent);

//                JSONClass jsonVerInfo = null;
//                try
//                {
//                    Debug.LogFormat("---内容-:{0}", strContent);
//                    jsonVerInfo = JSONNode.Parse(strContent) as JSONClass;
//                }
//                catch (System.Exception ex)
//                {
//                    DoneWithNoDownload();
//                    Debug.LogErrorFormat("解析VersionInfo的JSon失败：{0}", ex.Message);
//                    return;
//                }
//                if (jsonVerInfo == null)
//                {
//                    Debug.LogWarningFormat("Parse version info fail, content:{0}", strContent);
//                    DoneWithNoDownload();
//                    return;
//                }

//                m_strNewVersion = jsonVerInfo["v"];            // 热更版本号 x1.x2.x3.x4
//                string strForceVersion = jsonVerInfo["miniV"];      // 强更版本号 x1.x2.x3.x4 如果不需要强更返回空
//                string strTipsVersion = jsonVerInfo["tipsV"];       // 最低弹窗版本 x1.x2.x3.x4
//                string strUpdateNotice = jsonVerInfo["content"];    // 更新公告
//                string strSwitch = jsonVerInfo["switch"];           // 是否提审0 关闭；1开启
//                int iSwitch = string.IsNullOrEmpty(strSwitch) ? 0 : int.Parse(strSwitch);
//                Config.Instance.IsSubmit = iSwitch == 1 ? true : false;

//                JSONClass jsonDataNode = jsonVerInfo["data"] as JSONClass;
//                if (jsonDataNode != null)
//                {
//                    m_strDownloadUrl = jsonDataNode["url"];
//                    m_strMainDownloadUrl = m_strDownloadUrl;
//                    m_strBakDownloadUrl = jsonDataNode["burl"];
//                }


//                Debug.LogFormat("GameNewVersionInfo,version:{0}, ForceVersion:{1}, TipVersion:{2}, url:{3}, burl:{4}",
//                    m_strNewVersion, strForceVersion, strTipsVersion, m_strDownloadUrl, m_strBakDownloadUrl);


//                // 当前版本号如果小于强更版本号，服务器才会返回这个字段
//                if (!string.IsNullOrEmpty(strForceVersion))
//                {
//                    NoticeForceUpdate(strUpdateNotice, false);
//                    return;
//                }

//                if (!string.IsNullOrEmpty(strTipsVersion))
//                {
//                    NoticeForceUpdate(strUpdateNotice, true);
//                    return;
//                }

//                DealWithHotFixStep();
//            }
//        }

//        private void DealWithHotFixStep()
//        {
//            string strCurrVersion = Config.Version;
//            // 有热更版本
//            if (strCurrVersion != m_strNewVersion)
//            {
//                Debug.Log("版本不一致，需要热更");
//                // 先当做热更处理
//                ResetPatchCachePath(m_strNewVersion);

//                //需要更新 开始校验MD5文件
//                DowndLoadMd5File();
//            }
//            else
//            {
//                // 不需要更新
//                DoneWithNoDownload();
//            }
//        }

//        // 强更提醒 有配置就用配置，没配置就用默认提醒
//        private void NoticeForceUpdate(string strUpdateNotice, bool canSkip)
//        {
//            System.Action onSkip = null;
//            if (canSkip)
//            {
//                onSkip = () =>
//                {
//                    DealWithHotFixStep();
//                };
//            }

//            Debug.LogFormat("强更提醒,canSkip:{0}, notice:{1}", canSkip, strUpdateNotice);
//            // string strTitle = "更新公告";
//            // EventMgr.onUpdateNotice(strTitle, strUpdateNotice, ()=>{
//            //     NoticeForceUpdate(strUpdateNotice, canSkip);
//            //     // GameHelper.ToAppStore();
//            // }, onSkip);
//        }

//        private void DoneWithNoDownload()
//        {
//            m_onDownPatchDone(false);
//            // EventMgr.onProgressChange.Invoke(1);
//        }

//        // 重新设置一下缓存路径
//        private void ResetPatchCachePath(string strNewVersion)
//        {
//            //updatecache目录的版本号如果和最新版本号不一致 要先清空缓存路径
//            if (!IsValueEqualPrefs(BaseDef.KEY_CACHE_HOTFIX_VERSION, strNewVersion))
//            {
//                FileHelper.DeleteDirectory(PathDef.UPDATE_CACHE_PATH);
//                FileHelper.CreateDirectory(PathDef.UPDATE_CACHE_PATH);
//                PlayerPrefs.SetString(BaseDef.KEY_CACHE_HOTFIX_VERSION, strNewVersion);
//            }
//        }

//        // 下载md5 文件
//        private void DowndLoadMd5File()
//        {
//            m_fStartDowndTime = Time.time;
//            Debug.LogFormat("---DowndLoadMd5File,  url:{0}", m_strDownloadUrl);
//            string strMd5URL = m_strDownloadUrl + "md5";
//            DownloadMgr.DownloadText(strMd5URL, OnDownloadMd5File);
//        }

//        // 下载md5文件
//        public void OnDownloadMd5File(ErrorCode code, string msg, string strText)
//        {
//            EventMgr.onProgressChange.Invoke(0.5f);
//            if (code != ErrorCode.SUCCESS)  //失败的情况
//            {
//                // 首次失败 尝试使用备用地址下载
//                if (m_strDownloadUrl != m_strBakDownloadUrl)
//                {
//                    Debug.Log("使用备用地址进行热更。");
//                    m_strDownloadUrl = m_strBakDownloadUrl;
//                    DowndLoadMd5File();
//                    return;
//                }

//                // 还是失败，弹提示，玩家自己控制重试，这个时候会切回使用主下载地址
//                Debug.LogErrorFormat("Download md5 file fail, error:{0}", msg);
//                string strTips = "下载更新文件失败，请重试";
//                string strBtn = "重试";
//                EventMgr.onLaunchConfirmTips(strTips, strBtn, () =>
//                {
//                    m_strDownloadUrl = m_strMainDownloadUrl;
//                    DowndLoadMd5File();
//                }, true);
//            }
//            else    // 成功的情况
//            {
//                Debug.Log("下载MD5文件成功。");
//                m_dicSvrMd5 = FileHelper.ReadABMD5FromText(strText);

//                int arch = Util.GetNotSystemArchType();
//                string strNoNeedLuaPkg = BaseDef.LUA_PACKAGE + arch + "." + BaseDef.AB_SUFFIX;
//                m_dicSvrMd5.Remove(strNoNeedLuaPkg);
//                string strNoNeedLuaUpdatePkg = BaseDef.LUA_UPDATE + arch + "." + BaseDef.AB_SUFFIX;
//                m_dicSvrMd5.Remove(strNoNeedLuaUpdatePkg);

//                CalDiffToDownload();
//            }
//        }

//        // 计算差异文件并开始下载
//        private void CalDiffToDownload()
//        {
//            m_lstDiffMd5 = m_lstDiffMd5 == null ? new List<ABFileInfo>() : m_lstDiffMd5;
//            m_lstDiffMd5.Clear();
//            m_totalSize = 0;
//            string strLocalMd5Path = PathDef.UPDATE_DATA_PATH + "/" + BaseDef.MD5_FILENAME;
//            Debug.LogFormat("本地Md5文件路径：{0}", strLocalMd5Path);
//            m_dicLocalMd5 = FileHelper.ReadAbMD5Info(strLocalMd5Path);
//            foreach (KeyValuePair<string, ABFileInfo> keyvalue in m_dicSvrMd5)
//            {
//                ABFileInfo newMd5Info = keyvalue.Value;
//                ABFileInfo localMd5Info = null;
//                m_dicLocalMd5.TryGetValue(newMd5Info.filename, out localMd5Info);
//                if (localMd5Info == null || localMd5Info.md5 != newMd5Info.md5)
//                {
//                    // 如果有差异，先删除本地文件
//                    if (localMd5Info != null)
//                    {
//                        string localFilePath = PathDef.UPDATE_DATA_PATH + "/" + localMd5Info.filename;
//                        FileHelper.DeleteFile(localFilePath);
//                    }
//                    //排除和母包中md5一样的热更列表
//                    bool isNeedDownLoad = true;
//                    // CheckABNeedUpdate的逻辑有需要自己补充
//                    // bool isNeedDownLoad = GameABFileChecker.Instance.CheckABNeedUpdate(newMd5Info);
//                    if (isNeedDownLoad)
//                    {
//                        string strCachePath = PathDef.UPDATE_CACHE_PATH + "/" + newMd5Info.filename;
//                        if (!FileHelper.IsFileExist(strCachePath))
//                        {
//                            m_totalSize += newMd5Info.compressSize;
//                            m_lstDiffMd5.Add(newMd5Info);
//                        }
//                    }
//                }
//            }


//            Debug.LogFormat("需要下载的更新大小：{0}, 差异文件数量：{1}", m_totalSize, m_lstDiffMd5.Count);
//            if (m_totalSize > 0)
//            {
//                //wifi或者数据下载量小于设定值，直接下载
//                if (Util.IsWifi() || m_totalSize < BaseDef.AUTO_DOWNLOAD_SIZE)
//                {
//                    RealDownloadPatch();
//                }
//                else
//                {
//                    string strTips = string.Format("是否确定使用手机流量下载[{0}]的游戏资源？", Util.FormatByte(m_totalSize));
//                    string strBtn = "继续";
//                    EventMgr.onLaunchConfirmTips(strTips, strBtn, RealDownloadPatch, false);
//                }
//            }
//            else
//            {
//                m_onDownPatchDone(true);
//            }
//        }

//        //用户点击了确定按钮
//        private void RealDownloadPatch()
//        {
//            long needSize = Mathf.FloorToInt((m_totalUnpackSize + m_totalSize) * 1.2f);
//            // 正确的磁盘容量自己写接口取获取
//            long diskSize = 1073741824;
//            Debug.LogFormat("下载补丁，磁盘容量：{0}, 需要容量：{1}", diskSize, needSize);
//            if (diskSize < needSize)
//            {
//                long diff = needSize - diskSize;
//                string strTips = string.Format("手机存储空间不足，还缺: {0},请释放手机内存后重试", Util.FormatByte(diff));
//                string strBtn = "更新";
//                EventMgr.onLaunchConfirmTips(strTips, strBtn, RealDownloadPatch, false);
//                return;
//            }
//            Debug.LogFormat("确定下载补丁包,数量：{0}", m_lstDiffMd5.Count);
//            ResetWorks();
//            if (m_lockProgress == null)
//            {
//                m_lockProgress = new Object();
//                m_lockMsg = new Object();
//            }

//            m_strErrMsg = "";
//            m_haveDownedSize = 0;

//            string strDownTips = "资源更新中";
//            strDownTips = strDownTips + "(" + Util.FormatByte(m_totalSize) + ")";
//            EventMgr.onProgressTips.Invoke(strDownTips, true);

//            string strTempPath = PathDef.UPDATE_CACHE_PATH;
//            FileHelper.CreateDirectory(PathDef.UPDATE_CACHE_PATH);
//            for (int i = 0; i < m_lstDiffMd5.Count; i++)
//            {
//                ABFileInfo md5Info = m_lstDiffMd5[i];
//                string strURL = m_strDownloadUrl + md5Info.filename;
//                string strPath = strTempPath + "/" + md5Info.filename;
//                // Debug.Log("添加任务，下载文件到：{0}", strPath);
//                AddWork(new Downloader(strURL, strPath, OnDownloadFileProgress, OnDownloadFileEnd));
//            }
//            StartWork();
//        }

//        // 下载
//        private void OnDownloadFileProgress(float progress, int downedSize, int totleSize, int diff)
//        {
//            lock (m_lockProgress)
//            {
//                m_haveDownedSize += diff;
//            }
//        }

//        // 下载单个文件结束
//        private void OnDownloadFileEnd(ErrorCode code, string msg, byte[] bytes)
//        {
//            if (code != ErrorCode.SUCCESS)
//            {
//                lock (m_lockMsg)
//                {
//                    m_strErrMsg += msg;
//                    m_strErrMsg += "\n";
//                    IsFail = true;
//                }
//            }
//        }

//        protected override void OnWorkProgress(float value)
//        {
//            // 大小没改变就不刷新进度
//            if (m_lastDownedSize == m_haveDownedSize)
//            {
//                return;
//            }

//            m_lastDownedSize = m_haveDownedSize;
//            float totalProgress = m_haveDownedSize * 1.0f / m_totalSize;

//            EventMgr.onProgressChange.Invoke(totalProgress);
//        }

//        protected override void OnWorkDone()
//        {
//            if (IsFail)
//            {
//                Debug.LogFormat("下载热更新资源失败，已下载:{0}，未下载:{1}, msg:{2}", Util.FormatByte(m_haveDownedSize), Util.FormatByte(m_totalSize), m_strErrMsg);
//                int lackSize = m_totalSize - m_haveDownedSize;
//                string strTips = string.Format("还有[{0}]资源未下载完成，请检查网络后继续下载", Util.FormatByte(lackSize));
//                string strBtn = "继续";
//                EventMgr.onLaunchConfirmTips(strTips, strBtn, () =>
//                {
//                    // 重新计算差异再下载
//                    CalDiffToDownload();
//                }, false);
//            }
//            else
//            {
//                RemoveUselessFile();
//                m_onDownPatchDone(true);
//            }
//            EventMgr.onProgressEnd.Invoke();
//        }

//        // 删除Update目录下无用的热更文件
//        private void RemoveUselessFile()
//        {
//            if (!FileHelper.IsDirectoryExist(PathDef.UPDATE_DATA_PATH))
//            {
//                return;
//            }
//            string[] arrOldFiles = FileHelper.GetAllChildFiles(PathDef.UPDATE_DATA_PATH, ".unity3d");
//            for (int i = 0; i < arrOldFiles.Length; i++)
//            {
//                string strFullFileName = arrOldFiles[i];
//                string strFileName = Path.GetFileName(strFullFileName);
//                if (!m_dicSvrMd5.ContainsKey(strFileName))
//                {
//                    Debug.LogFormat("***RemoveUselessFile: {0}", strFullFileName);
//                    FileHelper.DeleteFile(strFullFileName);
//                }
//            }
//        }


//        // 将版本号写入缓存
//        public void WriteVersion()
//        {
//            string strNewFileINfo = FileHelper.GenABFileDicMd5String(m_dicSvrMd5);
//            string strPath = PathDef.UPDATE_DATA_PATH + "/" + BaseDef.MD5_FILENAME;
//            Config.Version = m_strNewVersion;
//            FileHelper.SaveTextToFile(strNewFileINfo, strPath);
//            Debug.LogFormat("WriteVersion:{0}, strNewFileINfo:{1}", m_strNewVersion, strNewFileINfo);
//            PlayerPrefs.Save();
//            m_dicSvrMd5.Clear();
//            m_dicLocalMd5.Clear();
//            m_lstDiffMd5.Clear();
//        }
//    }
//}
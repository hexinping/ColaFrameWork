# -*- coding: UTF-8 -*-
__author__ = 'minhuaxu'

import sys
import os
sys.path.append(os.path.abspath(os.path.join(os.getcwd(), "..")))
from wpyscripts.tools.baisc_operater import *
from wpyscripts.common.adb_process import excute_adb
import CheckDevices

local_package = "com.tencent.tfwl.yw"
player_name = "ares"
ip = "192.168.1.204"

def start_game():
    #检测adb设备是否连接
    logger.debug("connect device name:{0},android version:{1}".format(CheckDevices.getDeviceName(),CheckDevices.getAndroidVersion()))

    # 步骤 1 ，本地环境准备，清楚数据，拉起游戏
    device._clear_user_info(local_package)
    device.launch_app(local_package)
    time.sleep(10)

    #步骤 2 ，点击任意位置跳过开场动画
    excute_adb("shell input tap 10 10")

    version = engine.get_sdk_version()
    logger.debug("Version Information:{0}".format(version))

    # 步骤 3 ， 设置ip和登录名
    name = find_element_wait("/UGUIRoot/Canvas/uilogin_new_v1/canvas_topui/login/input_account")
    engine.input(name, player_name)

    result = engine.call_registered_handler("AutoSetIp",ip)
    logger.debug(result)

    enterBtn = engine.find_element("/UGUIRoot/Canvas/uilogin_new_v1/canvas_topui/login/button_login")
    engine.click(enterBtn)

    startBtn = find_element_wait("/UGUIRoot/Canvas/uilogin_new_v1/canvas_topui/entergame/btn_begin",max_count=3,sleeptime =1)
    if startBtn is None:
        logger.debug(u"登录失败")
        return
    else:
        screen_shot_click(startBtn,sleeptime=0, exception=True)

    enterGame = find_element_wait("/UGUIRoot/Canvas1/uiselectrole_v1/right_panel/btn_entry",max_count=5,sleeptime =1)
    if enterGame is None:
        logger.debug(u"登录失败")
        return
    else:
        engine.click(enterGame)

    mainpage = find_element_wait("/UGUIRoot/Canvas/uimainpage_subsys_v1")
    if mainpage is None:
        logger.debug(u"登录失败")
    else:
        logger.debug(u"登录成功")

start_game()

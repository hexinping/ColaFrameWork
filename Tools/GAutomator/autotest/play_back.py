# -*- coding: UTF-8 -*-
__author__ = 'minhuaxu'

import traceback

import sys,os
sys.path.append(os.path.abspath(os.path.join(os.getcwd(), "..")))
from wpyscripts.tools.baisc_operater import *
from wetest_exceptions import *

from decision_tree import *
record_file_dir = "records/test.txt"


class PlayBacker(object):
    def __init__(self, gamepackage, descision_tree):
        self._MAX_EXCEPTION = 10  # 最大出现异常次数
        self._excepiton_time = 0
        self._index = 0  # 上一次点击的录制内容的序列号
        self.game_package = gamepackage
        self._none_element_times = 0  # 连续出现没有可点击节点的次数
        self.descision_tree = descision_tree
        self._unknow_activity = 1

        self._handle_package_dict = {"com.tencent.mobileqq": "_qq_wechat_login",
                                     "com.tencent.wechat": "_qq_wechat_login"}

    def _qq_wechat_login(self):
        """
            QQ或者微信登陆，如果抛异常则直接结束
        :return:
        """
        try:
            device.login_qq_wechat_wait(120)
            time.sleep(5)
        except:
            stack = traceback.format_exc()
            logger.error(stack)
            raise WeTestPlayBackException("Login Error")

    def _default_unknow_package_handle(self, package=None, trytime=10):
        """
            未知的，包处理方式
        :return:
        """
        try:
            report.screenshot()
            if self._unknow_activity % trytime == 0:
                logger.debug("Unknow pakcage : {0},device.back()".format(package))
                device.back()
                self._unknow_activity = 1
            else:
                engine.click_position(random.randrange(0, 700),
                                      random.randrange(0, 700))  # 随机在屏幕上点击。有些游戏屏幕任意位置点击，返回可点击节点是空的
                self._unknow_activity += 1
            time.sleep(3)
        except:
            stack = traceback.format_exc()
            logger.error(stack)
            raise WeTestPlayBackException("Login Error")

    def _handle_package(self, detect_content):
        """
            处理包名
            如果是，com.tencent.mobileqq或者com.tencent.wechat，执行登陆
            如果是，不认识的包名，则执行back
            如果是，游戏包名，返回节点为空，且多次连续多次出现这样的情况，则执行back
        :return:
        """
        if len(detect_content) == 1 and detect_content[0] == self.game_package:
            # 还是游戏的package，但是返回可点击的节点为空，可能在web界面也可能是在加载进度条的页面。执行返回。也有可能是在加载界面
            self._default_unknow_package_handle(detect_content[0], trytime=30)
            return False
        self._none_element_times = 0
        if len(detect_content) > 2 and detect_content[0] == self.game_package:
            # 游戏界面
            return True

        package = detect_content[0]
        handle_fun = self._handle_package_dict.get(package, None)
        if not handle_fun:
            "未知的package"
            logger.warn("Unknow Package : {0}".format(package))
            self._default_unknow_package_handle(package, trytime=3)
        else:
            func = getattr(self, handle_fun)
            func()

        return False

    def _detect(self):
        """
            探知游戏当前可点击节点情况
        :return:
        """
        try:
            scene, elements = engine.get_touchable_elements_bound()
            detect_content = None
            if elements:
                detect_content = [self.game_package, scene, elements]
            else:
                try:
                    package = device.get_top_package_activity()
                    detect_content = [package.package_name]
                except:
                    stack = traceback.format_exc()
                    self._excepiton_time += 1
                    logger.debug(stack)

            return detect_content
        except:
            stack = traceback.format_exc()
            self._excepiton_time += 1
            logger.warn(stack)

    def _over(self):
        """
            判断回放是否需要结束，如果回放到最后一个节点，且回放率超过80%
        :return:
        """
        test_element_num = 0
        for i in self.descision_tree.sequence_used:
            if i > 0:
                test_element_num += 1
        tested_elements = test_element_num * 1.0 / len(self.descision_tree.sequenc_index)
        if self.descision_tree.last_index >= len(self.descision_tree.sequenc_index) - 1:
            if tested_elements > 0.8:
                return True
            else:
                self.descision_tree.last_index = -1
                return False

    def run(self):
        """
            获取当前的游戏状况
        :return:
        """
        max_count = len(self.descision_tree.sequence_used) * 2
        for i in range(max_count):
            if self._excepiton_time > self._MAX_EXCEPTION:
                raise WeTestPlayBackException("Too much exceptions")
            if self._over(): return True
            detect_contents = self._detect()
            if not detect_contents:
                time.sleep(3)
                continue
            re = self._handle_package(detect_contents)
            if re:
                # 确定最佳的选择
                actions = self.descision_tree.classify(detect_contents)
                logger.debug("Do action sequence : {0}".format(self.descision_tree.continue_seq))
                for action in actions:
                    try:
                        action.do_action()
                    except:
                        stack = traceback.format_exc()
                        logger.warn(stack)
                        self._excepiton_time += 1

    def statistics(self):
        testnum = 0
        for i in self.descision_tree.sequence_used:
            if i > 0:
                testnum += 1
        logger.debug(u"测试覆盖占比:{0}".format(testnum * 1.0 / len(self.descision_tree.sequence_used)))


def main():
    file_path = os.path.split(os.path.realpath(__file__))[0]
    file_path=os.path.join(file_path,unicode(record_file_dir, 'utf-8'))
    r = RecorderParser(file_path)
    r.parse()
    tree = DescionTree()
    tree.create_tree(r.inputs)
    tree.set_display(device.get_display_size())
    game_dict = tree.tree
    for key, value in game_dict.items():
        logger.debug(key + "       =================================================================")
        for v in value:
            logger.debug("\t\t {0}".format(v))

    logger.debug(tree.sequenc_index)

    game_package = os.environ.get("PKGNAME")
    if not game_package:
        game_package = "com.tencent.wetest.demo"
    play_back = PlayBacker(game_package, tree)
    try:
        play_back.run()
    except:
        stack = traceback.format_exc()
        logger.error(stack)
    finally:
        play_back.statistics()


main()

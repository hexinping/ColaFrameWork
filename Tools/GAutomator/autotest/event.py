# -*- coding: UTF-8 -*-
__author__ = 'minhuaxu'

import re, traceback
import math
from wpyscripts.tools.baisc_operater import *

try:
    import cPickle as pickle
except ImportError:
    import pickle

from wetest_exceptions import *


class TouchPhase(object):
    DOWN = 1
    UP = 2
    MOVE = 3

    _convert = {1: 1,
                2: 2,
                3: 3,
                "1": 1,
                "2": 2,
                "3": 3,
                "0": 0}

    @classmethod
    def get_phase(cls, num):
        return cls._convert.get(num, 0)


class TouchEvent(object):
    """
        touch事件
    """

    def __init__(self):
        self.deltatime = 0  # SDK启动至今的毫秒数
        self.x = 0.0  # 屏幕坐标
        self.y = 0.0  # 屏幕坐标
        self.relativeX = 0.0  # 归一化屏幕相对zuobiao x/width
        self.relativeY = 0.0  # 归一化屏幕相对zuobiao y/height
        self.fingerId = 0  # 唯一标示手指
        self.phase = 0  # 1标示down,2标示up,3标示move

    def parse(self, str):
        """
            {phase:1,fingerid:0,x:794.269,y:722.0001,relativeX:0.4427364,relativeY:0.6685185,deltatime:31531}
            r"(txt\s*=\s*(?P<txt>[^,\}]*))"
        :param str:
        :return:
        """
        touch_regex = re.compile(
            r"phase:\s*(?P<phase>[1-3]),fingerid:\s*(?P<fingerid>[\d]),x:\s*(?P<x>[\d|\.]*),y:\s*(?P<y>[\d|\.]*),"
            r"relativeX:\s*(?P<relativeX>[\d|\.]*),relativeY:\s*(?P<relativeY>[\d|\.]*),deltatime:\s*(?P<deltatime>[\d]*)")
        search_result = touch_regex.search(str)
        if not search_result:
            raise WeTestInputFormatInvaild(str)

        dict = search_result.groupdict()
        self.deltatime = int(dict.get("deltatime", "0"))
        self.x = float(dict.get("x", "0.0"))
        self.y = float(dict.get("y", "0.0"))
        self.relativeX = float(dict.get("relativeX", "0.0"))
        self.relativeY = float(dict.get("relativeY", "0.0"))
        self.fingerId = int(dict.get("fingerid", "0"))
        self.phase = TouchPhase.get_phase(dict.get("phase", "0"))

    def __str__(self):
        str = "phase:{0},fingerid:{1},x:{2},y:{3},relativeX:{4},relativeY:{5},deltatime:{6}".format(self.phase,
                                                                                                    self.fingerId,
                                                                                                    self.x, self.y,
                                                                                                    self.relativeX,
                                                                                                    self.relativeY,
                                                                                                    self.deltatime)
        return str


class Event(object):
    def __init__(self):
        self.interval = 0

    def do_action(self):
        """
            直接操作手机相关动作
        :return:
        """
        pass


class ClickEvent(Event):
    def __init__(self, touch=None):
        super(ClickEvent, self).__init__()
        if touch:
            self.x = touch.x
            self.y = touch.y
            self.relativeX = touch.relativeX
            self.relativeY = touch.relativeY
            self.element_name = "None"
        else:
            self.x=0
            self.y=0
            self.relativeY=0
            self.relativeX=0
            self.element_name = "None"

    def __str__(self):
        str = "Click Event x = {0},y = {1},relativeX = {2},relativeY = {3},interval = {4}".format(self.x, self.y,
                                                                                                  self.relativeX,
                                                                                                  self.relativeY,
                                                                                                  self.interval)
        return str


    def __repr__(self):
        return self.__str__()

    def do_action(self):
        sleeptime=round(self.interval / 1000 + 0.5)
        screen_shot_click_pos(self.x, self.y, sleeptime=sleeptime)


class PressEvent(Event):
    def __init__(self, touch=None, duration=0):
        super(PressEvent, self).__init__()
        if touch:
            self.x = touch.x
            self.y = touch.y
            self.relaiveX = touch.relativeX
            self.relativeY = touch.relativeY
        else:
            self.x=0
            self.y=0
            self.relaiveX=0
            self.relativeY=0
        self.duration = duration
        self.element_name = "None"

    def __str__(self):
        str = "Press Event x = {0},y = {1},relativeX = {2},relativeY = {3},duration = {4},interval = {5}".format(self.x,
                                                                                                                 self.y,
                                                                                                                 self.relaiveX,
                                                                                                                 self.relativeY,
                                                                                                                 self.duration,
                                                                                                                 self.interval)
        return str

    def __repr__(self):
        return self.__str__()

    def do_action(self):
        sleeptime=round(self.interval / 1000 + 0.5)
        logger.debug("Press ({0},{1}),element {2},durantion time = {3},sleeptime={4}".format(self.x,self.y,self.element_name,self.duration,sleeptime))
        report.capture_and_mark(self.x, self.y, self.element_name)
        engine.press_position(self.x, self.y, self.duration)
        time.sleep(sleeptime)


class Swipe(Event):
    """
        滑动时间特别长的话，可以暂时推断为，joystick
    """

    def __init__(self, touch_down=None, touch_up=None):
        super(Swipe, self).__init__()
        if touch_down and touch_up:
            self.start_x = touch_down.x
            self.start_y = touch_down.y
            self.start_relaiveX = touch_down.relativeX
            self.start_relativeY = touch_down.relativeY
            self.end_x = touch_up.x
            self.end_y = touch_up.y
            self.end_relaiveX = touch_up.relativeX
            self.end_relativeY = touch_up.relativeY
            self.duration = touch_up.deltatime - touch_down.deltatime
        else:
            self.start_x = 0
            self.start_y = 0
            self.start_relaiveX = 0
            self.start_relativeY = 0
            self.end_x = 0
            self.end_y = 0
            self.end_relaiveX = 0
            self.end_relativeY = 0
            self.duration = 0

    def __str__(self):
        str = "Swipe Event ({0},{1}) => ({2},{3}),({4},{5}) => ({6},{7}),duration = {8},interval = {9}".format(
            self.start_x, self.start_y, self.end_x, self.end_y, self.start_relaiveX, self.start_relativeY,
            self.end_relaiveX, self.end_relativeY, self.duration, self.interval)
        return str

    def __repr__(self):
        return self.__str__()

    def do_action(self):
        """
            swipe如果持续时长超过,2秒（经验值），直接转化为joystick，swipe_and_press
        :return:
        """
        report.screenshot()
        sleeptime=round(self.interval / 1000 + 0.5)
        if self.duration > 2000:
            logger.debug("Joystick ({0},{1})==>({2},{3}),duration={4},sleeptime={5}".format(self.start_x, self.start_y, self.end_x, self.end_y, self.duration - 2000,sleeptime))
            engine.swipe_and_press(self.start_x, self.start_y, self.end_x, self.end_y, 200, self.duration - 2000)
        else:
            logger.debug("Swipe ({0},{1})==>({2},{3}),duration={4},sleeptime={5}".format(self.start_x, self.start_y, self.end_x, self.end_y, self.duration ,sleeptime))
            engine.swipe_position(self.start_x, self.start_y, self.end_x, self.end_y, self.duration / 10)
        time.sleep(sleeptime)


class Converter(object):
    """
        将记录的相对坐标，根据当前手机的长宽,进行转换
    """

    def __init__(self):
        self.display_time=1
        self.display=None


    def get_display(self):
        if self.display == None:
            for i in range(10):
                try:
                    display=device.get_display_size()
                    if display:
                        self.display=display
                        break
                except:
                    stack=traceback.format_exc()
                    logger.warn(stack)

        if self.display_time % 30 == 0:
            try:
                display = device.get_display_size()
                if display:
                    self.display = display
            except:
                stack = traceback.format_exc()
                logger.warn(stack)
        return self.display

    def convert_touchable_event(self, element, recorder_item):
        """
            从get_touchable_element_bound获取的内容转换为,ClickEvent
        :param element:
        :param 记录的内容。如果recorder_item为空，则说明没有记录
        :return:
        """

        if recorder_item and len(recorder_item) >= 3:
            event = recorder_item[2][0]
            if isinstance(event, ClickEvent):
                c = ClickEvent()
                bound = element[1]
                e = element[0]
                c.x = bound.x + bound.width / 2
                c.y = bound.y + bound.height / 2
                c.element_name = e.object_name
                c.interval = event.interval
                return c
            elif isinstance(event, PressEvent):
                c = PressEvent()
                bound = element[1]
                e = element[0]
                c.x = bound.x + bound.width / 2
                c.y = bound.y + bound.height / 2
                c.element_name = e.object_name
                c.duration = event.duration
                c.interval = event.interval
                return c
            elif isinstance(event, Swipe):
                c = Swipe()
                c.start_x = event.start_relaiveX * self.get_display().width
                c.start_y = event.start_relativeY * self.get_display().height
                c.end_x = event.end_relaiveX * self.get_display().width
                c.end_y = event.end_relativeY * self.get_display().height
                c.duration = event.duration
                c.interval = event.interval
                return c
        else:
            c = ClickEvent()
            bound = element[1]
            e = element[0]
            c.x = bound.x + bound.width / 2
            c.y = bound.y + bound.height / 2
            c.element_name = e.object_name
            c.interval=2000
            return c

    def convert_event_event(self, event):
        """
            把原来的event转换下，主要根据屏幕相当坐标，转化为当前手机的绝对坐标
        :param event:
        :return:
        """
        if isinstance(event, ClickEvent):
            c = ClickEvent()
            c.x = event.relativeX * self.get_display().width
            c.y = event.relativeY * self.get_display().height
            c.interval = event.interval
            return c
        elif isinstance(event, PressEvent):
            c = PressEvent()
            c.x = event.relaiveX * self.get_display().width
            c.y = event.relativeY * self.get_display().height
            c.interval = event.interval
            c.duration=event.duration
            return c
        elif isinstance(event, Swipe):
            c = Swipe()
            c.start_x = event.start_relaiveX * self.get_display().width
            c.start_y = event.start_relativeY * self.get_display().height
            c.end_x = event.end_relaiveX * self.get_display().width
            c.end_y = event.end_relativeY * self.get_display().height
            c.duration = event.duration
            c.interval = event.interval
            return c


class RecorderParser(object):
    def __init__(self, path):
        self.path = path
        self.line_regex = re.compile(r"(?P<scene>.*?),(?P<element>.*?),(?P<touches>.*)")
        self.touches_find_regex = re.compile(r"{.*?}")
        self.inputs = []
        self._events_sequence = {}  # 手指输入序列 {fingerId,[[touchevent,item]]}
        self._last_event = None
        self._last_time = 0

    def parse(self):
        with open(self.path, "r") as file:
            contents = file.readlines()
            print(contents)
            for line in contents:
                item, touches = self._parse_line(line)
                invaild_event = self._check_invaild_event(item, touches)
                if invaild_event:
                    self.inputs.append(item)
                else:
                    item = None
                self._handle_touch_events(touches, item)

        file.close()

    def _check_invaild_event(self, item, touches):
        if not item or not touches:
            return False
        if item[1] == None or item[1] == '':
            for touch in touches:
                if touch.phase == TouchPhase.DOWN:
                    return True
            return False
        else:
            return True

    def _compute_event_type(self, touch_down, touch_up):
        """
            根据这两个的时间差
        :param touch_down:
        :param touch_up:
        :return:
        """
        # 计算出上一次的间隔时间
        interval = 0
        if self._last_event:
            interval = touch_down.deltatime - self._last_time
            self._last_event.interval = interval
        self._last_time = touch_up.deltatime

        # 计算出事件类型，如果是distance，down和up的距离相差40（NGUI的阀值设定）以上就认为是滑动
        distance = (touch_down.x - touch_up.x) ** 2 + (touch_down.y - touch_up.y) ** 2
        interval = touch_up.deltatime - touch_down.deltatime

        if distance > 1600:
            self._last_event = Swipe(touch_down, touch_up)
        elif interval > 1600:
            self._last_event = PressEvent(touch_down, interval)
        else:
            self._last_event = ClickEvent(touch_down)

        return self._last_event

    def _handle_touch_events(self, touches, item):
        """
            touches，本地输入的事件
            item，本次输入的内容位置信息
            将down,move,up推算出click,press,swipe,or joystick
        :return:
        """
        for touch in touches:
            fingerid = touch.fingerId
            if fingerid != 0:
                print(touch)
            sequence = []
            if self._events_sequence.has_key(fingerid):
                sequence = self._events_sequence.get(fingerid, [])
            else:
                self._events_sequence[fingerid] = sequence
            if touch.phase == TouchPhase.UP and len(sequence) != 0:
                # 现在没有抬起事件，只有UP事件。所以当出现UP事件的时候，同个一个finger下面照理来说，应该是具有Down事件的
                old_t = sequence[-1][0]
                if old_t.phase == TouchPhase.DOWN:
                    event = self._compute_event_type(old_t, touch)
                    old_item = sequence[-1][1][-1]  # sequnce结构[touch,[scene,element,[]]]
                    old_item.append(event)
            sequence.append([touch, item])

    def _parse_line(self, line=""):
        line = line.strip()
        search_result = self.line_regex.search(line)
        dict = search_result.groupdict()
        item = []
        scene = dict.get("scene", None)
        item.append(scene)
        element = dict.get("element", None)
        item.append(element)
        item.append([])
        if element == "": element = None
        touches_str = dict.get("touches", "")
        touches_array = self.touches_find_regex.findall(touches_str)
        touches = []
        for touch_line in touches_array:
            try:
                t = TouchEvent()
                t.parse(touch_line)
                touches.append(t)
            except Exception as e:
                traceback.print_exc()
        return item, touches


class DataSaver(object):
    def __init__(self):
        pass

    def save(self, path, data):
        with open(path, "wb") as f:
            pickle.dump(data, f)

    def load(self, path):
        with open(path, "rb") as f:
            data = pickle.load(f)
            return data


if __name__ == '__main__':
    r = RecorderParser("E:\\sgame_recorder.txt")
    r.parse()
    for item in r.inputs:
        print("**************************************")
        for it in item[:-1]:
            print(it)
        for it in item[-1]:
            print(it)

    saver = DataSaver()
    saver.save("E:\\sgame_recorder.dat", r.inputs)
    inputs = saver.load("E:\\sgame_recorder.dat")
    for item in inputs:
        print("**************************************")
        for it in item[:-1]:
            print(it)
        for it in item[-1]:
            print(it)

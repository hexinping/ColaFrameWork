# -*- coding: UTF-8 -*-
__author__ = 'minhuaxu'

import random
import math
from event import *
import logging

logger = logging.getLogger(__name__)
"""
    决策树
"""


def _candidate_actions_cmp(a, b):
    """
        使用次数升序排列，序列号升序排列
    :param a:
    :param b:
    :return:
    """
    result = cmp(a[0], b[0])
    if result == 0:
        return cmp(a[1], b[1])
    else:
        return result


class DescionTree(object):
    def __init__(self):
        self.tree = {}
        self.labels = ["Scene", "Index", "Elements"]
        self.sequenc_index = []  # 输入序列号与sequence决策树里面的位置
        self.sequence_used = []
        self._converter = Converter()

        self.last_index = -1
        self._last_select = None

        self.continue_seq = []

        self.width = 0
        self.height = 0

    def create_tree(self, input_items):
        game_label = self.tree
        for index, item in enumerate(input_items):
            scene = item[0]
            sequence = []
            if game_label.has_key(scene):
                sequence = game_label.get(scene, [])
            else:
                game_label[scene] = sequence
            value = [index]
            value.extend(item[1:])
            sequence.append(value)
            self.sequenc_index.append(len(sequence) - 1)
            self.sequence_used.append(0)

    def set_display(self, display):
        self.width = display.width
        self.height = display.height


    def _unknow_best_select(self, touchable_elements, scene=None):
        """
            没有找到依据的情况下，从众多节点中选出最优的一个点
        :return:
        """
        logger.debug("##Learn## scene:{0}".format(scene))
        logger.debug("##Learn## last_index={0}".format(self.last_index))
        logger.debug("##Learn## last_select={0}".format(self._last_select))
        for e, b in touchable_elements:
            logger.debug("##Learn## element {0}".format(e.object_name))
        element = random.choice(touchable_elements)
        logger.debug("##Learn## random_select={0}".format(element))
        element = self._converter.convert_touchable_event((e, b), None)
        return [element]

    def _find_suitable_sequence(self, sequence, elements):
        """
            序列不匹配，尝试从记录中找到符合的一段。上一步执行到步骤10，步骤11操作的节点并不在当前返回的可点击节点里面。那么尝试去查找，是否存在匹配的录制的记录。
        :param sequence:
        :param elements:
        :return:
        """
        p=-1
        best_action=[]
        candidate_actions = {}  # 以回放次数和序列号作为,key，touch_element作为value
        for i, e in enumerate(sequence):
            for touch_element, bound in elements:
                if e[1] == touch_element.object_name:
                    key = (self.sequence_used[e[0]], e[0])
                    if candidate_actions.has_key(key):
                        candidate_actions[key].append((touch_element,bound,e))
                    else:
                        candidate_actions[key] = [(touch_element,bound,e)]
        keys = candidate_actions.keys()
        keys.sort(cmp=_candidate_actions_cmp) #优先处理点击次数少的，其次序列靠前的
        if keys:
            actions=candidate_actions[keys[0]]
            elements=[(touch_element,bound) for touch_element,bound,e in actions]
            touch_element,bound,e=actions[0]
            touch_element,bound=self._find_best_element(e,elements)
            best_action.append(self._converter.convert_touchable_event((touch_element,bound), e))
            logger.debug("Continue sequence:{0}".format(self.continue_seq))
            self.continue_seq = [e[0]]
            self.last_index = e[0]
            p = self.last_index
            self.sequence_used[self.last_index] += 1
        if best_action:
            self._increase_sequence(best_action, sequence, p)
        return best_action

    def _find_best_sequence(self, sequence, elements):
        """
            序列号匹配，节点匹配的录制记录。如上一次执行到步骤10，步骤11中包含，本地返回的可点击节点。那么步骤11本认为是最适合的节点
        :param index:
        :param elements:
        :return:
        """
        try:
            position = self.sequenc_index[self.last_index + 1]
        except:
            logger.error("sequence_len : {0},last_index : {1}".format(len(self.sequenc_index), self.last_index))
            raise WeTestPlayBackException("End")
        # 记录中应该出现的（position下一个位置查找），当前可点击节点出现了
        recorder_element = sequence[position]
        best_action = []
        p = -1
        # 返回节点中
        candidates=[]
        for e, b in elements:
            # 序列号匹配，节点也匹配。
            # TODO 如果存在相同节点名称的，应该选择位置最接近的
            if e.object_name == recorder_element[1]:
                #和最接近的距离进行比较
                candidates.append((e,b))
        element = self._find_best_element(recorder_element, candidates)
        if element:
            best_action.append(self._converter.convert_touchable_event(element, recorder_element))
            self.continue_seq.append(self.last_index + 1)
            self.last_index = self.last_index + 1
            self.sequence_used[self.last_index] += 1
            p = self.last_index
            self._increase_sequence(best_action, sequence, p)
        return best_action

    def _find_best_element(self, recorder_element, elements):
        element = None
        distance = 10000
        if len(elements) < 1:
            return element
        elif len(elements) == 1:
            return elements[0]
        for e, b in elements:
            event = recorder_element[2][0]
            relativex = event.relativeX * self.width
            relativey = event.relativeY * self.height
            if relativex >= b.x and relativex <= b.x+b.width :
                distance_x = 0
            else:
                distance_x = abs(relativex-b.x) if abs(relativex-b.x) <= abs(relativex-b.x-b.width) else abs(relativex-b.x-b.width)
            if relativey >= b.y and relativey <= b.y + b.height:
                distance_y = 0
            else:
                distance_y = abs(relativey-b.y) if abs(relativey-b.y) <= abs(relativey-b.y-b.height) else abs(relativey-b.y-b.height)
            tmp = math.sqrt(pow(distance_x, 2)+pow(distance_y, 2))
            if tmp < distance:
                distance = tmp
                element = (e, b)
                if tmp == 0:
                    break
        return element

    def _increase_sequence(self, best_action, sequence, index):
        """
            确定执行的某个记录点，如何他的后续操作中不包含element的。这一次性返回即可，避免多次调用get_touchable_elements_bound()
        :param actions:
        :return:
        """
        if best_action and index >= 0:
            p = self.sequenc_index[index] + 1
            for recorder_element in sequence[p:]:
                if recorder_element[0] == self.last_index + 1 and (
                                recorder_element[1] == "" or not isinstance(recorder_element[2][0], ClickEvent)):
                    best_action.append(self._converter.convert_event_event(recorder_element[2][0]))
                    self.continue_seq.append(recorder_element[0])
                    self.last_index = recorder_element[0]
                    self.sequence_used[self.last_index] += 1
                else:
                    break
        return best_action

    def classify(self, test_contents):
        """
            从输入的内容["Scene","Index",[Elements]]
            返回的节点，可能是Element
            也有可能是，执行序列。如果，决策树选中的节点下连续的序列中，有包含无element的一起返回。这么做的主要原因，在于减少get_touchable_element_bound的次数
        :param test_contents:
        :return:
        """
        scene = test_contents[1]
        sequence = self.tree.get(scene, None)
        select_result = []
        if not sequence:
            # scene都不匹配，随机选择
            element = self._unknow_best_select(test_contents[2], scene)
            select_result.extend(element)
            return select_result

        # 从当前序列面选择最佳的
        best_action = self._find_best_sequence(sequence, test_contents[2])
        if best_action:
            # 最佳选择一定是录制记录里面的，需要更新记录
            self._last_select = best_action
            select_result.extend(best_action)
        else:
            best_action = self._find_suitable_sequence(sequence, test_contents[2])
            if best_action:
                # 找不到最佳的，找最适合的
                self._last_select = best_action
                select_result.extend(best_action)
            else:
                # scene匹配，但是序列号和element都不匹配的
                element = self._unknow_best_select(test_contents[2], scene)
                select_result.extend(element)

        return select_result


def convertJson(obj):
    return obj.__dict__


def test():
    file_path = os.path.split(os.path.realpath(__file__))[0]
    file_path = os.path.join(file_path, "../")
    r = RecorderParser("../recorder.txt")
    r.parse()
    tree = DescionTree()
    tree.create_tree(r.inputs)
    game_dict = tree.tree
    for key,value in game_dict.items():
        print(key+"       =================================================================")
        for v in value:
            print "\t\t {0}".format(v)

    print tree.sequenc_index
    import json
    js = json.dumps(tree.tree, default=convertJson)
    print(js)

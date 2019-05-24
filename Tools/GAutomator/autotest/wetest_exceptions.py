#-*- coding: UTF-8 -*-
__author__ = 'minhuaxu'

from wpyscripts.common.wetest_exceptions import *

class WeTestInputFormatInvaild(WeTestRuntimeError):
    """
        错误的文本输入
    """
    pass


class WeTestPlayBackException(WeTestRuntimeError):
    """
        回放过程中的错误
    """
    pass
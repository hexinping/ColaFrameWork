# -*- coding: UTF-8 -*-
__author__ = 'minhuaxu'


import sys,os
import json
sys.path.append(os.path.abspath(os.path.join(os.getcwd(), "..")))
import wpyscripts.manager as manager

record_file_dir = "records/test.txt"


class Record(object):
    ENTER_RECORD = 500
    LEAVE_RECORD = 501

    def __init__(self,path):
        self.engine = manager.get_engine()
        self.socket = self.engine.socket
        self.record_file=open(path,"w")

    def send_record(self):
        self.socket.send_command(Record.ENTER_RECORD)

    def format_touches(self,touches):
        t_str=[]
        for touch in touches:
            str="{{phase:{phase},fingerid:{fingerId},x:{x},y:{y},relativeX:{relativeX},relativeY:{relativeY},deltatime:{deltatime}}}".format(**touch)
            t_str.append(str)

        res=",".join(t_str)

        return res

    def format_record(self, record):
        touches_str=self.format_touches(record["touches"])
        record_str="{0},{1},[{2}]\n".format(record["scene"],record["name"],touches_str)
        return record_str

    def recv_record(self):
        self.socket.socket.settimeout(None)
        while True:
            record = self.socket._recv_data()
            print record
            record_str=self.format_record(record)
            print record_str
            self.record_file.write(record_str)
            self.record_file.flush()


if __name__ == '__main__':
    record = Record(unicode(record_file_dir, 'utf-8'))
    record.send_record()
    record.recv_record()

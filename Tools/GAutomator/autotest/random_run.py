import os, sys
sys.path.append(os.path.abspath(os.path.join(os.getcwd(), "..")))
from wpyscripts.tools.baisc_operater import *
import wpyscripts.tools.traverse.travel as travel

logger = manager.get_logger()

def random_search():
    log_dir = os.environ.get("UPLOADDIR")
    if log_dir:
        log_dir = os.path.join(log_dir, "policy.log")
    else:
        log_dir = "../autotest/random_run_{0}.log".format(os.environ.get("ADB_SERIAL", ""))
    logger.info("run random search in testcase runner")
    travel.explore(log_dir, [], mode=0, max_num=3000)


random_search()
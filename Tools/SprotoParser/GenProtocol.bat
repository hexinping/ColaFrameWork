@chcp 65001
set base_dir=%~dp0
%base_dir:~0,2%
cd %base_dir%

set sproto_src_dir = .\Protocols\
set sproto_dst_dir = .\Outputs\
@echo 开始执行sproto协议导出...

rem 工作都转移到Lua中实现吧

@pause


--[[Notice:This lua config file is auto generate by Xls2Lua Tools，don't modify it manually! --]]
local fieldIdx = {}
fieldIdx.id = 1
fieldIdx.path = 2
local data = {
{100,[[Arts/UI/Prefabs/uiLoginPanel.prefab]]},
{101,[[Arts/UI/Prefabs/uiloading.prefab]]},
{102,[[Arts/UI/Prefabs/UIDebugPanel.prefab]]},
{103,[[Arts/UI/Prefabs/UISettingPanel.prefab]]},
{105,[[Arts/UI/Prefabs/UIWorldDialog.prefab]]},
{104,[[Arts/UI/Prefabs/UIModelPanel.prefab]]},
{2001,[[Arts/UI/Textures/atlas/airfightSheet.prefab]]},
{300001,[[Arts/UI/Material/material_defaultgray.mat]]},
{400001,[[Arts/UI/Textures/head_icon/GuChenSha.png]]},
{400002,[[Arts/UI/Textures/texture/loading/LoadingBackground0.png]]},}
local mt = {}
mt.__index = function(a,b)
	if fieldIdx[b] then
		return a[fieldIdx[b]]
	end
	return nil
end
mt.__newindex = function(t,k,v)
	error('do not edit config')
end
mt.__metatable = false
for _,v in ipairs(data) do
	setmetatable(v,mt)
end
return data
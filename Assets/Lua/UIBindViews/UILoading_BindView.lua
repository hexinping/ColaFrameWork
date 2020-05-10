--[[Notice:This lua uiview file is auto generate by UIViewExporter，don't modify it manually! --]]

local public = {}

public.viewPath = "Arts/UI/Prefabs/UILoading.prefab"

function public.BindView(uiView, Panel)
	if nil ~= Panel then
		local collection = Panel:GetComponent("UIComponentCollection")
		if nil ~= collection then
			--pass
		else
			error("BindView Error! UIComponentCollection is nil!")
		end
	else
		error("BindView Error! Panel is nil!")
	end
end

return public
--[[Notice:This lua uiview file is auto generate by UIViewExporter，don't modify it manually! --]]

local public = {}

function public.BindView(uiView, Panel)
	if nil ~= Panel then
		local collection = Panel:GetComponent("UIComponentCollection")
		if nil ~= collection then
			uiView.m_Btn_AnimBack = collection:Get(0)
			uiView.m_Btn_One = collection:Get(1)
			uiView.m_Btn_Two = collection:Get(2)
			uiView.m_Btn_Three = collection:Get(3)
			uiView.m_Btn_Close = collection:Get(4)
			uiView.m_Btn_Switch = collection:Get(5)
			uiView.m_Btn_AnimBefore = collection:Get(6)
		else
			error("BindView Error! UIComponentCollection is nil!")
		end
	else
		error("BindView Error! Panel is nil!")
	end
end

return public
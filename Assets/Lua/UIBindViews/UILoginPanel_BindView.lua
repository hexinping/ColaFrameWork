--[[Notice:This lua uiview file is auto generate by UIViewExporter，don't modify it manually! --]]

local public = {}

function public.BindView(uiView, Panel)
	if nil ~= Panel then
		local collection = Panel:GetComponent("UIComponentCollection")
		if nil ~= collection then
			uiView.m_usernameInput = collection:Get(0)
			uiView.m_passwordInput = collection:Get(1)
			uiView.m_bg = collection:Get(2)
			uiView.m_cancelBtn = collection:Get(3)
			uiView.m_okBtn = collection:Get(4)
		else
			error("BindView Error! UIComponentCollection is nil!")
		end
	else
		error("BindView Error! Panel is nil!")
	end
end

return public
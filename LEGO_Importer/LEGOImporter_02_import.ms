function li_setObjSettings =
(
	settings = objimp.getIniName()
	li_scaleFactor = rl_Importer.scaleFactor.value
	li_utilityClass.scaleFactor = li_scaleFactor
	
	setIniSetting settings "General" "Preset" "<LEGO>"
	setIniSetting settings "Units/Scale" "ObjScale" (li_scaleFactor as String)
	preset = getIniSetting settings "General" "Preset"
	print ("For importing, the Obj preset is set to " + preset )
	print ("Scale: " + (li_scaleFactor as String))
)

function fn_partLayer part = 
(
	LayerManager.newLayerFromName "Used parts"
	partsLayer = LayerManager.getLayerFromName "Used parts"
	partsLayer.addNode part
)

function fn_modelLayer part = 
(
	LayerManager.newLayerFromName "Model"
	modelLayer = LayerManager.getLayerFromName "Model"
	modelLayer.addNode part
)

function importLDraw = 
(
	fullColorLib = materialLibrary()
	solidColorLib = loadTempMaterialLibrary (li_Lib + "\Resources\LEGO_Colors_Solid.mat")
	transColorLib = loadTempMaterialLibrary (li_Lib + "\Resources\LEGO_Colors_Trans.mat")
	metalColorLib = loadTempMaterialLibrary (li_Lib + "\Resources\LEGO_Colors_Metallic.mat")
	
	for solidMat in solidColorLib do
	(
		append fullColorLib solidMat
	)
	for transpMat in transColorLib do
	(
		append fullColorLib transpMat
	)
	for metalMat in metalColorLib do
	(
		append fullColorLib metalMat
	)
	
	--TODO: Create and load transparent and metallic libraries
	
	li_setObjSettings()
	
	li_Importer = dotNetObject "LDraw.Importer"
	if (li_Importer.PreImport()) do
	(
		li_Importer.GetModels()
		li_Importer.GetPartsOrSubmodels()
		local imported = li_Importer.ImportParts()
		
		if imported == true do rl_Importer.assembleBtn.enabled = true
	)
	
	local results = li_utilityClass.GetErrors true
	rl_Status.errorList.Items = results
)
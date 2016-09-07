function li_setObjSettings =
(
	settings = objimp.getIniName()
	li_scaleFactor = rl_Importer.scaleFactor.value
	li_utilityClass.scaleFactor = li_scaleFactor
	
	-- First resets the import settings to default, then change the settings for importing the LEGO parts
	print "Resetting importer configuration"
	setIniSetting settings "General" "Preset" "<NONE>"
	setIniSetting settings "Objects" "AsEditablePoly" "1"
	setIniSetting settings "Objects" "Retriangulate" "0"
	setIniSetting settings "Geometry" "FlipZyAxis" "0"
	setIniSetting settings "Material" "UniqueWireColor" "0"
	setIniSetting settings "Material" "ImportMaterials" "0"
	setIniSetting settings "Units/Scale" "ObjScale" (li_scaleFactor as String)
	print "Importer configured"
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
	solidColorLib = loadTempMaterialLibrary (li_LibRoot + "\Resources\LEGO_Colors_Solid.mat")
	transColorLib = loadTempMaterialLibrary (li_LibRoot + "\Resources\LEGO_Colors_Trans.mat")
	metalColorLib = loadTempMaterialLibrary (li_LibRoot + "\Resources\LEGO_Colors_Metallic.mat")
	
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
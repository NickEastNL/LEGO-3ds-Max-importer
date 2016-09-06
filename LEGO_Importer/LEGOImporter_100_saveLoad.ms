function li_saveSettings =
(
	local lastFile = li_File
	local libroot = li_LibRoot
	local library = li_Lib
	local lastScale = li_scaleFactor
	
	fname = createfile ((pathConfig.GetDir #MaxData)+"scripts\\LEGO_Importer\\LEGO_Settings.ini");
	close fname
	
	local fPath = ((pathConfig.GetDir #MaxData) + "scripts\\LEGO_Importer\\LEGO_Settings.ini")
	
	setINISetting fPath "Global" "LastFile" lastFile
	setINISetting fPath "Global" "LibraryDir" library
	setINISetting fPath "Global" "LibraryRoot" libroot
	setINISetting fPath "Global" "LastScale" (lastScale as String)
)

function li_loadSettings =
(
	local fPath = ((pathConfig.GetDir #MaxData) + "scripts\\LEGO_Importer\\LEGO_Settings.ini")
	
	local lastFile = getINISetting fPath "Global" "LastFile"
	local library = getINISetting fPath "Global" "LibraryDir"
	local libroot = getINISetting fPath "Global" "LibraryRoot"
	local lastScale = (getINISetting fPath "Global" "LastScale" as Float)
	
	li_File = lastFile
	li_Lib = library
	li_LibRoot = libroot
	li_scaleFactor = lastScale
)

function li_saveLoadSettings =
(
	if (li_floaterDialog != undefined) then
	(
		if (li_floaterDialog.open == true) then
		(
			li_saveSettings()
			
			closeRolloutFloater li_floaterDialog
			clearListener()
		)
		else if (li_floaterDialog.open == false) then
		(
			closeRolloutFloater li_floaterDialog
			
			filein ((pathConfig.GetDir #MaxData)+"Scripts\\LEGO_Importer\\LEGO_Importer_rollouts.ms")
			
			if li_File != undefined do rl_Importer.filePath.text = li_File
			if li_Lib != undefined do rl_Importer.libPath.text = li_Lib
			if li_scaleFactor != undefined do rl_Importer.scaleFactor.value = li_scaleFactor as Float
			
			li_loadSettings()
			clearListener()
		)
	)
	else
	(
		filein ((pathConfig.GetDir #MaxData)+"Scripts\\LEGO_Importer\\LEGO_Importer_rollouts.ms")
		
		if li_File != undefined do rl_Importer.filePath.text = li_File
		if li_Lib != undefined do rl_Importer.libPath.text = li_Lib
		if li_scaleFactor != "" do rl_Importer.scaleFactor.value = li_scaleFactor as Float

		li_loadSettings()
		clearListener()
	)
)
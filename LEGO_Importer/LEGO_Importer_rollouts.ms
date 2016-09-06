rollout rl_Importer "Importer" width:320 height:328
(
	GroupBox 'fileSelect' "Step 1: Select file" pos:[8,8] width:304 height:128 align:#left
	
	button 'selectFile' "..." pos:[277,30] width:25 height:21 enabled:true align:#left
	edittext 'filepath' "Open File" pos:[24,32] width:244 height:16 align:#left
	
	button 'validateBtn' "Step 2: Validate" pos:[108,144] width:104 height:32 toolTip:"Validate the LDraw file" align:#left
	button 'importBtn' "Step 3: Import" pos:[108,192] width:104 height:32 enabled:false toolTip:"Import the parts" align:#left
	
	edittext 'libPath' "Parts Library" pos:[24,64] width:244 height:16 align:#left
	button 'selectLib' "..." pos:[277,63] width:25 height:21 align:#left

	GroupBox 'assembleGrp' "Step 4: Assemble" pos:[8,232] width:304 height:88 align:#left
	checkbox 'noMaterials' "No materials" pos:[118,256] width:84 height:18 toolTip:"Import model with the default 'Medium Stone Grey' material" align:#left
	button 'assembleBtn' "Assemble" pos:[108,280] width:104 height:32 enabled:false toolTip:"Assemble the model" align:#left
	spinner 'scaleFactor' "Scale" pos:[24,152] width:53 height:16 range:[0,10,0] type:#float align:#left
	dropDownList 'subLibrary' "Sublibrary" pos:[88,88] width:168 height:40 align:#left items:#("Default")
	
	on rl_Importer open do
	(
		li_loadSettings()
	)
	on rl_Importer close do
	(
		li_saveSettings()
	)
	on selectFile pressed do
	(
		li_File = getOpenFileName caption:"Open LDraw file" types:"LDraw Model (*.ldr)|*.ldr|LDraw Multi-Part (*.mpd)|*.mpd"
		if li_File == undefined then
		(
			filePath.text = ""
			print "No file chosen"; errorState = true
		) 
		else
		(
			filePath.text = li_File
		)
	)
	on validateBtn pressed do
	(
		if li_File != undefined then
		(
			validateFile()
		)
		else
		(
			print "No file selected"
		)
	)
	on importBtn pressed do
	(
		importLDraw()
	)
	on selectLib pressed do
	(
		li_LibRoot = getSavePath caption:"Select LEGO Parts Library" initialDir:"C:/"
		if li_LibRoot == undefined then
		(
			libPath.text = ""
			print "No library selected"; errorState = true
		)
		else
		(
			libPath.text = li_LibRoot
			li_Lib = li_LibRoot + @"\" + subLibrary.selected
		)
	)
	on assembleBtn pressed do
	(
		instanceLDraw()
	)
	on scaleFactor changed val do
	(
		li_scaleFactor = scaleFactor.value
	)
)

rollout rl_Status "Status" width:320 height:312
(
	listbox 'errorList' "" pos:[16,16] width:288 height:21 selection:0 readOnly:true align:#left
)

li_floaterDialog = newRolloutFloater "LEGO Importer" 330 681
addRollout rl_Importer li_floaterDialog
addRollout rl_Status li_floaterDialog
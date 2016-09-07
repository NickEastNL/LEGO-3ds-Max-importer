function validateFile =
(
	if li_Lib == undefined then
	(
		print "Library not set"; errorState = true
	)
	else
	(
		local validator = dotNetObject "LDraw.Validator"
		validator.LoadFile li_File
		local validated = validator.SetLibrary li_Lib
		validator.GetUniqueParts()
		
		if validated == true do rl_Importer.importBtn.enabled = true
		
		local results = li_utilityClass.GetErrors true
		rl_Status.errorList.Items = results
	)
)

function getSubLibraries =
(
	if li_LibRoot != undefined then
	(
		local validator = dotNetObject "LDraw.Validator"
		local libs = validator.GetSubLibraries li_LibRoot
		
		return libs
	)
)
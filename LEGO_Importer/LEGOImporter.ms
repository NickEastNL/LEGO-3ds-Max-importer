macroScript openImporter category:"LEGO" ButtonText:"Open Importer" tooltip:"Open Importer Rollout" icon:#("LEGO",1)
(
	global li_floaterDialog
	global rl_Importer
	global rl_Status
	global li_utilityClass = dotNetClass "LDraw.Utility"

	global fullColorLib
	global solidColorLib
	global transColorLib
	global li_File
	global li_Lib
	global li_Importer
	global li_scaleFactor
	global partsLayer
	global modelLayer

	on execute do
	(
		li_saveLoadSettings()
		print li_Lib
	)
)
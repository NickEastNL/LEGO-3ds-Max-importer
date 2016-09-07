scriptsDir = getDir #MaxData

getScripts = getFiles (scriptsDir + "Scripts\\LEGO_Importer\\LEGOImporter*.ms")
for f in getScripts do
(
	fileIn f
)

dotNet.loadAssembly (scriptsDir + "Scripts\\LEGO_Importer\\LDraw.dll")

print "LEGO Importer loaded"
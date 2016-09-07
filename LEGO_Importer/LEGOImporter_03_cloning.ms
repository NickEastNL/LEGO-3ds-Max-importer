function instanceLDraw = 
(
	if (li_Importer.CreateSubmodels()) do 
	(
		print "Model is assembled"
		
		if (rl_Importer.noMaterials.checked == true) do
		(
			print "No materials assigned"
		)
	)
	
	local results = li_utilityClass.GetErrors true
	rl_Status.errorList.Items = results
)

function clonePart iD pName matrixT colorID =
(
	select iD
	maxOps.cloneNodes $ cloneType:#instance newNodes:&nnl #nodialog
	select nnl
	$.name = pName
	$.wirecolor = (color 0 0 0)
	print colorID
	if (rl_Importer.noMaterials.checked == false) do
	(
		local newMat = false
		
		for mat in fullColorLib do
		(
			newMat = matchPattern mat.name pattern:colorID
			if (newMat == true) do $.material = fullColorLib[mat.name]
		)
	)
	
	LDMatrix = (matrix3 [-1,0,0] [0,0,1] [0,-1,0] [0,0,0])
	newTrans = xformMat matrixT LDMatrix
	newTrans.translation.X = -newTrans.translation.X
	
	-- MAGIC HAPPENING HERE!!! --
	newRot = inverse newTrans.rotation
	newRot.x = -newRot.x
	-- MAGIC ENDS --
	
	$.rotation = newRot
	$.position = newTrans.position
	--print (newTrans)
	--print ($.transform)
)
function qB_MissingParts =
(
	queryBox "Some parts have not been found during the validation process. Do you want to import the model excluding missing parts?" title:"Missing parts" beep:true
)

function qB_LongImport =
(
	queryBox "The current model contains many parts, which would take some time to import. Do you want to continue?" title:"Many parts" beep:true
)
# LEGO 3ds-Max importer
The "LEGO LDraw to 3ds Max importer" is a tool written with .NET and Maxscript to parse LDraw files created with tools such as LDCad, LeoCad and MLCad and assemble it in 3ds Max.

It is not a native importer plugin. It does not actually create geometry on-the-fly. Instead, it uses a custom-build parts library, available open source [here](https://github.com/stargatefreak92/LEGO-Parts-Library). The library uses `.obj` text-based files to store it on GitHub and is used by the importer to import the actual parts before assembly.

The "LDraw" folder contains the Visual Studio solution for the C# class library which contains the bulk functionality of the plugin. The "LEGO-Importer" folder contains the Maxscript files and the most stable build of the LDraw class library (as "LDraw.dll"). The folder has to be placed in "Appdata/local/Autodesk/[version]/ENU/scripts" and the "LEGOImporter_loader.ms" has to be placed in the "scripts/startup" folder. If this is correct, you should be able to find the "LEGO" category in the "Customize User Interface" menu and the "Open Importer Rollout" command.

## Process
The process of how the importer assembles the models is important to understand. It is possible that Max might freeze during the process, which does not mean it crashed. It should also be mentioned that you should start with an empty scene (using the "Reset Scene" option).

1. With the dialog open, select an LDraw file (see below for a description).
2. Select the parts library root directory (if cloned from GitHub, it should be called "LEGO-Parts-Library").
3. Click the "Validate" button to parse the file and check for errors. This should list all the parts which were found in the parts library as well as those not found (see below for more information).
4. The "Import" button should enable. Clicking it will cause the Importer to import the unique parts from the library into the current scene.
5. Once imported, the "Assemble" button becomes available. Clicking it will assemble the model using the parts that have been imported. (This process might freeze Max, don't worry, it still progresses in the background. Just wait for it to finish.)

The fully assembled model is grouped under the "Model" layer, while the imported "master" parts are stored in the "Used Parts" layer. The Used Parts layer will be hidden after assembling the model.

### LDraw file
You can load any LDraw file created from the available LDraw tools. Such a file typically has the `.ldr` extension. LDraw also supports sub-models where a single model is comprised of multiple groups of parts. These are typically saved with the `.mpd` extension and are known as "Multi-Part Documents". Both file-types can be parsed by the Importer.

MPD files will create point helpers with the sub-model name and the associated parts as children. Note that only the sub-models themselves will be assembled. They will not be instanced and placed at the correct transform, which means you have to manually copy and place them.

### Parts not found
After validating the file, the status list will show all the unique parts used in the file that were found as well as those that were not. If one or more parts were not found in the parts library, that means they weren't created yet, and the Importer will show a warning dialog when clicking "Import". You can still import and assemble the model, but the tool will ignore the missing parts.

See the readme of the [LEGO Parts library](https://github.com/stargatefreak92/LEGO-Parts-Library) for more information regarding missing parts.

### Empty scene
You should always start with an empty scene before imported a LEGO model, preferably by using the "Reset Scene" menu option to completely reset the scene when you have been working with another one. If you have to reimport the same model again, for example because you've added the missing parts or added new parts to the model, you can just run the importer again. It will ignore any part that was already imported and placed. However, if parts have been moved or deleted from the model, the importer will not take this into account. In this case, you should remove the entire assembled model and run the import and assembly commands again.

There is one more issue. To avoid having to reset XForms and lose the instance relation between the cloned parts, scale adjustments are done in the OBJ importer. You can also rescale the model in Max using the "Rescale World Units" utility without having to reset XForms and losing the instance relation. However, if you want to reimport the model, with new parts and change the scale, you have to remove everything, including the hidden parts. Otherwise, some parts will be scaled incorrectly compared to others. Reasonably complex models are reasonably fast to import, though running it on an already imported/assembled model will slow the importer as it has to check which parts already exist.

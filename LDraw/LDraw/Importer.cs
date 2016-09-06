using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using Autodesk.Max;
using ManagedServices;

namespace LDraw
{
	/// <summary>
	/// Parses the file and imports each part
	/// </summary>
	public class Importer
	{
		IGlobal global = GlobalInterface.Instance;

		List<Part> allParts = new List<Part>();
		List<Model> models = new List<Model>();
		List<Submodel> subModels = new List<Submodel>();

		List<string> entries = Utility.fileLines;
		
		public bool PreImport()
		{
			IFPValue importQuery = global.FPValue.Create();
			IFPValue errorQuery = global.FPValue.Create();
			bool result = true;

			if (Utility.uniqueParts.Count > 50)
			{
				global.ExecuteMAXScriptScript("qB_LongImport()", false, importQuery);
				result = importQuery.B;
			}

			if (result != false && Utility.missingParts.Count > 0)
			{
				global.ExecuteMAXScriptScript("qB_MissingParts()", false, errorQuery);
				result = errorQuery.B;
			}

			return result;
		}

		/// <summary>
		/// Parses the file lines to get each model of an MPD file,
		/// or just one if it's a normal LDraw file
		/// </summary>
		/// <returns></returns>
		public bool GetModels()
		{
			// Checks if the current file is a Multi-Part Document
			if (Utility.isMPD == true)
			{
				for (int i = 0; i < entries.Count; i++)
				{
					if (entries[i].StartsWith("0 FILE"))
					{
						Regex fileRegex = new Regex(@"^0 FILE\s([^\.]+)");
						Match fileName = fileRegex.Match(entries[i]);

						Model newModel = new Model();
						newModel.ModelName = fileName.Result("$1");
						models.Add(newModel);

						Console.WriteLine(newModel.ModelName);
					}
				}
				
				return true;
			}
			else // If not, create a single main model
			{
				Model newModel = new Model();
				newModel.ModelName = "Main";
				models.Add(newModel);

				Console.WriteLine(newModel.ModelName);

				return true;
			}
		}

		/// <summary>
		/// Parses the file lines to get all parts and/or submodels with their transforms and color IDs
		/// </summary>
		/// <returns></returns>
		public bool GetPartsOrSubmodels()
		{
			// A regular expression used to extract the data from a line that defines a part or submodel
			Regex partRegex = new Regex(@"^.\s([^\s]+)\s([^\s]+)\s([^\s]+)\s([^\s]+)\s([^\s]+)\s([^\s]+)\s([^\s]+)\s([^\s]+)\s([^\s]+)\s([^\s]+)\s([^\s]+)\s([^\s]+)\s([^\s]+)\s([^\.]+)");

			if (Utility.isValidated == true)
			{
				// If the file is an MPD, it will sort each part and submodel in the appropriate parent model
				if (Utility.isMPD == true)
				{
					int index = 0;

					// Goes through each model found
					foreach (var model in models)
					{
						// Searches for the index in the array where the model is defined
						var currentModel = entries.FindIndex(x => x.StartsWith("0 FILE " + model.ModelName));
						index = currentModel;

						Utility.WriteListener(index + ": " + model.ModelName);
						Console.WriteLine(index + ": " + model.ModelName);

						do // Loops through each line until it reaches the next "0 FILE" line or the end of file
						{
							// Parses a LEGO part instance
							if (!entries[index].StartsWith("0 FILE") && entries[index].EndsWith(".dat"))
							{
								Match values = partRegex.Match(entries[index]);

								string colorID = values.Result("$1");
								string partID = values.Result("$14");

								var duplicateParts = allParts.FindAll(x => x.PartID == partID);

								if (Utility.uniqueParts.Find(x => x.PartID == partID).Exists == true)
								{
									Part newPart = new Part();
									newPart.PartName = partID + "_" + (duplicateParts.Count + 1).ToString().PadLeft(3, '0');
									newPart.PartID = partID;
									newPart.ColorID = colorID;
									newPart.Transform = GetTransform(values);
									newPart.ParentModel = model;

									allParts.Add(newPart);
									model.Parts.Add(newPart);

									Utility.WriteListener(newPart.ParentModel.ModelName + "::Part: " + newPart.PartName);
									Console.WriteLine(newPart.ParentModel.ModelName + "::Part: " + newPart.PartName);
								}
							}

							// Parses a model instance
							if (!entries[index].StartsWith("0 FILE") && entries[index].EndsWith(".ldr"))
							{
								Match values = partRegex.Match(entries[index]);
								
								string submodelName = values.Result("$14");

								var duplicateModel = subModels.FindAll(x => x.SourceModel.ModelName == submodelName);

								if (models.Exists(x => x.ModelName == submodelName))
								{
									Submodel newSubmodel = new Submodel();
									newSubmodel.SubmodelName = submodelName + "_" + (duplicateModel.Count + 1).ToString().PadLeft(3, '0');
									newSubmodel.Transform = GetTransform(values);
									newSubmodel.SourceModel = models.Find(x => x.ModelName == submodelName);
									newSubmodel.ParentModel = model;

									//model.IsDependency = true;

									subModels.Add(newSubmodel);
									model.Submodels.Add(newSubmodel);

									Utility.WriteListener(newSubmodel.ParentModel.ModelName + "::Model: " + newSubmodel.SubmodelName);
									Console.WriteLine(newSubmodel.ParentModel.ModelName + "::Model: " + newSubmodel.SubmodelName);
								}
							}

							index++;
						} while (index < entries.Count && !entries[index].StartsWith("0 FILE"));


						Utility.WriteListener("Model contains submodels: " + model.IsDependency);
						Console.WriteLine("Model contains submodels: " + model.IsDependency);
					}

					return true;
				}
				else // If the file is not an MPD, just add all parts to a single main model
				{
					for (int i = 0; i < entries.Count; i++)
					{
						Match values = partRegex.Match(entries[i]);

						string colorID = values.Result("$1");
						string partID = values.Result("$14");

						var duplicateParts = allParts.FindAll(x => x.PartID == partID);

						if (Utility.uniqueParts.Find(x => x.PartID == partID).Exists == true)
						{
							Part newPart = new Part();
							newPart.PartName = partID + "_" + (duplicateParts.Count + 1).ToString().PadLeft(3, '0');
							newPart.PartID = partID;
							newPart.ColorID = colorID;
							newPart.Transform = GetTransform(values);
							newPart.ParentModel = models[0];

							allParts.Add(newPart);
							models[0].Parts.Add(newPart);

							Console.WriteLine(newPart.ParentModel.ModelName + ": " + newPart.PartName);
						}
					}

					return true;
				}
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Imports the unique parts so only one copy exists in the scene
		/// </summary>
		/// <returns></returns>
		public bool ImportParts()
		{
			if (Utility.uniqueParts.Count > 0)
			{
				Utility.WriteListener("Importing parts...");

				for (var i = 0; i < Utility.uniqueParts.Count; i++)
				{
					if (!SearchParts(Utility.uniqueParts[i].PartID) && Utility.uniqueParts[i].Exists == true)
					{
						Utility.WriteListener(Utility.uniqueParts[i].Path);
						global.ExecuteMAXScriptScript("ImportFile \"" + Utility.uniqueParts[i].Path + "\" #noprompt", false, null); // Executes a script to import the desired file according the stores file path
						global.ExecuteMAXScriptScript("$.wirecolor = (color 0 0 0)", false, null);
						global.ExecuteMAXScriptScript("$.material = fullColorLib[\"71 - Medium Stone Grey\"]", false, null);
						global.ExecuteMAXScriptScript("fn_partLayer $" + Utility.uniqueParts[i].PartID, false, null);
					}

					var percentage = (100.0f * i) / Utility.uniqueParts.Count;
					//global.ExecuteMAXScriptScript("progressUpdate " + percentage, false, null);
				}
				//global.ExecuteMAXScriptScript("progressEnd()", false, null);
				Utility.ClearErrors();
				Utility.results.Add("Import successfull!");
				return true;
			}
			else
			{
				Utility.WriteListener("No parts to import");
				return false;
			}
		}

		/// <summary>
		/// Creates the submodels in the scene
		/// </summary>
		/// <returns></returns>
		public bool CreateSubmodels()
		{
			if (Utility.isValidated == true)
			{
				if (Utility.isMPD == true)
				{
					Utility.WriteListener("Assembling submodels...");

					for (int i = 0; i < models.Count; i++)
					{
						global.ExecuteMAXScriptScript("newPoint = point()", false, null);
						global.ExecuteMAXScriptScript("newPoint.box = true", false, null);
						global.ExecuteMAXScriptScript("newPoint.size = 40", false, null);
						global.ExecuteMAXScriptScript("newPoint.wirecolor = color 0 252 0", false, null);
						global.ExecuteMAXScriptScript("newPoint.name = \"" + models[i].ModelName + "\"", false, null);
						InstanceParts(models[i], true);

						//if (models[i].IsDependency != true)
						//{
						//	global.ExecuteMAXScriptScript("newPoint = point()", false, null);
						//	global.ExecuteMAXScriptScript("newPoint.box = true", false, null);
						//	global.ExecuteMAXScriptScript("newPoint.name = \"" + models[i].ModelName + "\"", false, null);
						//	InstanceParts(models[i], true);
						//}
						//if (models[i].IsDependency == true)
						//{
						//	InstanceParts(models[i]);
						//}
					}

					return true;
				}
				else
				{
					InstanceParts(models[0]);

					return true;
				}
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Creates instances of each part within each model
		/// </summary>
		/// <param name="model"></param>
		public void InstanceParts(Model model, bool isSubModel = false)
		{
			Utility.WriteListener("Instancing parts for \"" + model.ModelName + "\"");

			List<Part> parts = model.Parts;
			List<Submodel> submodels = model.Submodels;

			for (int i = 0; i < model.Parts.Count; i++)
			{
				if (Utility.uniqueParts.Find(x => x.PartID == parts[i].PartID).Exists == true)
				{
					if (SearchParts(parts[i].PartID))
					{
						if (!SearchParts(parts[i].PartName))
						{
							Utility.WriteListener(parts[i].PartName + " part doesn't exist");

							string transform = SetTransform(parts[i].Transform);

							global.ExecuteMAXScriptScript("clonePart $" + parts[i].PartID + " " + "\"" + parts[i].PartName + "\" " + transform + " \"" + parts[i].ColorID + " -*\"", false, null);
							global.ExecuteMAXScriptScript("fn_modelLayer $" + parts[i].PartName, false, null);

							if (isSubModel == true)
							{
								global.ExecuteMAXScriptScript("$" + parts[i].PartName + ".parent = $" + model.ModelName, false, null);
							}
						}
						else
						{
							Utility.WriteListener("Instance already exists");
						}
					}
					else
					{
						Utility.WriteListener("Part does not exist in scene");
					}
				}
			}

			for (int i = 0; i < model.Submodels.Count; i++)
			{
				if (!SearchParts(submodels[i].SubmodelName))
				{
					Utility.WriteListener(submodels[i].SubmodelName + " helper doesn't exist");

					string transform = SetTransform(submodels[i].Transform);

					global.ExecuteMAXScriptScript("clonePart $" + model.ModelName + " " + "\"PL_" + submodels[i].SubmodelName + "\" " + transform + " \"" + null + " -*\"", false, null);
					global.ExecuteMAXScriptScript("$.box = true", false, null);
					global.ExecuteMAXScriptScript("$.size = 50", false, null);
					global.ExecuteMAXScriptScript("$.wirecolor = color 0 252 252", false, null);
					global.ExecuteMAXScriptScript("fn_modelLayer $" + submodels[i].SubmodelName, false, null);


					if (isSubModel == true)
					{
						global.ExecuteMAXScriptScript("$PL_" + submodels[i].SubmodelName + ".parent = $" + model.ModelName, false, null);
					}
				}
			}

			Utility.ClearErrors();
			Utility.results.Add("Assembly of \"" + model.ModelName + "\" successfull");

			global.ExecuteMAXScriptScript("partsLayer.ishidden = true", false, null);
		}

		/// <summary>
		/// Extracts the transformation matrix from a line
		/// </summary>
		/// <param name="line"></param>
		/// <returns></returns>
		public IMatrix3 GetTransform(Match values)
		{
			// Row 1
			float row1x;
			float.TryParse(values.Result("$5"), out row1x);
			float row1y;
			float.TryParse(values.Result("$6"), out row1y);
			float row1z;
			float.TryParse(values.Result("$7"), out row1z);

			// Row 2
			float row2x;
			float.TryParse(values.Result("$8"), out row2x);
			float row2y;
			float.TryParse(values.Result("$9"), out row2y);
			float row2z;
			float.TryParse(values.Result("$10"), out row2z);

			// Row 3
			float row3x;
			float.TryParse(values.Result("$11"), out row3x);
			float row3y;
			float.TryParse(values.Result("$12"), out row3y);
			float row3z;
			float.TryParse(values.Result("$13"), out row3z);

			// Position
			float posX;
			float.TryParse(values.Result("$2"), out posX);
			float posY;
			float.TryParse(values.Result("$3"), out posY);
			float posZ;
			float.TryParse(values.Result("$4"), out posZ);
			//posY = -posY;

			IPoint3 row1 = global.Point3.Create(row1x, row1y, row1z);
			IPoint3 row2 = global.Point3.Create(row2x, row2y, row2z);
			IPoint3 row3 = global.Point3.Create(row3x, row3y, row3z);
			IPoint3 position = global.Point3.Create(posX, posY, posZ);

			IMatrix3 matrix = global.Matrix3.Create(row1, row2, row3, position);

			return matrix;
			//return null;
		}

		public string SetTransform(IMatrix3 matrix)
		{
			var row1T = matrix.GetRow(0);
			var row2T = matrix.GetRow(1);
			var row3T = matrix.GetRow(2);
			var row4T = matrix.GetRow(3);

			string row1 = "[" + row1T.X + ", " + row1T.Y + ", " + row1T.Z + "]";
			string row2 = "[" + row2T.X + ", " + row2T.Y + ", " + row2T.Z + "]";
			string row3 = "[" + row3T.X + ", " + row3T.Y + ", " + row3T.Z + "]";
			string row4 = "[" + (row4T.X * Utility.scaleFactor) + ", " + (row4T.Y * Utility.scaleFactor) + ", " + (row4T.Z * Utility.scaleFactor) + "]";
			string transform = "(matrix3 " + row1 + " " + row2 + " " + row3 + " " + row4 + ")";

			return transform;
		}

		/// <summary>
		/// Searches for a part in the scene
		/// </summary>
		/// <param name="partID"></param>
		/// <returns>Returns true if the part already exists</returns>
		public bool SearchParts(string partID)
		{
			try
			{
				var script = global.ExecuteMAXScriptScript("select $" + partID, true, null);
				//Utility.WriteListener(script.ToString());
				if (script)
				{
					//Utility.WriteListener(partID + " exists");
					//Utility.WriteListener("True");
					return true;
				}
				else
				{
					//Utility.WriteListener("False");
					return false;
				}
			}
			catch (Exception e)
			{
				Utility.WriteListener("Error: " + e.Message);
				return false;
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using Autodesk.Max;

namespace LDraw
{
	/// <summary>
	/// Parses LDraw files and validates its contents before importing
	/// </summary>
	public class Validator
    {
		IGlobal global = GlobalInterface.Instance;
		
		public string objPath;
		
		public Validator()
		{
			Utility.fileExists = false;
			Utility.libraryExists = false;
			Utility.isValidated = false;
			Utility.isMPD = false;

			Utility.fileLines.Clear();
			Utility.uniqueParts.Clear();

			Utility.ClearErrors();
		}

		/// <summary>
		/// Retrieves the sub-libraries, identified by their "L_" prefix.
		/// 3ds Max will display a list in a dropdown menu to select which library to use when importing parts
		/// </summary>
		/// <param name="absPath">The absolute path to the root of the library</param>
		/// <returns>Array of strings with readable names for each library, or null if none are found</returns>
		public string[] GetSubLibraries(string absPath)
		{
			if (absPath != null)
			{
				string[] SubLibsPaths = Directory.GetDirectories(absPath, "L_*");
				List<string> SubLibsList = new List<string>();
				string[] SubLibs;

				foreach (var path in SubLibsPaths)
				{
					string DirName = new DirectoryInfo(path).Name;
					string LibName = DirName.Substring(2);

					SubLibsList.Add(LibName);
				}

				SubLibs = SubLibsList.ToArray();
				return SubLibs;
			}

			return null;
		}
		
		/// <summary>
		/// Loads the LDraw file and saves each line as a string ready for parsing
		/// </summary>
		/// <param name="absPath">The absolute path to the LDraw file (including its name and extension)</param>
		/// <returns>Returns true if the file is found and false if not or null</returns>
        public bool LoadFile(string absPath)
		{
			if (absPath != null)
			{
				Utility.filePath = absPath;

				// Checks if the file exists, then reads it
				if (File.Exists(Utility.filePath))
				{
					var reader = new StreamReader(File.OpenRead(Utility.filePath)); // Opens the file in a reader

					// Reads the file and parses each line to an array
					string line;
					while ((line = reader.ReadLine()) != null)
					{
						if (line.StartsWith("0 FILE"))
						{
							Utility.isMPD = true;
						}
						if (line.StartsWith("0 FILE") || line.StartsWith("1"))
						{
							Utility.fileLines.Add(line);
						}
					}

					if (Utility.fileLines.Count > 0)
					{
						Utility.SetErrors(ErrorType.RESULT, "LDraw file loaded and parsed");
						if (Utility.isMPD == true)
						{
							Utility.SetErrors(ErrorType.RESULT, "LDraw file is a Multi-Part document");
						}
					}
					else
					{
						Utility.SetErrors(ErrorType.RESULT, "LDraw file not the correct format");
					}

					Utility.fileExists = true;
					return true;
				}
				else
				{
					Utility.SetErrors(ErrorType.ERROR, "LDraw file not found");
					return false;
				}
			}

			Utility.SetErrors(ErrorType.ERROR, "No file selected");
			return false;
		}

		/// <summary>
		/// Sets the library path and checks if it is correct
		/// </summary>
		/// <param name="absLibPath">The absolute path to the root of the library</param>
		/// <returns>Returns true if the library is found and false if it is incorrect or null</returns>
		public bool SetLibrary(string absLibPath)
		{
			if (absLibPath != null)
			{
				Utility.libraryPath = absLibPath;
				
				// Checks if the library is valid by checking if a specific file exists
				if (File.Exists(Utility.libraryPath + "/categories.xml"))
				{
					Utility.SetErrors(ErrorType.RESULT, "Library exists");

					Utility.libraryExists = true;

					return true;
				}
				else
				{
					Utility.SetErrors(ErrorType.ERROR, "Incorrect folder selected. Requires 'categories.xml'");

					return false;
				}
			}

			Utility.SetErrors(ErrorType.ERROR, "Library not set");
			return false;
		}

		/// <summary>
		/// Parses the file line by line, then gets each unique part used and stores a reference in an array
		/// </summary>
		/// <returns></returns>
		public bool GetUniqueParts()
		{
			if (Utility.fileExists == true && Utility.libraryExists == true)
			{
				// Regular expression to extract the part ID from the whole line
				Regex regex = new Regex(@".*\s(.*)\.");

				// Loops through each entry (line) in the array
				foreach (var entry in Utility.fileLines)
				{
					if (entry.StartsWith("1") && entry.EndsWith(".dat")) // Line is a sub-part
					{
						// Gets each part ID from the whole line
						Match value = regex.Match(entry);
						string partID = value.Result("$1");

						// Check if the current part ID already exists in the list
						if (!Utility.uniqueParts.Exists(x => x.PartID == partID)) // If not, add it
						{
							UniquePart newPart = new UniquePart();
							newPart.PartID = partID;
							newPart.Amount = 1;

							// Checks if the actual part file exists
							if (FindPart(partID))
							{
								newPart.Exists = true;
								newPart.Path = objPath;
								//Utility.WriteListener(newPart.Path);
								//Console.WriteLine(newPart.Path);
							}

							Utility.uniqueParts.Add(newPart);
						}
						else // If it exists, increment the Amount
						{
							var currentPart = Utility.uniqueParts.Find(x => x.PartID == partID);
							currentPart.Amount++;
						}
					}
				}

				Utility.SetErrors(ErrorType.RESULT, "Unique parts succesfully parsed");
				Utility.isValidated = true; // Sets the validation as finished
				return true;
			}

			Utility.SetErrors(ErrorType.ERROR, "File or library wasn't loaded correctly, or this function was called prematurely");
			return false;
		}

		/// <summary>
		/// Searches the library for a part file
		/// </summary>
		/// <returns>Returns true if the part is found, false if not</returns>
		public bool FindPart(string partID)
		{
			// Searches for the ".obj" file with the part ID as name in the root and all subdirectories
			var file = Directory.GetFiles(Utility.libraryPath, partID + ".obj", SearchOption.AllDirectories);

			if (file.Length > 0)
			{
				Utility.SetErrors(ErrorType.FOUND, partID);
				objPath = file[0];
				return true;
			}
			else
			{
				Utility.SetErrors(ErrorType.MISSING, partID);
				return false;
			}
		}
    }
}

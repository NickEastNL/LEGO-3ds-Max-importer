#define MAX

using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Max;

public enum ErrorType
{
	ERROR,
	MISSING,
	RESULT,
	FOUND
}

namespace LDraw
{
	public static class Utility
	{
		public static string filePath; // The absolute path to the LDraw file, including name and extension
		public static string libraryPath; // The absolute path to the root directory of the parts library

		public static bool fileExists = false; // If the LDraw file exists or is valid
		public static bool libraryExists = false; // If the library is valid
		public static bool isValidated = false; // If the validation is succesfull
		public static bool isMPD = false; // If the current file is a multi-part model

		public static float scaleFactor = 1.0f;

		// Stores each line in the file
		public static List<string> fileLines = new List<string>();
		// Stores each unique part found in the file
		public static List<UniquePart> uniqueParts = new List<UniquePart>();

		public static List<string> missingParts = new List<string>();
		public static List<string> foundParts = new List<string>();
		public static List<string> errors = new List<string>();
		public static List<string> results = new List<string>();

		/// <summary>
		/// Outputs a string to the Maxscript Listener
		/// </summary>
		/// <param name="input"></param>
		public static void WriteListener(string input)
		{
#if MAX
			GlobalInterface.Instance.TheListener.EditStream.Wputs(input + "\n");
#endif
		}

		public static void WriteListener(int input)
		{
#if MAX
			GlobalInterface.Instance.TheListener.EditStream.Wputs(input + "\n");
#endif
		}

		public static void WriteListener(float input)
		{
#if MAX
			GlobalInterface.Instance.TheListener.EditStream.Wputs(input + "\n");
#endif
		}

		/// <summary>
		/// Creates an entry in any of the results/errors lists
		/// </summary>
		/// <param name="type"></param>
		/// <param name="message"></param>
		public static void SetErrors(ErrorType type, string message)
		{
			if (type == ErrorType.ERROR)
			{
				errors.Add(message);
			}

			if (type == ErrorType.MISSING)
			{
				missingParts.Add(message);
			}

			if (type == ErrorType.FOUND)
			{
				foundParts.Add(message);
			}

			if (type == ErrorType.RESULT)
			{
				results.Add(message);
			}
		}

		/// <summary>
		/// Clears all error and result lists
		/// </summary>
		public static void ClearErrors()
		{
			missingParts.Clear();
			foundParts.Clear();
			errors.Clear();
			results.Clear();
		}

		/// <summary>
		/// Retrieve all results and errors
		/// </summary>
		/// <param name="returnArray"></param>
		/// <returns>Returns a string array with all list entries sorted</returns>
		public static string[] GetErrors(bool returnArray = false)
		{
			if (foundParts.Count > 0 && missingParts.Count == 0 && errors.Count == 0)
			{
				//Console.WriteLine("Validation succesfull!");
				WriteListener("Validation succesfull!");
			}

			if (results.Count > 0)
			{
				results.Add("-----------------------------------------");
				foreach (var result in results)
				{
					//Console.WriteLine(result);
					WriteListener(result);
				}
			}

			if (foundParts.Count > 0)
			{
				foundParts.Sort();

				foundParts.Insert(0, "--------Below are the parts found--------");
				foundParts.Add("-------------------------------------------");

				foreach (var part in foundParts)
				{
					//Console.WriteLine(part);
					WriteListener(part);
				}
			}

			if (missingParts.Count > 0)
			{
				missingParts.Sort();

				missingParts.Insert(0, "--------Below are the parts that were not found--------");
				missingParts.Add("-------------------------------------------------------");

				//Console.WriteLine("Some parts could not be found in the library.");
				//WriteListener("Some parts could not be found in the library.");

				foreach (var part in missingParts)
				{
					//Console.WriteLine(part);
					WriteListener(part);
				}
			}

			if (errors.Count > 0)
			{
				//Console.WriteLine("There are errors.");
				//WriteListener("There are errors.");

				foreach (var error in errors)
				{
					//Console.WriteLine(error);
					WriteListener(error);
				}
			}

			if (returnArray)
			{
				string[] finalResults;
				var cResults = results.Union(foundParts).Union(missingParts).Union(errors).ToList();
				finalResults = cResults.ToArray();
				return finalResults;
			}
			else
			{
				string[] finalResults;
				finalResults = results.ToArray();
				return finalResults;
			}
		}
	}
}

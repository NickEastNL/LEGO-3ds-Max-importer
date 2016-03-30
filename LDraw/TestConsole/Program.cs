using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LDraw;

namespace TestConsole
{
	class Program
	{
		static void Main(string[] args)
		{
			var validator = new LDraw.Validator();
			var importer = new LDraw.Importer();
			
			validator.LoadFile(@"E:\Development\Projects\LEGO Nexus\Concept\Models\Spaceships\Venture Explorer\Venture Explorer.mpd");
			//validator.LoadFile(@"E:\Development\Projects\LEGO Nexus\Concept\Models\Spaceships\Venture Explorer\Parts.ldr");
			//validator.LoadFile(@"D:\Development\Projects\LEGO Nexus\Assets\Scripts\3ds Max\LDraw\LDraw\LDrawTest.ldr");
			validator.SetLibrary(@"E:\Development\Projects\LEGO Parts Library");
			validator.GetUniqueParts();
			importer.GetModels();
			importer.GetPartsOrSubmodels();

			Utility.GetErrors(false);

			Console.ReadKey();

			//importer.GetParts();

			//Console.ReadKey();
		}
	}
}

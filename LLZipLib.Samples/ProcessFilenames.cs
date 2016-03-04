namespace LLZipLib.Samples
{
	internal class ProcessFilenames
	{
		private static int Main(string[] args)
		{
			if (args.Length == 0)
				return 1;
			var filename = args[0];

			var zip = ZipArchive.Read(filename);

			foreach (var entry in zip.Entries)
			{
				entry.LocalFileHeader.Filename = "foo." + entry.LocalFileHeader.Filename;
				entry.CentralDirectoryHeader.Filename = entry.LocalFileHeader.Filename;
			}

			zip.Write(filename);
			return 0;
		}
	}
}

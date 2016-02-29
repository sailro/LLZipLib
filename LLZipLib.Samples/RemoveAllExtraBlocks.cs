namespace LLZipLib.Samples
{
	internal class RemoveAllExtraBlocks
	{
		private static int Main(string[] args)
		{
			if (args.Length == 0)
				return 1;
			var filename = args[0];

			var zip = new ZipArchive();
			zip.Read(filename);

			foreach (var entry in zip.Entries)
			{
				entry.LocalFileHeader.Extra = new byte[0];
				entry.CentralDirectoryHeader.Extra = new byte[0];
			}

			zip.Write(filename);
			return 0;
		}
	}
}

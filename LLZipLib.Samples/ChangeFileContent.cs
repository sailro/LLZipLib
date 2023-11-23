﻿namespace LLZipLib.Samples;

internal class ChangeFileContent
{
	private static int Main(string[] args)
	{
		if (args.Length == 0)
			return 1;
		var filename = args[0];

		var zip = ZipArchive.Read(filename);

		var entry = zip.Entries.FirstOrDefault(e => e.LocalFileHeader.Filename == "readme.txt");
		if (entry != null)
		{
			entry.Data = zip.StringConverter.GetBytes("This is my new content", StringConverterContext.Content);
			// this is not compressed
			entry.LocalFileHeader.Compression = entry.CentralDirectoryHeader.Compression = 0;
		}

		zip.Write(filename);
		return 0;
	}
}

namespace LLZipLib.Samples;

internal class RemoveAllDataDescriptors
{
	private static int Main(string[] args)
	{
		if (args.Length == 0)
			return 1;
		var filename = args[0];

		var zip = ZipArchive.Read(filename);

		foreach (var entry in zip.Entries.Where(entry => entry.HasDataDescriptor))
		{
			entry.LocalFileHeader.CompressedSize = entry.DataDescriptor.CompressedSize;
			entry.LocalFileHeader.UncompressedSize = entry.DataDescriptor.UncompressedSize;
			entry.LocalFileHeader.Crc = entry.DataDescriptor.Crc;
			entry.HasDataDescriptor = false;
		}

		zip.Write(filename);
		return 0;
	}
}

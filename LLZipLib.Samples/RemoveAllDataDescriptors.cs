using System.Linq;

namespace LLZipLib.Samples
{
	internal class RemoveAllDataDescriptors
	{
		private static int Main(string[] args)
		{
			if (args.Length == 0)
				return 1;
			var filename = args[0];

			var zip = new ZipArchive();
			zip.Read(filename);

			foreach (var entry in zip.Entries.Where(entry => entry.HasDataDescriptor))
			{
				entry.LocalFileHeader.CompressedSize = entry.DataDescriptor.CompressedSize;
				entry.LocalFileHeader.UncompressedSize = entry.DataDescriptor.UncompressedSize;
				entry.LocalFileHeader.Crc = entry.DataDescriptor.Crc;
				entry.LocalFileHeader.Flags = (ushort)(entry.LocalFileHeader.Flags & ~4); 
				entry.DataDescriptor = null;
			}

			zip.Write(filename);
			return 0;
		}
	}
}

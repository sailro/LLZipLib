namespace LLZipLib.Samples
{
	internal class CreateArchive
	{
		private static void Main()
		{
			var zip = new ZipArchive();
			var entry = new ZipEntry();

			entry.LocalFileHeader.Filename = entry.CentralDirectoryHeader.Filename = "foo.txt";
			entry.Data = zip.StringConverter.GetBytes("Hello world!", StringConverterContext.Content);
			zip.Entries.Add(entry);

			zip.Write("foo.zip");
		}
	}
}

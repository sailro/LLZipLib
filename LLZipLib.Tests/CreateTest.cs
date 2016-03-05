using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LLZipLib.Tests
{
	[TestClass]
	public class CreateTest : BaseTest
	{
		[TestMethod]
		public void CreateEmptyArchive()
		{
			var compareFile = Path.Combine(_filesDirectory, "empty.archive.zip");
			var tmpFile = Path.GetTempFileName();
			var zip = new ZipArchive();
			zip.Write(tmpFile);
			FileHelper.CompareFiles(compareFile, tmpFile);
		}

		[TestMethod]
		public void CreateSimpleArchive()
		{
			var compareFile = Path.Combine(_filesDirectory, "simple.zip");
			var tmpFile = Path.GetTempFileName();
			var zip = new ZipArchive();

			var entry = new ZipEntry();
			zip.Entries.Add(entry);

			entry.Data = zip.StringConverter.GetBytes("Foo", StringConverterContext.Content);
			entry.LocalFileHeader.Filename = entry.CentralDirectoryHeader.Filename = "Foo.txt";
			entry.LocalFileHeader.Version = 0xa;
			entry.CentralDirectoryHeader.Version = 0xa;
			entry.CentralDirectoryHeader.VersionNeeded = 0xa;

			zip.Write(tmpFile);
			FileHelper.CompareFiles(compareFile, tmpFile);
		}
	}
}


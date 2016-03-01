using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LLZipLib.Tests
{
	[TestClass]
	public class RoundTripTest : BaseTest
	{
		[TestMethod]
		public void TestRoundTrip()
		{
			foreach (var file in Directory.GetFiles(_filesDirectory))
			{
				TestContext.WriteLine("Testing {0}", file);
				var zip = new ZipArchive();
				zip.Read(file);
				var tmpFile = Path.GetTempFileName();

				foreach (var zipEntry in zip.Entries)
				{
					// this will test the IStringConverter default implementation
					zipEntry.CentralDirectoryHeader.Comment = zipEntry.CentralDirectoryHeader.Comment;
					zipEntry.CentralDirectoryHeader.Filename = zipEntry.CentralDirectoryHeader.Filename;
					zipEntry.LocalFileHeader.Filename = zipEntry.LocalFileHeader.Filename;
				}

				zip.Write(tmpFile);
				FileHelper.CompareFiles(file, tmpFile);
			}
		}
	}
}


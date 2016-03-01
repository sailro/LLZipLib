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
	}
}


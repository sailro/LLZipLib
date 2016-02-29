using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LLZipLib.Test
{
	[TestClass]
	public class RoundTripTest
	{
		private TestContext _testContextInstance;
		private string _filesDirectory;

		public TestContext TestContext
		{
			get
			{
				return _testContextInstance;
			}
			set
			{
				_testContextInstance = value;
				_filesDirectory = Path.Combine(_testContextInstance.TestDir, @"..\..");
				_filesDirectory = Path.Combine(_filesDirectory, @"LLZipLib.Test\Files");
				_filesDirectory = Path.GetFullPath(_filesDirectory);
			}
		}

		private static void CompareFiles(BinaryReader reoriginal, BinaryReader recompared)
		{
			Assert.AreEqual(reoriginal.BaseStream.Length, recompared.BaseStream.Length, "Size mismatch");
			while (reoriginal.BaseStream.Position < reoriginal.BaseStream.Length)
			{
				var b1 = reoriginal.ReadByte();
				var b2 = recompared.ReadByte();
				Assert.AreEqual(b1, b2, "Position: {0}", reoriginal.BaseStream.Position);
			}
		}

		private static void CompareFiles(Stream soriginal, Stream scompared)
		{
			using (var reoriginal = new BinaryReader(soriginal))
			{
				using (var recompared = new BinaryReader(scompared))
				{
					CompareFiles(reoriginal, recompared);
				}
			}
		}

		private static void CompareFiles(string original, string compared)
		{
			using (var fsoriginal = new FileStream(original, FileMode.Open))
			{
				using (var fscompared = new FileStream(compared, FileMode.Open))
				{
					CompareFiles(fsoriginal, fscompared);
				}
			}
		}

		[TestMethod]
		public void TestRoundTrip()
		{
			foreach (var file in Directory.GetFiles(_filesDirectory))
			{
				_testContextInstance.WriteLine("Testing {0}", file);
				var zip = new ZipArchive();
				zip.Read(file);
				var tmpFile = Path.GetTempFileName();

				zip.Write(tmpFile);
				CompareFiles(file, tmpFile);
			}
		}
	}
}


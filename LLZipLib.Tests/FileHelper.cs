using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LLZipLib.Tests
{
	public static class FileHelper
	{
		public static void CompareFiles(BinaryReader reoriginal, BinaryReader recompared)
		{
			Assert.AreEqual(reoriginal.BaseStream.Length, recompared.BaseStream.Length, "Size mismatch");
			while (reoriginal.BaseStream.Position < reoriginal.BaseStream.Length)
			{
				var b1 = reoriginal.ReadByte();
				var b2 = recompared.ReadByte();
				Assert.AreEqual(b1, b2, "Position: {0}", reoriginal.BaseStream.Position);
			}
		}

		public static void CompareFiles(Stream soriginal, Stream scompared)
		{
			using (var reoriginal = new BinaryReader(soriginal))
			{
				using (var recompared = new BinaryReader(scompared))
				{
					CompareFiles(reoriginal, recompared);
				}
			}
		}

		public static void CompareFiles(string original, string compared)
		{
			using (var fsoriginal = new FileStream(original, FileMode.Open))
			{
				using (var fscompared = new FileStream(compared, FileMode.Open))
				{
					CompareFiles(fsoriginal, fscompared);
				}
			}
		}
	}
}
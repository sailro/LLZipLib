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

		public static BinaryReader LoadInMemory(string filename)
		{
			using (var stream = new BufferedStream(new FileStream(filename, FileMode.Open)))
			{
				var memory = new MemoryStream();
				stream.CopyTo(memory);
				memory.Seek(0, SeekOrigin.Begin);
				return new BinaryReader(memory);
			}
		}

		public static void CompareFiles(string original, string compared)
		{
			CompareFiles(LoadInMemory(original), LoadInMemory(compared));
		}
	}
}

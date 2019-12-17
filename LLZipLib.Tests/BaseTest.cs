using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LLZipLib.Tests
{
	public abstract class BaseTest
	{
		public TestContext TestContext
		{
			get;
			set;
		}

		public string GetTestDirectory()
		{
			var asmPath = GetType().Assembly.Location;
			var basePath = Path.Combine(Path.GetDirectoryName(asmPath), "Files");
			return basePath;
		}
	}
}

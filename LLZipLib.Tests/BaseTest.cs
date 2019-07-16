using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LLZipLib.Tests
{
	public abstract class BaseTest
	{
		private TestContext _testContextInstance;
		protected string _filesDirectory;

		public TestContext TestContext
		{
			get => _testContextInstance;
			set
			{
				_testContextInstance = value;
				_filesDirectory = Path.Combine(_testContextInstance.TestDir, @"..\..");
				_filesDirectory = Path.Combine(_filesDirectory, @"LLZipLib.Tests\Files");
				_filesDirectory = Path.GetFullPath(_filesDirectory);
			}
		}
	}
}

using System.Text;

namespace LLZipLib
{
	internal class DefaultStringConverter : IStringConverter
	{
		private static readonly Encoding Encoding = Encoding.UTF8;

		public string GetString(byte[] buffer, StringConverterContext context)
		{
			return Encoding.GetString(buffer);
		}

		public byte[] GetBytes(string str, StringConverterContext context)
		{
			return Encoding.GetBytes(str);
		}
	}
}

using System.Text;

namespace LLZipLib
{
	public class DefaultStringConverter : IStringConverter
	{
		private static readonly Encoding Encoding = Encoding.UTF8;

		public virtual string GetString(byte[] buffer, StringConverterContext context)
		{
			return Encoding.GetString(buffer);
		}

		public virtual byte[] GetBytes(string str, StringConverterContext context)
		{
			return Encoding.GetBytes(str);
		}
	}
}

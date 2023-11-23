using System.Text;

namespace LLZipLib;

public class DefaultStringConverter : IStringConverter
{
	private static readonly Encoding _encoding = Encoding.UTF8;

	public virtual string GetString(byte[] buffer, StringConverterContext context)
	{
		return _encoding.GetString(buffer);
	}

	public virtual byte[] GetBytes(string str, StringConverterContext context)
	{
		return _encoding.GetBytes(str);
	}
}

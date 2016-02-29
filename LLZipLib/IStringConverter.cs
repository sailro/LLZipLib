namespace LLZipLib
{
	public enum StringConverterContext
	{
		Comment,
		Filename
	}

	public interface IStringConverter
	{
		string GetString(byte[] buffer, StringConverterContext context);
		byte[] GetBytes(string str, StringConverterContext context);
	}
}

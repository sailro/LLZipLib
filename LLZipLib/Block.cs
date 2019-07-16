namespace LLZipLib
{
	public abstract class Block
	{
		internal long Offset { get; set; }
		internal abstract int GetSize();
	}
}

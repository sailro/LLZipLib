namespace Llziplib
{
	public abstract class Descriptor : Block
	{
		public uint Crc { get; set; }
		public int CompressedSize { get; set; }
		public int UncompressedSize { get; set; }
	}
}
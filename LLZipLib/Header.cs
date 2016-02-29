namespace LLZipLib
{
	public abstract class Header : Descriptor
	{
		public uint Signature { get; protected set; }
		public ushort Version { get; set; }
		public ushort Flags { get; set; }
		public ushort Compression { get; set; }
		public ushort Date { get; set; }
		public ushort Time { get; set; }
		public byte[] Filename { get; set; }
		public byte[] Extra { get; set; }
	}
}
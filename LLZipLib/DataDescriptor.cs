using System.IO;

namespace LLZipLib
{
	public class DataDescriptor : Descriptor
	{
		public DataDescriptor(BinaryReader reader)
		{
			Crc = reader.ReadUInt32();
			CompressedSize = reader.ReadInt32();
			UncompressedSize = reader.ReadInt32();
		}

		internal void Write(BinaryWriter writer)
		{
			writer.Write(Crc);
			writer.Write(CompressedSize);
			writer.Write(UncompressedSize);
		}

		internal override int GetSize()
		{
			return 3*sizeof (uint);
		}
	}
}
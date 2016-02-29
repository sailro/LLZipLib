using System.IO;

namespace LLZipLib
{
	public class DataDescriptor : Descriptor
	{
		public DataDescriptor(BinaryReader reader)
		{
			Offset = reader.BaseStream.Position;

			Crc = reader.ReadUInt32();
			CompressedSize = reader.ReadInt32();
			UncompressedSize = reader.ReadInt32();
		}

		internal void Write(BinaryWriter writer)
		{
			Offset = writer.BaseStream.Position;

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
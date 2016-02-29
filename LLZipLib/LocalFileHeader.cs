using System;
using System.IO;

namespace LLZipLib
{
	public class LocalFileHeader : Header
	{
		public LocalFileHeader(BinaryReader reader)
		{
			Offset = reader.BaseStream.Position;

			Signature = reader.ReadUInt32();
			if (Signature != 0x04034B50)
				throw new NotSupportedException("bad signature");

			Version = reader.ReadUInt16();
			Flags = reader.ReadUInt16();
			Compression = reader.ReadUInt16();
			Time = reader.ReadUInt16();
			Date = reader.ReadUInt16();
			Crc = reader.ReadUInt32();
			CompressedSize = reader.ReadInt32();
			UncompressedSize = reader.ReadInt32();
			var filenameLength = reader.ReadUInt16();
			var extraLength = reader.ReadUInt16();
			Filename = reader.ReadBytes(filenameLength);
			Extra = reader.ReadBytes(extraLength);
		}

		internal override int GetSize()
		{
			return 4*sizeof (uint) + 7*sizeof (ushort) + (Filename?.Length ?? 0) + (Extra?.Length ?? 0);
		}

		internal void Write(BinaryWriter writer)
		{
			Offset = writer.BaseStream.Position;

			writer.Write(Signature);
			writer.Write(Version);
			writer.Write(Flags);
			writer.Write(Compression);
			writer.Write(Time);
			writer.Write(Date);
			writer.Write(Crc);
			writer.Write(CompressedSize);
			writer.Write(UncompressedSize);
			writer.Write((ushort) (Filename?.Length ?? 0));
			writer.Write((ushort) (Extra?.Length ?? 0));

			if (Filename != null)
				writer.Write(Filename, 0, Filename.Length);
			if (Extra != null)
				writer.Write(Extra, 0, Extra.Length);
		}
	}
}
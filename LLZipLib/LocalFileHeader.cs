using System;
using System.IO;

namespace LLZipLib
{
	public class LocalFileHeader : Header
	{
		public ZipEntry ZipEntry { get; set; }

		public LocalFileHeader()
		{
			Signature = Signatures.LocalFileHeader;
		}

		public static LocalFileHeader Read(BinaryReader reader)
		{
			var header = new LocalFileHeader
			{
				Offset = reader.BaseStream.Position,
				Signature = reader.ReadUInt32()
			};

			if (header.Signature != Signatures.LocalFileHeader)
				throw new NotSupportedException("bad signature");

			header.Version = reader.ReadUInt16();
			header.Flags = reader.ReadUInt16();
			header.Compression = reader.ReadUInt16();
			header.Time = reader.ReadUInt16();
			header.Date = reader.ReadUInt16();
			header.Crc = reader.ReadUInt32();
			header.CompressedSize = reader.ReadInt32();
			header.UncompressedSize = reader.ReadInt32();

			var filenameLength = reader.ReadUInt16();
			var extraLength = reader.ReadUInt16();
			header.FilenameBuffer = reader.ReadBytes(filenameLength);
			header.Extra = reader.ReadBytes(extraLength);

			return header;
		}

		public string Filename
		{
			get => ZipEntry.ZipArchive.StringConverter.GetString(FilenameBuffer, StringConverterContext.Filename);
			set => FilenameBuffer = ZipEntry.ZipArchive.StringConverter.GetBytes(value, StringConverterContext.Filename);
		}

		internal override int GetSize()
		{
			return 4*sizeof (uint) + 7*sizeof (ushort) + (FilenameBuffer?.Length ?? 0) + (Extra?.Length ?? 0);
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
			writer.Write((ushort) (FilenameBuffer?.Length ?? 0));
			writer.Write((ushort) (Extra?.Length ?? 0));

			if (FilenameBuffer != null)
				writer.Write(FilenameBuffer, 0, FilenameBuffer.Length);
			if (Extra != null)
				writer.Write(Extra, 0, Extra.Length);
		}
	}
}

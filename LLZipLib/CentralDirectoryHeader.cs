using System;
using System.IO;

namespace LLZipLib
{
	public class CentralDirectoryHeader : Header
	{
		public ZipEntry ZipEntry { get; set; }

		public uint LocalHeaderOffset { get; private set; }
		public uint ExternalAttribute { get; set; }
		public ushort InternalAttribute { get; set; }
		public ushort DiskNumber { get; set; }
		public ushort VersionNeeded { get; set; }

		public byte[] CommentBuffer { get; set; } = { };

		public string Comment
		{
			get => ZipEntry.ZipArchive.StringConverter.GetString(CommentBuffer, StringConverterContext.Comment);
			set => CommentBuffer = ZipEntry.ZipArchive.StringConverter.GetBytes(value, StringConverterContext.Comment);
		}

		public string Filename
		{
			get => ZipEntry.ZipArchive.StringConverter.GetString(FilenameBuffer, StringConverterContext.Filename);
			set => FilenameBuffer = ZipEntry.ZipArchive.StringConverter.GetBytes(value, StringConverterContext.Filename);
		}

		public CentralDirectoryHeader()
		{
			Signature = Signatures.CentralDirectoryHeader;
		}

		internal override int GetSize()
		{
			return 6 * sizeof(uint) + 11 * sizeof(ushort) + (FilenameBuffer?.Length ?? 0) + (Extra?.Length ?? 0) + (Comment?.Length ?? 0);
		}

		internal static CentralDirectoryHeader Read(BinaryReader reader)
		{
			var header = new CentralDirectoryHeader {Offset = reader.BaseStream.Position, Signature = reader.ReadUInt32()};

			if (header.Signature != Signatures.CentralDirectoryHeader)
				throw new NotSupportedException("bad signature");

			header.Version = reader.ReadUInt16();
			header.VersionNeeded = reader.ReadUInt16();
			header.Flags = reader.ReadUInt16();
			header.Compression = reader.ReadUInt16();
			header.Time = reader.ReadUInt16();
			header.Date = reader.ReadUInt16();
			header.Crc = reader.ReadUInt32();
			header.CompressedSize = reader.ReadInt32();
			header.UncompressedSize = reader.ReadInt32();
			var filenameLength = reader.ReadUInt16();
			var extraLength = reader.ReadUInt16();
			var commentLength = reader.ReadUInt16();
			header.DiskNumber = reader.ReadUInt16();
			header.InternalAttribute = reader.ReadUInt16();
			header.ExternalAttribute = reader.ReadUInt32();
			header.LocalHeaderOffset = reader.ReadUInt32();
			header.FilenameBuffer = reader.ReadBytes(filenameLength);
			header.Extra = reader.ReadBytes(extraLength);
			header.CommentBuffer = reader.ReadBytes(commentLength);

			return header;
		}

		internal void Write(BinaryWriter writer)
		{
			if (ZipEntry == null)
				throw new InvalidOperationException("this header must be linked to a ZipEntry");

			Offset = writer.BaseStream.Position;

			writer.Write(Signature);
			writer.Write(Version);
			writer.Write(VersionNeeded);
			writer.Write(Flags);
			writer.Write(Compression);
			writer.Write(Time);
			writer.Write(Date);
			writer.Write(Crc);
			writer.Write(CompressedSize);
			writer.Write(UncompressedSize);
			writer.Write((ushort)(FilenameBuffer?.Length ?? 0));
			writer.Write((ushort)(Extra?.Length ?? 0));
			writer.Write((ushort)(Comment?.Length ?? 0));
			writer.Write(DiskNumber);
			writer.Write(InternalAttribute);
			writer.Write(ExternalAttribute);

			//At this time, all local headers are written
			LocalHeaderOffset = (uint)ZipEntry.LocalFileHeader.Offset;
			writer.Write(LocalHeaderOffset);

			if (FilenameBuffer != null)
				writer.Write(FilenameBuffer, 0, FilenameBuffer.Length);
			if (Extra != null)
				writer.Write(Extra, 0, Extra.Length);
			if (Comment != null)
				writer.Write(CommentBuffer, 0, Comment.Length);
		}
	}
}

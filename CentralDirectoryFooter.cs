using System;
using System.IO;
using System.Linq;

namespace Llziplib
{
	public class CentralDirectoryFooter : Block
	{
		public uint Signature { get; }
		public byte[] Comment { get; set; }
		public uint CentralDirectoryOffset { get; internal set; }
		public uint CentralDirectorySize { get; set; }
		public ushort TotalDiskEntries { get; set; }
		public ushort DiskEntries { get; set; }
		public ushort CentralDirectoryDiskNumber { get; set; }
		public ushort DiskNumber { get; set; }

		public CentralDirectoryFooter()
		{
		}

		public CentralDirectoryFooter(BinaryReader reader)
		{
			Offset = reader.BaseStream.Position;

			Signature = reader.ReadUInt32();
			if (Signature != 0x06054B50)
				throw new NotSupportedException("bad signature");

			DiskNumber = reader.ReadUInt16();
			CentralDirectoryDiskNumber = reader.ReadUInt16();
			if (DiskNumber != 0 || CentralDirectoryDiskNumber != 0)
				throw new NotSupportedException("bad disk number");

			DiskEntries = reader.ReadUInt16();
			TotalDiskEntries = reader.ReadUInt16();
			if (DiskEntries != TotalDiskEntries)
				throw new NotSupportedException("multiple volume not supported");

			CentralDirectorySize = reader.ReadUInt32();
			CentralDirectoryOffset = reader.ReadUInt32();
			var commentLength = reader.ReadUInt16();
			Comment = reader.ReadBytes(commentLength);
		}

		internal override int GetSize()
		{
			return 3*sizeof (uint) + 5*sizeof (ushort) + (Comment?.Length ?? 0);
		}

		internal void Write(ZipArchive archive, BinaryWriter writer)
		{
			Offset = writer.BaseStream.Position;

			writer.Write(Signature);
			writer.Write(DiskNumber);
			writer.Write(CentralDirectoryDiskNumber);
			writer.Write(DiskEntries);
			writer.Write(TotalDiskEntries);

			//At this time, everything is written
			var firstEntry = archive.Entries.FirstOrDefault();
			CentralDirectoryOffset = (uint) (firstEntry?.CentralDirectoryHeader.Offset ?? 0);
			CentralDirectorySize = (uint) archive.Entries.Sum(entry => entry.CentralDirectoryHeader.GetSize());

			writer.Write(CentralDirectorySize);
			writer.Write(CentralDirectoryOffset);

			writer.Write((ushort) Comment.Length);
			if (Comment != null)
				writer.Write(Comment, 0, Comment.Length);
		}
	}
}

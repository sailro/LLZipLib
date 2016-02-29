using System;
using System.IO;
using System.Linq;

namespace LLZipLib
{
	public class CentralDirectoryFooter : Block
	{
		public ZipArchive ZipArchive { get; set; }

		public uint Signature { get; private set; }
		public uint CentralDirectoryOffset { get; private set; }
		public uint CentralDirectorySize { get; set; }
		public ushort TotalDiskEntries { get; set; }
		public ushort DiskEntries { get; set; }
		public ushort CentralDirectoryDiskNumber { get; set; }
		public ushort DiskNumber { get; set; }

		public byte[] CommentBuffer { get; set; }
		public string Comment
		{
			get { return ZipArchive.StringConverter.GetString(CommentBuffer, StringConverterContext.Comment); }
			set { CommentBuffer = ZipArchive.StringConverter.GetBytes(value, StringConverterContext.Comment); }
		}

		public CentralDirectoryFooter(ZipArchive archive)
		{
			ZipArchive = archive;
		}

		public CentralDirectoryFooter(ZipArchive archive, BinaryReader reader) : this(archive)
		{
			if (!TrySeekToSignature(reader))
				throw new NotSupportedException("bad signature");

			DiskNumber = reader.ReadUInt16();
			CentralDirectoryDiskNumber = reader.ReadUInt16();
			if (DiskNumber != 0 || CentralDirectoryDiskNumber != 0)
				throw new NotSupportedException("bad disk number");

			DiskEntries = reader.ReadUInt16();
			TotalDiskEntries = reader.ReadUInt16();
			if (DiskEntries != TotalDiskEntries)
				throw new NotSupportedException("multiple volumes are not supported");

			CentralDirectorySize = reader.ReadUInt32();
			CentralDirectoryOffset = reader.ReadUInt32();
			var commentLength = reader.ReadUInt16();
			CommentBuffer = reader.ReadBytes(commentLength);
		}

		private bool TrySeekToSignature(BinaryReader reader)
		{
			var currentPosition = reader.BaseStream.Seek(-GetSize(), SeekOrigin.End);
			const int signatureLength = sizeof(uint);

			while (currentPosition >= 0)
			{
				Signature = reader.ReadUInt32();
				if (Signature == 0x06054B50)
				{
					Offset = currentPosition;
					return true;
				}
				currentPosition = reader.BaseStream.Seek(-signatureLength - 1, SeekOrigin.Current);
			}
			return false;
		}

		internal override int GetSize()
		{
			return 3*sizeof (uint) + 5*sizeof (ushort) + (CommentBuffer?.Length ?? 0);
		}

		internal void Write(BinaryWriter writer)
		{
			Offset = writer.BaseStream.Position;

			writer.Write(Signature);
			writer.Write(DiskNumber);
			writer.Write(CentralDirectoryDiskNumber);

			//At this time, everything is written
			var archive = ZipArchive;

			DiskEntries = TotalDiskEntries = (ushort) archive.Entries.Count;
			writer.Write(DiskEntries);
			writer.Write(TotalDiskEntries);

			var firstEntry = archive.Entries.FirstOrDefault();
			CentralDirectoryOffset = (uint) (firstEntry?.CentralDirectoryHeader.Offset ?? 0);
			CentralDirectorySize = (uint) archive.Entries.Sum(entry => entry.CentralDirectoryHeader.GetSize());

			writer.Write(CentralDirectorySize);
			writer.Write(CentralDirectoryOffset);

			writer.Write((ushort) CommentBuffer.Length);
			if (CommentBuffer != null)
				writer.Write(CommentBuffer, 0, CommentBuffer.Length);
		}
	}
}

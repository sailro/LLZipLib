using System;
using System.IO;
using System.Linq;

namespace LLZipLib
{
	public class CentralDirectoryFooter : Block
	{
		public ZipArchive ZipArchive { get; set; }

		public uint Signature { get; private set; } = Signatures.CentralDirectoryFooter;
		public uint CentralDirectoryOffset { get; private set; }
		public uint CentralDirectorySize { get; set; }
		public ushort TotalDiskEntries { get; set; }
		public ushort DiskEntries { get; set; }
		public ushort CentralDirectoryDiskNumber { get; set; }
		public ushort DiskNumber { get; set; }

		public byte[] CommentBuffer { get; set; } = {};
		public string Comment
		{
			get => ZipArchive.StringConverter.GetString(CommentBuffer, StringConverterContext.Comment);
            set => CommentBuffer = ZipArchive.StringConverter.GetBytes(value, StringConverterContext.Comment);
        }

		private static bool TrySeekToSignature(BinaryReader reader, CentralDirectoryFooter footer)
		{
			var currentPosition = reader.BaseStream.Seek(-footer.GetSize(), SeekOrigin.End);
			const int signatureLength = sizeof(uint);

			while (currentPosition >= 0)
			{
				footer.Signature = reader.ReadUInt32();
				if (footer.Signature == Signatures.CentralDirectoryFooter)
				{
					footer.Offset = currentPosition;
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

		internal static CentralDirectoryFooter Read(BinaryReader reader)
		{
			var footer = new CentralDirectoryFooter();
			if (!TrySeekToSignature(reader, footer))
				throw new NotSupportedException("bad signature");

			footer.DiskNumber = reader.ReadUInt16();
			footer.CentralDirectoryDiskNumber = reader.ReadUInt16();
			if (footer.DiskNumber != 0 || footer.CentralDirectoryDiskNumber != 0)
				throw new NotSupportedException("bad disk number");

			footer.DiskEntries = reader.ReadUInt16();
			footer.TotalDiskEntries = reader.ReadUInt16();
			if (footer.DiskEntries != footer.TotalDiskEntries)
				throw new NotSupportedException("multiple volumes are not supported");

			footer.CentralDirectorySize = reader.ReadUInt32();
			footer.CentralDirectoryOffset = reader.ReadUInt32();
			var commentLength = reader.ReadUInt16();
			footer.CommentBuffer = reader.ReadBytes(commentLength);

			return footer;
		}

		internal void Write(BinaryWriter writer)
		{
			//At this time, everything is written
			var zip = ZipArchive;
			if (zip == null)
				throw new InvalidOperationException("this footer must be linked to a ZipArchive");

			Offset = writer.BaseStream.Position;

			writer.Write(Signature);
			writer.Write(DiskNumber);
			writer.Write(CentralDirectoryDiskNumber);

			DiskEntries = TotalDiskEntries = (ushort) zip.Entries.Count;
			writer.Write(DiskEntries);
			writer.Write(TotalDiskEntries);

			var firstEntry = zip.Entries.FirstOrDefault();
			CentralDirectoryOffset = (uint) (firstEntry?.CentralDirectoryHeader.Offset ?? 0);
			CentralDirectorySize = (uint) zip.Entries.Sum(entry => entry.CentralDirectoryHeader.GetSize());

			writer.Write(CentralDirectorySize);
			writer.Write(CentralDirectoryOffset);

			writer.Write((ushort) CommentBuffer.Length);
			if (CommentBuffer != null)
				writer.Write(CommentBuffer, 0, CommentBuffer.Length);
		}
	}
}

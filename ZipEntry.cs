using System.Diagnostics;
using System.IO;

namespace Llziplib
{
	public class ZipEntry
	{
		public CentralDirectoryHeader CentralDirectoryHeader { get; }
		public LocalFileHeader LocalFileHeader { get; }
		public byte[] Data { get; set; }
		public DataDescriptor DataDescriptor { get; set; }

		public bool HasDataDescriptor => (LocalFileHeader.Flags & 4) != 0;

		public ZipEntry(BinaryReader reader, CentralDirectoryHeader header)
		{
			CentralDirectoryHeader = header;
			reader.BaseStream.Seek(header.LocalHeaderOffset, SeekOrigin.Begin);
			LocalFileHeader = new LocalFileHeader(reader);
			Data = reader.ReadBytes(LocalFileHeader.CompressedSize);

			if (HasDataDescriptor)
				DataDescriptor = new DataDescriptor(reader);
		}

		internal void Write(BinaryWriter writer)
		{
			LocalFileHeader.Write(writer);

			if (Data != null)
				writer.Write(Data, 0, Data.Length);
			if (HasDataDescriptor)
				DataDescriptor?.Write(writer);
		}
	}
}
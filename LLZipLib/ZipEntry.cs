using System.IO;

namespace LLZipLib
{
	public class ZipEntry
	{
		public ZipArchive ZipArchive { get; set; }

		public CentralDirectoryHeader CentralDirectoryHeader { get; }
		public LocalFileHeader LocalFileHeader { get; set;  }
		public DataDescriptor DataDescriptor { get; set; }

		public byte[] _data = {};
		public byte[] Data
		{
			get
			{
				return _data;
			}
			set
			{
				_data = value;
				LocalFileHeader.Crc = 0;
				LocalFileHeader.CompressedSize = CentralDirectoryHeader.CompressedSize =  0;

				if (_data == null)
					return;

				LocalFileHeader.Crc = CentralDirectoryHeader.Crc = Crc32Helper.UpdateCrc32(LocalFileHeader.Crc, _data, 0, _data.Length);
				LocalFileHeader.CompressedSize = CentralDirectoryHeader.CompressedSize = _data.Length;
				LocalFileHeader.UncompressedSize = CentralDirectoryHeader.UncompressedSize = _data.Length;
			}
		}

		public bool HasDataDescriptor => (LocalFileHeader.Flags & 4) != 0;

		public ZipEntry(ZipArchive archive)
		{
			ZipArchive = archive;
			LocalFileHeader = new LocalFileHeader(this);
			CentralDirectoryHeader = new CentralDirectoryHeader(this);
		}

		public ZipEntry(ZipArchive archive, BinaryReader reader, CentralDirectoryHeader header) : this(archive)
		{
			header.ZipEntry = this;
			CentralDirectoryHeader = header;
			reader.BaseStream.Seek(header.LocalHeaderOffset, SeekOrigin.Begin);
			LocalFileHeader = new LocalFileHeader(reader) {ZipEntry = this};

			// do not trigger CRC or CompressedSize recomputation
			_data = reader.ReadBytes(LocalFileHeader.CompressedSize);

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
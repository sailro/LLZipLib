using System.IO;

namespace LLZipLib
{
	public class ZipEntry
	{
		public ZipArchive ZipArchive { get; set; }

		public CentralDirectoryHeader CentralDirectoryHeader { get; set; }
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
				LocalFileHeader.CompressedSize = CentralDirectoryHeader.CompressedSize = 0;

				if (_data == null)
					return;

				LocalFileHeader.Crc =
					CentralDirectoryHeader.Crc = Crc32Helper.UpdateCrc32(LocalFileHeader.Crc, _data, 0, _data.Length);
				LocalFileHeader.CompressedSize = CentralDirectoryHeader.CompressedSize = _data.Length;
				LocalFileHeader.UncompressedSize = CentralDirectoryHeader.UncompressedSize = _data.Length;
			}
		}

		public bool HasDataDescriptor => (LocalFileHeader.Flags & 4) != 0;

		public ZipEntry()
		{
			CentralDirectoryHeader = new CentralDirectoryHeader {ZipEntry = this};
			LocalFileHeader = new LocalFileHeader {ZipEntry = this};
			DataDescriptor = new DataDescriptor();
		}

		public static ZipEntry Read(BinaryReader reader, CentralDirectoryHeader header)
		{
			var entry = new ZipEntry {CentralDirectoryHeader = header};
			header.ZipEntry = entry;

			reader.BaseStream.Seek(header.LocalHeaderOffset, SeekOrigin.Begin);
			entry.LocalFileHeader = LocalFileHeader.Read(reader);
			entry.LocalFileHeader.ZipEntry = entry;

			// do not trigger CRC or [Un]CompressedSize recomputation
			entry._data = reader.ReadBytes(entry.LocalFileHeader.CompressedSize);

			if (entry.HasDataDescriptor)
				entry.DataDescriptor = DataDescriptor.Read(reader);

			return entry;
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
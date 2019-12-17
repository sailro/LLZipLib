using System.IO;

namespace LLZipLib
{
	public class ZipEntry
	{
		public ZipArchive ZipArchive { get; set; }

		public CentralDirectoryHeader CentralDirectoryHeader { get; set; }
		public LocalFileHeader LocalFileHeader { get; set; }
		public DataDescriptor DataDescriptor { get; set; }

		public byte[] _data = { };

		public byte[] Data
		{
			get => _data;
			set
			{
				_data = value;

				LocalFileHeader.Crc = 0;
				LocalFileHeader.CompressedSize = CentralDirectoryHeader.CompressedSize = 0;
				LocalFileHeader.UncompressedSize = CentralDirectoryHeader.UncompressedSize = 0;
				if (HasDataDescriptor)
				{
					DataDescriptor.Crc = 0;
					DataDescriptor.CompressedSize = 0;
					DataDescriptor.UncompressedSize = 0;
				}

				if (_data == null)
					return;

				var crc = Crc32Helper.UpdateCrc32(LocalFileHeader.Crc, _data, 0, _data.Length);
				var length = _data.Length;
				if (HasDataDescriptor)
				{
					DataDescriptor.Crc = crc;
					DataDescriptor.CompressedSize = _data.Length;
					DataDescriptor.UncompressedSize = _data.Length;
				}
				else
				{
					LocalFileHeader.Crc = crc;
					LocalFileHeader.CompressedSize = length;
					LocalFileHeader.UncompressedSize = length;
				}

				CentralDirectoryHeader.Crc = crc;
				CentralDirectoryHeader.CompressedSize = length;
				CentralDirectoryHeader.UncompressedSize = length;
			}
		}

		public bool HasDataDescriptor
		{
			get => (LocalFileHeader.Flags & 8) != 0;
			set
			{
				if (value)
					LocalFileHeader.Flags |= 8;
				else
					LocalFileHeader.Flags = (ushort)(LocalFileHeader.Flags & ~8);
			}
		}

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
			entry._data = reader.ReadBytes(header.CompressedSize);

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

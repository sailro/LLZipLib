using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LLZipLib
{
	public class ZipArchive : Block
	{
		public IStringConverter StringConverter => new DefaultStringConverter();
		public CentralDirectoryFooter CentralDirectoryFooter { get; private set; }
		public ZipEntryCollection Entries { get; }

		public ZipArchive()
		{
			CentralDirectoryFooter = new CentralDirectoryFooter {ZipArchive = this};
			Entries = new ZipEntryCollection(this);
		}

		public static ZipArchive Read(string filename)
		{
			using (var stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
				return Read(stream);
		}

		public static ZipArchive Read(Stream stream)
		{
			using (var reader = new BinaryReader(stream, Encoding.ASCII))
				return Read(reader);
		}

		public static ZipArchive Read(BinaryReader reader)
		{
			var zip = new ZipArchive
			{
				Offset = reader.BaseStream.Position,
				CentralDirectoryFooter = CentralDirectoryFooter.Read(reader)
			};

			zip.CentralDirectoryFooter.ZipArchive = zip;

			var headers = new List<CentralDirectoryHeader>();
			reader.BaseStream.Seek(zip.CentralDirectoryFooter.CentralDirectoryOffset, SeekOrigin.Begin);

			for (var i = 0; i < zip.CentralDirectoryFooter.DiskEntries; i++)
				headers.Add(CentralDirectoryHeader.Read(reader));

			foreach (var header in headers)
				zip.Entries.Add(ZipEntry.Read(reader, header));

			return zip;
		}

		public void Write(string filename)
		{
			using (var stream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.Read))
				Write(stream);
		}

		public void Write(Stream stream)
		{
			using (var writer = new BinaryWriter(stream, Encoding.UTF8))
				Write(writer);
		}

		public void Write(BinaryWriter writer)
		{
			Offset = writer.BaseStream.Position;

			foreach (var entry in Entries)
				entry.Write(writer);

			foreach (var entry in Entries)
				entry.CentralDirectoryHeader.Write(writer);

			CentralDirectoryFooter.Write(writer);

			if (writer.BaseStream.Position - Offset != GetSize())
				throw new NotSupportedException("size mismatch");
		}

		internal override int GetSize()
		{
			return CentralDirectoryFooter.GetSize() +
			       Entries.Sum(entry =>
				       entry.CentralDirectoryHeader.GetSize()
				       + entry.LocalFileHeader.GetSize()
				       + (entry.Data?.Length ?? 0)
				       + (entry.HasDataDescriptor ? entry.DataDescriptor.GetSize() : 0));
		}
	}
}
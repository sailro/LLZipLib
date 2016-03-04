using System.Collections.ObjectModel;

namespace LLZipLib
{
	public class ZipEntryCollection : Collection<ZipEntry>
	{
		private ZipArchive ZipArchive { get; }
		public ZipEntryCollection(ZipArchive zipArchive)
		{
			ZipArchive = zipArchive;
		}

		private void TagZipEntry(ZipEntry zipEntry)
		{
			if (zipEntry != null)
				zipEntry.ZipArchive = ZipArchive;
		}

		protected override void InsertItem(int index, ZipEntry zipEntry)
		{
			TagZipEntry(zipEntry);
			base.InsertItem(index, zipEntry);
		}

		protected override void SetItem(int index, ZipEntry zipEntry)
		{
			TagZipEntry(zipEntry);
			base.SetItem(index, zipEntry);
		}
	}
}

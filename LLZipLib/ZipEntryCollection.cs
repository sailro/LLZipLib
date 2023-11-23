using System.Collections.ObjectModel;

namespace LLZipLib;

public class ZipEntryCollection(ZipArchive zip) : Collection<ZipEntry>
{
	private ZipArchive ZipArchive { get; } = zip;

	private void TagZipEntry(ZipEntry zipEntry)
	{
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

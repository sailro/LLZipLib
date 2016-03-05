# LLZipLib
Low level Zip Library, allowing advanced tweaks (injecting/removing extra blocks, altering flags, crafting special archives). Self contained, no third party dependencies. If you just want to unzip files, this is not for you :)

.ZIP File Format Specification:
https://pkware.cachefly.net/webdocs/casestudies/APPNOTE.TXT

How to use it:

## Changing file content
```csharp
var zip = ZipArchive.Read(filename);

var entry = zip.Entries.FirstOrDefault(e => e.LocalFileHeader.Filename == "readme.txt");
if (entry != null)
{
	entry.Data = zip.StringConverter.GetBytes("This is my new content", StringConverterContext.Content);
	// this is not compressed
	entry.LocalFileHeader.Compression = entry.CentralDirectoryHeader.Compression = 0;
}

zip.Write(filename);
```

## Processing file names
```csharp
var zip = ZipArchive.Read(filename);

foreach (var entry in zip.Entries)
{
	entry.LocalFileHeader.Filename = "foo." + entry.LocalFileHeader.Filename;
	entry.CentralDirectoryHeader.Filename = entry.LocalFileHeader.Filename;
}

zip.Write(filename);
```

## Creating archive / adding entry
```csharp
var zip = new ZipArchive();
var entry = new ZipEntry();

zip.Entries.Add(entry);
entry.LocalFileHeader.Filename = entry.CentralDirectoryHeader.Filename = "foo.txt";
entry.Data = zip.StringConverter.GetBytes("Hello world!", StringConverterContext.Content);

zip.Write("foo.zip");
```

## Removing all data descriptors
```csharp
var zip = ZipArchive.Read(filename);

foreach (var entry in zip.Entries.Where(entry => entry.HasDataDescriptor))
{
	entry.LocalFileHeader.CompressedSize = entry.DataDescriptor.CompressedSize;
	entry.LocalFileHeader.UncompressedSize = entry.DataDescriptor.UncompressedSize;
	entry.LocalFileHeader.Crc = entry.DataDescriptor.Crc;
	entry.HasDataDescriptor = false; 
}

zip.Write(filename);
```

## Removing all extra blocks
```csharp
var zip = ZipArchive.Read(filename);

foreach (var entry in zip.Entries)
{
	entry.LocalFileHeader.Extra = new byte[0];
	entry.CentralDirectoryHeader.Extra = new byte[0];
}

zip.Write(filename);
```

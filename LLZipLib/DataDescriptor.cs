using System;
using System.IO;

namespace LLZipLib
{
	public class DataDescriptor : Descriptor
	{
		public uint Signature { get; protected set; }
		public bool UseOptionalSignature { get; set; }

		public DataDescriptor()
		{
			Signature = Signatures.DataDescriptor;
			UseOptionalSignature = true;
		}

		internal static DataDescriptor Read(BinaryReader reader)
		{
			var descriptor = new DataDescriptor {Offset = reader.BaseStream.Position};

			// Read Optionnal Signature
			var signature = reader.ReadUInt32();
			if (signature == Signatures.DataDescriptor)
			{
				descriptor.UseOptionalSignature = true;
			}
			else
			{
				descriptor.UseOptionalSignature = false;
				reader.BaseStream.Position = descriptor.Offset;
			}

			descriptor.Crc = reader.ReadUInt32();
			descriptor.CompressedSize = reader.ReadInt32();
			descriptor.UncompressedSize = reader.ReadInt32();

			return descriptor;
		}

		internal void Write(BinaryWriter writer)
		{
			Offset = writer.BaseStream.Position;

			if (UseOptionalSignature)
				writer.Write(Signature);

			writer.Write(Crc);
			writer.Write(CompressedSize);
			writer.Write(UncompressedSize);
		}

		internal override int GetSize()
		{
			return (3 + Convert.ToInt32(UseOptionalSignature)) * sizeof(uint);
		}
	}
}

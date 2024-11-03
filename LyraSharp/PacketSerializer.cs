using System;
using System.Runtime.InteropServices;

namespace LyraSharp
{
    readonly struct PacketSerializer
    {
        readonly int headerBitCount;
        readonly int quantizedBitCount;

        public PacketSerializer(int headerBitCount, int quantizedBitCount)
        {
            this.headerBitCount = headerBitCount;
            this.quantizedBitCount = quantizedBitCount;
            var totalPacketBitCount = headerBitCount + quantizedBitCount;
            if (totalPacketBitCount > LyraConfig.MaxPacketBitCount)
            {
                throw new ArgumentException($"The sum of header bits ({headerBitCount}) and quantized bits ({quantizedBitCount}) has to be lower than the maximum packet bits ({LyraConfig.MaxPacketBitCount})");
            }
        }

        readonly int PacketSize => (int)MathF.Ceiling((float)(quantizedBitCount + headerBitCount) / (sizeof(byte) * 8));

        public readonly BitSet256 Pack(BitSet256 quantizedFeatures)
        {
            var totalPacketBitCount = headerBitCount + quantizedBitCount;

            var totalPacketByteCount = (totalPacketBitCount + 7) >> 3;

            var packetBits = GeneratePacketBits(quantizedFeatures);

            var packetBytes = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref packetBits, 1))[..totalPacketByteCount];

            packetBytes.Reverse();

            return packetBits;
        }

        public unsafe readonly BitSet256 Unpack(ReadOnlySpan<byte> packet)
        {
            if (packet.Length != PacketSize)
            {
                throw new ArgumentException($"Packet of unexpected length: {packet.Length}", nameof(packet));
            }
            return UnpackFeatures(packet);
        }

        private unsafe readonly BitSet256 UnpackFeatures(ReadOnlySpan<byte> encoded)
        {
            BitSet256 bitArray;
            var features = new Span<byte>(&bitArray, sizeof(BitSet256));
            encoded.CopyTo(features);
            features.Reverse();
            return bitArray;
        }

        private readonly BitSet256 GeneratePacketBits(in BitSet256 quantizedFeatures)
        {
            var packetBits = SetHeader();

            packetBits |= quantizedFeatures;
            return packetBits;
        }

        private readonly BitSet256 SetHeader()
        {
            var header = new BitSet256();
            // Must update if adding new header sections.
            var unusedBits = headerBitCount;
            // For each entry in the header, subtract the number of bits from the
            // unused_bits, get the bits, or it with header and the shift by the number
            // of bits of the next entry.
            header <<= (unusedBits + quantizedBitCount);
            return header;
        }
    }
}

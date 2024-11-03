using System;

namespace LyraSharp
{
    public struct LyraDecoder : IDisposable
    {
        ResidualVectorQuantizationDecoder vectorQuantizer;
        LyraGanModel generativeModel;


        public static LyraDecoder Create()
        {
            return new()
            {
                vectorQuantizer = ResidualVectorQuantizationDecoder.Create(),
                generativeModel = LyraGanModel.Create(),
            };
        }
        public readonly ReadOnlySpan<float> Decode(ReadOnlySpan<byte> encoded)
        {
            var quantizedBitCount = LyraConfig.PacketSizeToQuantizedBitCount(encoded.Length);
            if (quantizedBitCount < 0)
            {
                throw new ArgumentException($"The packet size ({encoded.Length} bytes) is not supported.", nameof(encoded));
            }
            var features = vectorQuantizer.DecodeToLossyFeatures(encoded);
            //var packet = new PacketSerializer(LyraConfig.HeaderBitCount, quantizedBitCount);
            //var unpacked = packet.Unpack(encoded);
            //vectorQuantizer.DecodeToLossyFeatures(unpacked, quantizedBitCount);
            return generativeModel.Decode(features);
        }

        public void Dispose()
        {
            vectorQuantizer.Dispose();
            generativeModel.Dispose();
        }
    }
}
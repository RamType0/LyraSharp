using System;

namespace LyraSharp
{
    public struct LyraEncoder : IDisposable
    {
        SoundStreamEncoder featureExtractor;
        ResidualVectorQuantizationEncoder vectorQuantizer;

        int quantizedBitCount;

        public int FrameRate => LyraConfig.FrameRate;
        public LyraEncoder(int bitrate = 9200)
        {
            var quantizedBitCount = LyraConfig.BitrateToQuantizedBitCount(bitrate);
            if (quantizedBitCount < 0)
            {
                throw new ArgumentException($"Bitrate {bitrate} bps is not supported by codec.", nameof(bitrate));
            }
            this.quantizedBitCount = quantizedBitCount;

            featureExtractor = SoundStreamEncoder.Create();
            vectorQuantizer = ResidualVectorQuantizationEncoder.Create();
        }

        public int Bitrate
        {
            readonly get => LyraConfig.GetBitrate(quantizedBitCount);
            set
            {
                var quantizedBitCount = LyraConfig.BitrateToQuantizedBitCount(value);
                if (quantizedBitCount < 0)
                {
                    throw new ArgumentException($"Bitrate {value} bps is not supported by codec.", nameof(value));
                }
                this.quantizedBitCount = quantizedBitCount;
            }
        }
        public readonly BitSet256 Encode(ReadOnlySpan<float> audio)
        {
            var audioForEncoding = audio;

            if (audioForEncoding.Length != LyraConfig.GetSampleCountPerHop(LyraConfig.SampleRate))
            {
                throw new ArgumentException($"The number of audio samples has to be exactly {LyraConfig.GetSampleCountPerHop(LyraConfig.SampleRate)}, but is {audio.Length}.", nameof(audio));
            }

            var features = featureExtractor.Extract(audioForEncoding);
            var quantizedFeatures = vectorQuantizer.Quantize(features, quantizedBitCount);
            return quantizedFeatures;
            //var packet = new PacketSerializer(LyraConfig.HeaderBitCount, quantizedBitCount);
            //return packet.Pack(quantizedFeatures);
        }

        public void Dispose()
        {
            featureExtractor.Dispose();
            vectorQuantizer.Dispose();
        }
    }
}

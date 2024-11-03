using System;

namespace LyraSharp
{
    static class LyraConfig
    {
        public static Version Version { get; } = new Version(MajorVersion, MinorVersion, MicroVersion);
        const int MajorVersion = 1;
        const int MinorVersion = 3;
        const int MicroVersion = 2;
        public const int FeatureCount = 64;
        public const int ChannelCount = 1;
        public const int OverlapFactor = 2;
        public const int HeaderBitCount = 0;
        public const int FrameRate = 50;
        // TODO: Implement resampler

        //static ReadOnlySpan<int> SupportedSampleRates => new int[] { 8000, 16000, 32000, 48000 };
        static readonly int[] supportedSampleRates = new int[] { SampleRate };
        static ReadOnlySpan<int> SupportedSampleRates => supportedSampleRates;
        public const int SampleRate = 16000;
        const int BitSizeOfByte = sizeof(byte) * 8;

        public const int MaxPacketBitCount = 184;

        public static int GetSampleCountPerHop(int sampleRate)
        {
            return sampleRate / FrameRate;
        }

        public static int GetSampleCountPerWindow(int sampleRate)
        {
            return OverlapFactor * GetSampleCountPerHop(sampleRate);
        }

        public static int GetPacketSize(int quantizedBitCount)
        {
            return (int)MathF.Ceiling((float)(quantizedBitCount + HeaderBitCount) / BitSizeOfByte);
        }

        public static int BitrateToPacketSize(int bitrate)
        {
            return (int)MathF.Ceiling((float)bitrate / FrameRate * BitSizeOfByte);
        }

        public static int GetBitrate(int quantizedBitCount)
        {
            return GetPacketSize(quantizedBitCount) * BitSizeOfByte * FrameRate;
        }
        public static bool IsSampleRateSupported(int sampleRate)
        {
            return SupportedSampleRates.IndexOf(sampleRate) != -1;
        }

        public static int PacketSizeToQuantizedBitCount(int packetSize)
        {
            for (int quantizedBitCount = 8; quantizedBitCount <= 184; quantizedBitCount += 8)
            {
                if (packetSize == GetPacketSize(quantizedBitCount))
                {
                    return quantizedBitCount;
                }
            }
            return -1;
        }

        public static int BitrateToQuantizedBitCount(int bitrate)
        {
            for (int quantizedBitCount = 8; quantizedBitCount <= 184; quantizedBitCount += 8)
            {
                if (bitrate == GetBitrate(quantizedBitCount))
                {
                    return quantizedBitCount;
                }
            }
            return -1;
        }
        public static void AssertParamsSupported(int sampleRate, int channelCount)
        {
            if (!IsSampleRateSupported(sampleRate))
            {
                throw new ArgumentException($"Sample rate {sampleRate} Hz is not supported by codec.", nameof(sampleRate));
            }
            if (channelCount != ChannelCount)
            {
                throw new ArgumentException($"Number of channels {channelCount} is not supported by codec. It needs to be {ChannelCount}.", nameof(channelCount));
            }
        }
    }



}

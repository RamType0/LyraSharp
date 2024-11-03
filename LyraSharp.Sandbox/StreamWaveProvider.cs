using NAudio.Wave;

namespace LyraSharp.Sandbox
{
    internal class StreamWaveProvider : IWaveProvider
    {
        public StreamWaveProvider(Stream stream, WaveFormat waveFormat)
        {
            this.stream = stream;
            WaveFormat = waveFormat;
        }
        Stream stream;
        public WaveFormat WaveFormat { get; }

        public int Read(byte[] buffer, int offset, int count)
        {
            return stream.Read(buffer, offset, count);
        }
    }
}

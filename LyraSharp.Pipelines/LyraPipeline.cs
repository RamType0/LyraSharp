using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace LyraSharp.Pipelines
{
    public class LyraPipeline
    {
        public static PipeReader CreateEncoder(PipeReader input, int bitrate = 9200)
        {
            var pipe = new Pipe();
            var output = pipe.Writer;
            Task.Run(async () =>
            {
                Exception? exception = null;
                try
                {
                    using var encoder = new LyraEncoder(bitrate);
                    var sampleRate = LyraConfig.SampleRate;

                    var sampleCountPerHop = LyraConfig.GetSampleCountPerHop(sampleRate);
                    var hopSize = sampleCountPerHop * sizeof(float);

                    var packetSize = LyraConfig.GetPacketSize(LyraConfig.BitrateToQuantizedBitCount(bitrate));

                    while (true)
                    {
                        var readResult = await input.ReadAtLeastAsync(hopSize);

                        var buffer = readResult.Buffer;
                        var tmpBuffer = buffer.IsSingleSegment ? null : ArrayPool<float>.Shared.Rent(hopSize);
                        while (buffer.Length >= hopSize)
                        {
                            var hopBuffer = buffer.Slice(0, hopSize);
                            buffer = buffer.Slice(hopBuffer.End);

                            BitSet256 packet;
                            if (hopBuffer.IsSingleSegment)
                            {
                                packet = encoder.Encode(MemoryMarshal.Cast<byte, float>(hopBuffer.FirstSpan[..hopSize]));
                            }
                            else
                            {
                                hopBuffer.CopyTo(MemoryMarshal.AsBytes(tmpBuffer.AsSpan()));
                                packet = encoder.Encode(tmpBuffer.AsSpan(0, sampleCountPerHop));
                            }
                            output.Write(MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<BitSet256, byte>(ref packet), packetSize));
                        }
                        if (tmpBuffer != null)
                        {
                            ArrayPool<float>.Shared.Return(tmpBuffer);
                        }
                        await output.FlushAsync();
                        if (readResult.IsCompleted)
                        {
                            break;
                        }
                        input.AdvanceTo(buffer.Start);
                    }
                }
                catch (Exception e)
                {
                    exception = e;
                }
                finally
                {
                    var inputComplete = input.CompleteAsync(exception);
                    var outputComplete = output.CompleteAsync(exception);
                    await inputComplete;
                    await outputComplete;
                }
            });
            return pipe.Reader;
        }
        public static PipeReader CreateDecoder(PipeReader input, int bitrate = 9200)
        {
            var pipe = new Pipe();
            var output = pipe.Writer;
            Task.Run(async () =>
            {
                Exception? exception = null;
                try
                {
                    using var decoder = LyraDecoder.Create();
                    var sampleRate = LyraConfig.SampleRate;

                    var sampleCountPerHop = LyraConfig.GetSampleCountPerHop(sampleRate);
                    var hopSize = sampleCountPerHop * sizeof(float);

                    var packetSize = LyraConfig.GetPacketSize(LyraConfig.BitrateToQuantizedBitCount(bitrate));

                    while (true)
                    {
                        var readResult = await input.ReadAtLeastAsync(packetSize);

                        var buffer = readResult.Buffer;
                        var tmpBuffer = buffer.IsSingleSegment ? null : ArrayPool<byte>.Shared.Rent(packetSize);
                        while (buffer.Length >= packetSize)
                        {
                            var packetBuffer = buffer.Slice(0, packetSize);
                            buffer = buffer.Slice(packetBuffer.End);

                            if (packetBuffer.IsSingleSegment)
                            {
                                output.Write(MemoryMarshal.Cast<float, byte>(decoder.Decode(packetBuffer.FirstSpan[..packetSize])));
                            }
                            else
                            {
                                packetBuffer.CopyTo(tmpBuffer.AsSpan());
                                output.Write(MemoryMarshal.Cast<float, byte>(decoder.Decode(tmpBuffer.AsSpan(0, packetSize))));
                            }
                        }
                        if (tmpBuffer != null)
                        {
                            ArrayPool<byte>.Shared.Return(tmpBuffer);
                        }
                        await output.FlushAsync();
                        if (readResult.IsCompleted)
                        {
                            break;
                        }
                        input.AdvanceTo(buffer.Start);
                    }
                }
                catch (Exception e)
                {
                    exception = e;
                }
                finally
                {
                    var inputComplete = input.CompleteAsync(exception);
                    var outputComplete = output.CompleteAsync(exception);
                    await inputComplete;
                    await outputComplete;
                }
            });
            return pipe.Reader;
        }

    }
}

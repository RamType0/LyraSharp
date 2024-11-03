using System;
using System.Runtime.InteropServices;
using TensorFlowLite;
using static LyraSharp.ResidualVectorQuantizer;

namespace LyraSharp
{
    struct ResidualVectorQuantizationEncoder : IDisposable
    {
        Interpreter interpreter;
        SignatureRunner signatureRunner;

        public static ResidualVectorQuantizationEncoder Create()
        {
            var interpreter = new Interpreter(model, null);
            var signatureRunner = interpreter.GetSignatureRunner("encode");
            signatureRunner.AllocateTensors().ThrowExceptionForStatus();
            return new()
            {
                interpreter = interpreter,
                signatureRunner = signatureRunner,
            };
        }

        public readonly BitSet256 Quantize(ReadOnlySpan<float> features, int quantizedBitCount)
        {
            if (MaxQuantizedBitCount < quantizedBitCount)
            {
                throw new ArgumentOutOfRangeException($"The number of bits cannot exceed maximum ({MaxQuantizedBitCount}).");
            }

            if (quantizedBitCount % BitCountPerQuantizer != 0)
            {
                throw new ArgumentException($"The number of bits ({quantizedBitCount}) has to be divisible by the number of bits per quantizer ({BitCountPerQuantizer}).", nameof(quantizedBitCount));
            }

            var requiredQuantizerCount = quantizedBitCount / BitCountPerQuantizer;
            signatureRunner.GetInputTensor("num_quantizers").CopyFromBuffer(MemoryMarshal.CreateReadOnlySpan(ref requiredQuantizerCount, 1)).ThrowExceptionForStatus();

            signatureRunner.GetInputTensor("input_frames").CopyFromBuffer(features).ThrowExceptionForStatus();

            signatureRunner.Invoke().ThrowExceptionForStatus();

            var nearestNeighbors = signatureRunner.GetOutputTensor("output_0").GetData<int>()[..requiredQuantizerCount];

            var quantizedBits = new BitSet256();
            for (int i = 0; i < nearestNeighbors.Length; i++)
            {
                var quantizedBitsSection = new BitSet256(nearestNeighbors[i]);
                quantizedBits |= quantizedBitsSection << (requiredQuantizerCount - i - 1) * BitCountPerQuantizer;
            }
            return quantizedBits;
        }
        public void Dispose()
        {
            signatureRunner.Dispose();
            interpreter.Dispose();
        }
    }
}

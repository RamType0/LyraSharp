using System;
using TensorFlowLite;
using static LyraSharp.ResidualVectorQuantizer;

namespace LyraSharp
{
    struct ResidualVectorQuantizationDecoder : IDisposable
    {
        Interpreter interpreter;
        SignatureRunner signatureRunner;
        public static ResidualVectorQuantizationDecoder Create()
        {
            var interpreter = new Interpreter(model, null);
            var signatureRunner = interpreter.GetSignatureRunner("decode");
            signatureRunner.AllocateTensors().ThrowExceptionForStatus();
            return new()
            {
                interpreter = interpreter,
                signatureRunner = signatureRunner,
            };
        }
        public readonly unsafe ReadOnlySpan<float> DecodeToLossyFeatures(ReadOnlySpan<byte> quantizedFeatures)
        {
            BitSet256 bitArray;
            quantizedFeatures.CopyTo(new Span<byte>(&bitArray, sizeof(BitSet256)));
            return DecodeToLossyFeatures(bitArray, quantizedFeatures.Length * sizeof(byte) * 8);
        }

        public readonly ReadOnlySpan<float> DecodeToLossyFeatures(in BitSet256 quantizedFeatures, int quantizedBitCount)
        {
            if (quantizedBitCount > MaxQuantizedBitCount)
            {
                throw new ArgumentException($"The number of bits cannot exceed maximum ({MaxQuantizedBitCount}).", nameof(quantizedBitCount));
            }
            if (quantizedBitCount % BitCountPerQuantizer != 0)
            {
                throw new ArgumentException($"The number of bits ({quantizedBitCount}) has to be divisible by the number of bits per quantizer ({BitCountPerQuantizer}).", nameof(quantizedBitCount));
            }

            var requiredQuantizerCount = quantizedBitCount / BitCountPerQuantizer;

            var quantizerMask = new BitSet256((1 << BitCountPerQuantizer) - 1);
            var indices = signatureRunner.GetInputTensor("encoding_indices").GetData<int>();
            var usedIndices = indices[..requiredQuantizerCount];
            for (int i = 0; i < usedIndices.Length; i++)
            {
                usedIndices[i] =
                    ((quantizedFeatures >>
                    ((requiredQuantizerCount - i - 1) * BitCountPerQuantizer)) &
                    quantizerMask).ToInt32();
            }
            indices[usedIndices.Length..].Fill(-1);

            signatureRunner.Invoke().ThrowExceptionForStatus();
            return signatureRunner.GetOutputTensor("output_0").GetData<float>();
        }

        public void Dispose()
        {
            signatureRunner.Dispose();
            interpreter.Dispose();
        }
    }
}

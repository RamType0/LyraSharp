using TensorFlowLite;

namespace LyraSharp
{
    static class ResidualVectorQuantizer
    {
        public static readonly Model model = ModelLoader.Load("quantizer.tflite");
        public const int MaxQuantizedBitCount = 184;
        public const int BitCountPerQuantizer = 4;
        public const int MaxQuantizerCount = MaxQuantizedBitCount / BitCountPerQuantizer;
    }
}

using System;
using TensorFlowLite;

namespace LyraSharp
{
    struct SoundStreamEncoder : IDisposable
    {
        static readonly Model model;

        Interpreter interpreter;
        TensorFlowLiteDelegate tensorFlowLiteDelegate;
        static SoundStreamEncoder()
        {
            model = ModelLoader.Load("soundstream_encoder.tflite");
        }

        public static SoundStreamEncoder Create()
        {
            var xnnPackDelegate = LyraDelegate.Create();
            using var options = InterpreterOptions.Create();
            options.AddDelegate(xnnPackDelegate);
            var interpreter = new Interpreter(model, options);
            interpreter.AllocateTensors().ThrowExceptionForStatus(); 
            return new()
            {
                interpreter = interpreter,
                tensorFlowLiteDelegate = xnnPackDelegate,
            };
        }

        public readonly ReadOnlySpan<float> Extract(ReadOnlySpan<float> audio)
        {
            interpreter.GetInputTensor(0).CopyFromBuffer(audio).ThrowExceptionForStatus();
            interpreter.Invoke().ThrowExceptionForStatus();
            var data = interpreter.GetOutputTensor(0).GetData<float>();
            return data;
        }
        public void Dispose()
        {
            interpreter.Dispose();
            tensorFlowLiteDelegate.Dispose();
        }
    }
}

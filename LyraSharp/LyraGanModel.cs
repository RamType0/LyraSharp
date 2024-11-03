using System;
using TensorFlowLite;

namespace LyraSharp
{
    struct LyraGanModel : IDisposable
    {
        static readonly Model model;
        Interpreter interpreter;
        TensorFlowLiteDelegate tensorFlowLiteDelegate;

        static LyraGanModel()
        {
            model = ModelLoader.Load("lyragan.tflite");
        }
        public static LyraGanModel Create()
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

        public readonly ReadOnlySpan<float> Decode(ReadOnlySpan<float> features)
        {
            interpreter.GetInputTensor(0).CopyFromBuffer(features).ThrowExceptionForStatus();
            interpreter.Invoke().ThrowExceptionForStatus();
            return interpreter.GetOutputTensor(0).GetData<float>();
        }

        public void Dispose()
        {
            interpreter.Dispose();
            tensorFlowLiteDelegate.Dispose();
        }
    }
}
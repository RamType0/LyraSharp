using TensorFlowLite;
using TensorFlowLite.Delegates;

namespace LyraSharp
{
    static class LyraDelegate
    {
        public static TensorFlowLiteDelegate Create() 
        { 
            var xnnpackDelegateOptions = XNNPackDelegate.Options.Default;
            xnnpackDelegateOptions.Flags |= XNNPackDelegate.Flags.Qu8;
            xnnpackDelegateOptions.ThreadCount = 1;
            var xnnpackDelegate = XNNPackDelegate.Create(xnnpackDelegateOptions);
            xnnpackDelegate.Flags |= TensorFlowLite.Native.TfLiteDelegateFlags.AllowDynamicTensors;
            return xnnpackDelegate;
        }
    }
}
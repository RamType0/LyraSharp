using System.IO;
using TensorFlowLite;
namespace LyraSharp
{
    static class ModelLoader
    {
        static byte[] LoadBinary(string name)
        {
            using var resourceStream = typeof(ModelLoader).Assembly.GetManifestResourceStream(typeof(ModelLoader), "Models." + name);
            using var memoryStream = new MemoryStream();
            resourceStream.CopyTo(memoryStream);
            return memoryStream.ToArray();
        }
        public static Model Load(string name)
        {
            return new Model(LoadBinary(name));
        }
    }
}

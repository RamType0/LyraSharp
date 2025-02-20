using LyraSharp;
using LyraSharp.Pipelines;
using LyraSharp.Sandbox;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using System.IO.Pipelines;


static MMDevice SelectDevice(DataFlow dataFlow, DeviceState deviceState)
{
    using MMDeviceEnumerator deviceEnumerator = new();
    var audioDevices = deviceEnumerator.EnumerateAudioEndPoints(dataFlow, deviceState);

    for (int i = 0; i < audioDevices.Count; i++)
    {
        var audioDevice = audioDevices[i];
        Console.WriteLine($"{i}:{audioDevice.FriendlyName}");
    }

    while (true)
    {
        Console.WriteLine("Please enter device index.");

        var line = Console.ReadLine();

        if (int.TryParse(line, out var selectedDeviceIndex) && 0 <= selectedDeviceIndex && selectedDeviceIndex < audioDevices.Count)
        {
            var selectedDevice = audioDevices[selectedDeviceIndex];

            return selectedDevice;
        }
    }
}

Console.WriteLine("Please select input device.");
using var selectedInputDevice = SelectDevice(DataFlow.Capture, DeviceState.Active);

Console.WriteLine($"Input Device: {selectedInputDevice.FriendlyName}");
Console.WriteLine();

Console.WriteLine("Please select output device.");
using var selectedOutputDevice = SelectDevice(DataFlow.Render, DeviceState.Active);

Console.WriteLine($"Output Device: {selectedOutputDevice.FriendlyName}");
Console.WriteLine();


int bitRate;
while (true)
{

    Console.WriteLine("Please enter bit rate.");
    if (int.TryParse(Console.ReadLine(), out bitRate)) 
    {
        break; 
    }
}

using var wasapiCapture = new WasapiCapture(selectedInputDevice,true, 20);
var waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(16000, 1);
wasapiCapture.WaveFormat = waveFormat;
var audioInputPipe = new Pipe();
var audioInputStream = audioInputPipe.Writer.AsStream();
wasapiCapture.DataAvailable += WasapiCapture_DataAvailable;
void WasapiCapture_DataAvailable(object? sender, WaveInEventArgs e)
{
    var wave = e.Buffer.AsSpan(0, e.BytesRecorded);
    audioInputStream.Write(e.Buffer, 0, e.BytesRecorded);
}


var encodedPacketReader = LyraPipeline.CreateEncoder(audioInputPipe.Reader, bitRate);

var decodedAudioReader = LyraPipeline.CreateDecoder(encodedPacketReader, bitRate);



using var wasapiOut = new WasapiOut(selectedOutputDevice, AudioClientShareMode.Shared, true, 20);

var waveProvider = new StreamWaveProvider(decodedAudioReader.AsStream(), waveFormat);

wasapiOut.Init(waveProvider);

wasapiCapture.StartRecording();
wasapiOut.Play();

Console.WriteLine("Press q to exit.");
while (true)
{
    if (Console.ReadKey().KeyChar == 'q')
    {
        Environment.Exit(0);
    }

}

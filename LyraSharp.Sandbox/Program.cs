using LyraSharp;
using LyraSharp.Pipelines;
using LyraSharp.Sandbox;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using System.IO.Pipelines;

var wasapiCapture = new WasapiCapture(WasapiCapture.GetDefaultCaptureDevice(),true, 20);
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

const int Bitrate = 9200;

var encodedPacketReader = LyraPipeline.CreateEncoder(audioInputPipe.Reader, Bitrate);

var decodedAudioReader = LyraPipeline.CreateDecoder(encodedPacketReader, Bitrate);

var wasapiOut = new WasapiOut(AudioClientShareMode.Shared, 20);

var waveProvider = new StreamWaveProvider(decodedAudioReader.AsStream(), waveFormat);

wasapiOut.Init(waveProvider);

wasapiCapture.StartRecording();
wasapiOut.Play();
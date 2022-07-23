using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using OpenCvSharp;
using Echeneis;

/// <summary>
/// ICriteria実装サンプル
/// キャプチャデバイスから画像を取得し、「ポケモンのうりょく」がcriteriaimpl.config.jsonで指定された値であればtrue
/// </summary>
public class CriteriaImpl : ICriteria
{
    Dictionary<string, int> capture;
    Detect detect;
    TessConfig tessConfig;
    Stats targetStats;

    public CriteriaImpl()
    {
        var jsonConfig = JsonSerializer.Deserialize<Config>(File.ReadAllText(Path.Join(AppContext.BaseDirectory, "criteriaimpl.config.json")));
        Config config = jsonConfig != null ? jsonConfig : throw new FileNotFoundException();

        capture = config.Capture;
        detect = config.Detect;
        tessConfig = config.TessConfig;
        targetStats = config.Target;
    }

    public bool Verify()
    {
        var index = capture["index"];
        var width = capture["width"];
        var height = capture["height"];

        var timeout = 5000;
        var stopwatch = new Stopwatch();

        var cancellationTokenSource = new CancellationTokenSource(); ;
        var cancellationToken = cancellationTokenSource.Token;

        using (var videoCapture = new VideoCapture(index)
        {
            FrameWidth = width,
            FrameHeight = height
        })
        using (var mat = new Mat())
        {
            stopwatch.Start();
            while (!videoCapture.IsOpened() && stopwatch.ElapsedMilliseconds < timeout) Thread.Sleep(1);
            if (!videoCapture.IsOpened())
            {
                throw new Exception("VideoCapture seems not to open.");
            }
            videoCapture.Read(mat);

            var size = new Size(detect.Width, detect.Height);
            var currentStats = mat.GetStats(tessConfig, size, detect.X, detect.Y);

            Console.WriteLine("======");
            Console.WriteLine("targetStats: {0}", targetStats.ToString());
            Console.WriteLine("currentStats: {0}", currentStats.ToString());
            Console.WriteLine("======");

            return currentStats == targetStats;
        }
    }
}

public class Config
{
#pragma warning disable CS8618
    [JsonPropertyName("capture")]
    public Dictionary<string, int> Capture { init; get; }
    [JsonPropertyName("detect")]
    public Detect Detect { init; get; }
    [JsonPropertyName("tessConfig")]
    public TessConfig TessConfig { init; get; }
    [JsonPropertyName("target")]
    public Stats Target { init; get; }
#pragma warning restore CS8618
}

public class Detect
{
#pragma warning disable CS8618
    [JsonPropertyName("width")]
    public int Width { init; get; }
    [JsonPropertyName("height")]
    public int Height { init; get; }
    [JsonPropertyName("x")]
    public int[] X { init; get; }
    [JsonPropertyName("y")]
    public int[] Y { init; get; }
#pragma warning restore CS8618
}

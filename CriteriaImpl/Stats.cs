using System.Text.Json.Serialization;
using OpenCvSharp;

public record Stats
{
    //public int CurrentHP { init; get; }
    [JsonPropertyName("hp")]
    public int HP { init; get; }
    [JsonPropertyName("attack")]
    public int Attack { init; get; }
    [JsonPropertyName("defence")]
    public int Defense { init; get; }
    [JsonPropertyName("spAtk")]
    public int SpAtk { init; get; }
    [JsonPropertyName("spDef")]
    public int SpDef { init; get; }
    [JsonPropertyName("speed")]
    public int Speed { init; get; }

    [JsonConstructor]
    public Stats() { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="mat">検査対象の画像</param>
    /// <param name="size">3桁が収まる矩形</param>
    /// <param name="statsX">資料参照</param>
    /// <param name="statsY">資料参照</param>
    public Stats(Mat mat, TessConfig tessConfig, Size size, ICollection<int> statsX, ICollection<int> statsY)
    {
        var x = statsX.ToArray();
        var y = statsY.ToArray();
        var corners = new Point[] {
            new Point(x[0], y[0]),
            new Point(x[1], y[0]),
            new Point(x[0], y[1]),
            new Point(x[0], y[2]),
            new Point(x[2], y[0]),
            new Point(x[2], y[1]),
            new Point(x[2], y[2])
        };
        var results = GetStatsFromCorners(mat, tessConfig, size, corners);

        //CurrentHP = results[0];
        HP = results[1];
        Attack = results[2];
        Defense = results[3];
        SpAtk = results[4];
        SpDef = results[5];
        Speed = results[6];
    }

    /// <summary>
    /// 各範囲のOCR結果を配列に詰めて返す
    /// </summary>
    /// <param name="mat"></param>
    /// <param name="size"></param>
    /// <param name="corners"></param>
    /// <returns></returns>
    private int[] GetStatsFromCorners(Mat mat, TessConfig tessConfig, Size size, Point[] corners)
    {
        if (corners.Length != 7)
        {
            throw new Exception("The number of corners must be 7.");
        }

        var results = new int[7];
        Parallel.ForEach(corners.Select((value, index) => (value, index)), pair =>
        {

            var corner = pair.value;
            var rect = new Rect(corner, size);

            using (var statMat = mat.Clone(rect))
            {
                results[pair.index] = GetStatNumber(statMat, tessConfig);
            }
        });
        return results;
    }

    /// <summary>
    /// OCRで「ポケモンのうりょく」の数値を取得する。失敗時は0
    /// </summary>
    /// <param name="mat"></param>
    /// <returns></returns>
    private int GetStatNumber(Mat mat, TessConfig tessConfig)
    {
        using (var matGray = mat.CvtColor(ColorConversionCodes.BGR2GRAY))
        {
            if (!int.TryParse(matGray.GetOCRResult(tessConfig), out var result))
            {
                result = 0;
            }
            return result;
        }
    }

    public override string ToString()
    {
        return string.Format("{0},{1},{2},{3},{4},{5}", HP, Attack, Defense, SpAtk, SpDef, Speed);
    }
}

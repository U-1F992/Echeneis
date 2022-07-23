using OpenCvSharp;
using OpenCvSharp.Text;

public static class MatExtension
{
    /// <summary>
    /// OCR結果を取得する。
    /// </summary>
    /// <param name="mat"></param>
    /// <param name="tessConfig"></param>
    /// <returns></returns>
    public static string GetOCRResult(this Mat mat, TessConfig tessConfig)
    {
        using (var tesseract = OCRTesseract.Create(tessConfig.DataPath, tessConfig.Language, tessConfig.CharWhitelist, tessConfig.Oem, tessConfig.PsMode))
        {
            tesseract.Run(mat, out var outputText, out var componentRects, out var componentTexts, out var componentConfidences);
            return outputText;
        }
    }

    public static Stats GetStats(this Mat mat, TessConfig tessConfig, Size size, ICollection<int> statsX, ICollection<int> statsY)
    {
        return new Stats(mat, tessConfig, size, statsX, statsY);
    }
}

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Imaging;

namespace CardDesigner;

[SuppressMessage("Interoperability", "CA1416:Plattformkompatibilität überprüfen")]
public class Program
{
    public static void Main(string[] args)
    {
        const string TemplatePath = "template.json";
        const string OutputPath = "welcome_card.png";

        CardDesigner.GenerateImage(TemplatePath, OutputPath, ImageFormat.Png);
        Console.WriteLine($"Bild gespeichert unter: {OutputPath}");
        
        Process.Start("explorer.exe", OutputPath);
    }
}

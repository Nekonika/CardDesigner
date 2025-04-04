using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using CardDesigner.Enums;
using Newtonsoft.Json;

namespace CardDesigner.Classes;

[JsonObject]
[SuppressMessage("Interoperability", "CA1416:Plattformkompatibilität überprüfen")]
public class TextElement : Element
{
    [JsonRequired]
    public required string Content { get; set; }
    
    public string FontFamily { get; set; } = "Arial";
    
    public float FontSize { get; set; } = 14f;
    
    public AlignmentX TextAlignment { get; set; } = AlignmentX.Left;
    
    public string Color { get; set; } = "#FFFFFF";
    
    [JsonIgnore]
    public override int Width
    {
        get => Convert.ToInt32(Size.Width);
        set => throw new InvalidOperationException();
    }
    
    [JsonIgnore]
    public override int Height
    {
        get => Convert.ToInt32(Size.Height);
        set => throw new InvalidOperationException();
    }

    [JsonIgnore]
    public SolidBrush Brush => new SolidBrush(ColorTranslator.FromHtml(Color));
    
    [JsonIgnore]
    public Font Font => new Font(FontFamily, FontSize);
    
    [JsonIgnore]
    public SizeF Size
    {
        get
        {
            using Font MyFont = Font;
            return Template.Graphics.MeasureString(Content, Font);
        }
    }
}
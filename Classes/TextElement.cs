using System.ComponentModel;
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
    
    [DefaultValue("Arial")]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public string FontFamily { get; set; } = "Arial";
    
    [DefaultValue(14f)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public float FontSize { get; set; } = 14f;
    
    [DefaultValue(FontStyle.Regular)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public FontStyle FontStyle { get; set; } = FontStyle.Regular;
    
    [DefaultValue(AlignmentX.Left)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public AlignmentX TextAlignment { get; set; } = AlignmentX.Left;
    
    [DefaultValue("#FFFFFF")]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
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
    public Font Font => new Font(FontFamily, FontSize, FontStyle);
    
    [JsonIgnore]
    public SizeF Size
    {
        get
        {
            using Font MyFont = Font;
            return Template.Graphics.MeasureString(Content, Font);
        }
    }
    
    public new TextElement ShallowCopy()
        => (TextElement)MemberwiseClone();
}
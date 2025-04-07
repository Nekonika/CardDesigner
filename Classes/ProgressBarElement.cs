using CardDesigner.Enums;
using Newtonsoft.Json;

namespace CardDesigner.Classes;

[JsonObject]
public class ProgressBarElement : Element
{
    [JsonRequired]
    public float Value { get; set; }

    public Shape Shape { get; set; } = Shape.Rectangle;
    
    public string Color { get; set; } = "#00FF00";
    
    public string BorderColor { get; set; } = "#FFFFFF";
    
    public float BorderWidth { get; set; }
    
    [JsonRequired]
    public override int Width { get; set; }
    
    [JsonRequired]
    public override int Height { get; set; }
}
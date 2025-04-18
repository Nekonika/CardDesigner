using System.ComponentModel;
using CardDesigner.Enums;
using Newtonsoft.Json;

namespace CardDesigner.Classes;

[JsonObject]
public class ProgressBarElement : Element
{
    [JsonRequired]
    public float Value { get; set; }

    [DefaultValue(Shape.Rectangle)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public Shape Shape { get; set; } = Shape.Rectangle;
    
    [DefaultValue("#00FF00")]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public string Color { get; set; } = "#00FF00";
    
    [DefaultValue("#FFFFFF")]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public string BorderColor { get; set; } = "#FFFFFF";
    
    [DefaultValue(0)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public float BorderWidth { get; set; }
    
    [JsonRequired]
    public override int Width { get; set; }
    
    [JsonRequired]
    public override int Height { get; set; }
    
    public new ProgressBarElement ShallowCopy()
        => (ProgressBarElement)MemberwiseClone();
}
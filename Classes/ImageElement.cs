using System.ComponentModel;
using CardDesigner.Enums;
using Newtonsoft.Json;

namespace CardDesigner.Classes;

[JsonObject]
public class ImageElement : Element
{
    [JsonRequired]
    public required string Path { get; set; }
    
    [DefaultValue(Shape.Rectangle)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public Shape Shape { get; set; } = Shape.Rectangle;
    
    [JsonRequired]
    public override int Width { get; set; }
    
    [JsonRequired]
    public override int Height { get; set; }
    
    public new ImageElement ShallowCopy()
        => (ImageElement)MemberwiseClone();
}
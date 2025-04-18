using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using CardDesigner.Enums;
using Newtonsoft.Json;

namespace CardDesigner.Classes;

[JsonObject]
[SuppressMessage("Interoperability", "CA1416:Plattformkompatibilität überprüfen")]
public class ShapeElement : Element
{
    [DefaultValue(Shape.Rectangle)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public Shape Shape { get; set; } = Shape.Rectangle;

    [DefaultValue("#00000000")]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public string Color { get; set; } = "#00000000";

    [JsonIgnore]
    public Pen Pen => new Pen(ColorTranslator.FromHtml(Color)) 
    {
        Width = BorderThickness
    };

    [DefaultValue(0)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int BorderThickness { get; set; }
    
    [JsonRequired]
    public override int Width { get; set; }
    
    [JsonRequired]
    public override int Height { get; set; }
    
    public new ShapeElement ShallowCopy()
        => (ShapeElement)MemberwiseClone();
}
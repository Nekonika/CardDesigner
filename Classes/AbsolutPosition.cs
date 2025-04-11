using System.ComponentModel;
using CardDesigner.Interfaces;
using Newtonsoft.Json;

namespace CardDesigner.Classes;

[JsonObject]
public class AbsolutPosition : IPosition
{
    [DefaultValue(0)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int X { get; set; }
    
    [DefaultValue(0)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int Y { get; set; }

    public (int X, int Y) GetPosition(Template template, Element element, Element? parent)
        => (X, Y);
}
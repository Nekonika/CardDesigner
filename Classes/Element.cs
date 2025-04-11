using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using CardDesigner.Interfaces;
using Newtonsoft.Json;

namespace CardDesigner.Classes;

[JsonObject]
[SuppressMessage("Interoperability", "CA1416:Plattformkompatibilität überprüfen")]
public abstract class Element
{
    [JsonIgnore] 
    public Template Template { get; set; } = null!;
    
    [JsonIgnore] 
    public Element? Parent { get; set; }
    
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public IPosition Position { get; set; } = new AbsolutPosition();
    
    [JsonRequired]
    public abstract int Width { get; set; }
    
    [JsonRequired]
    public abstract int Height { get; set; }
    
    [DefaultValue("#00000000")]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public string Background { get; set; } = "#00000000";
    
    [JsonIgnore]
    public Color BackgroundColor => ColorTranslator.FromHtml(Background);
    
    [JsonIgnore]
    public SolidBrush BackgroundBrush => new SolidBrush(BackgroundColor);
    
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public Element[] Children { get; set; } = [];
}
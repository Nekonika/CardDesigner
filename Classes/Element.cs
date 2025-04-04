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
    
    public required IPosition Position { get; set; } = new AbsolutPosition();
    
    [JsonRequired]
    public abstract int Width { get; set; }
    
    [JsonRequired]
    public abstract int Height { get; set; }
    
    public string Background { get; set; } = "#00000000";
    
    [JsonIgnore]
    public Color BackgroundColor => ColorTranslator.FromHtml(Background);
    
    [JsonIgnore]
    public SolidBrush BackgroundBrush => new SolidBrush(BackgroundColor);
    
    public Element[] Children { get; set; } = [];
}
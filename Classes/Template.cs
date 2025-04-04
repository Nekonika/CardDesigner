using System.Drawing;
using Newtonsoft.Json;

namespace CardDesigner.Classes;

[JsonObject]
public class Template
{
    [JsonIgnore] 
    public Graphics Graphics { get; set; } = null!;
    
    [JsonRequired]
    public required int Width { get; set; }
    
    [JsonRequired]
    public required int Height { get; set; }
    
    public string Background { get; set; } = "#00000000";
    
    [JsonIgnore]
    public Color BackgroundColor => ColorTranslator.FromHtml(Background);
    
    public Element[] Children { get; set; } = [];
}
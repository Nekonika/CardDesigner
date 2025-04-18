using System.ComponentModel;
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
    
    [DefaultValue("#00000000")]
    public string Background { get; set; } = "#00000000";
    
    [JsonIgnore]
    public Color BackgroundColor => ColorTranslator.FromHtml(Background);
    
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public Element[] Children { get; set; } = [];
    
    public Template ShallowCopy()
        => (Template)MemberwiseClone();

    public Template DeepCopy()
        => this.Copy();
}
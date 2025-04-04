using CardDesigner.Interfaces;
using Newtonsoft.Json;

namespace CardDesigner.Classes;

[JsonObject]
public class AbsolutPosition : IPosition
{
    public int X { get; set; }
    public int Y { get; set; }

    public (int X, int Y) GetPosition(Template template, Element element, Element? parent)
        => (X, Y);
}
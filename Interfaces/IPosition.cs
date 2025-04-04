using CardDesigner.Classes;

namespace CardDesigner.Interfaces;

public interface IPosition
{
    public (int X, int Y) GetPosition(Template template, Element element, Element? parent);
}
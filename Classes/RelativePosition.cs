using System.Drawing;
using CardDesigner.Enums;
using CardDesigner.Interfaces;
using Newtonsoft.Json;

namespace CardDesigner.Classes;

[JsonObject]
public class RelativePosition : IPosition
{
    public AlignmentX AlignmentX { get; set; } = AlignmentX.Left;
    
    public AlignmentY AlignmentY { get; set; } = AlignmentY.Top;

    public (int X, int Y) GetPosition(Template template, Element element, Element? parent)
    {
        (int ParentX, int ParentY) = parent is not null
            ? parent.Position.GetPosition(template, parent, parent.Parent)
            : (0, 0);
        
        int ParentWidth = parent?.Width ?? template.Width;
        int ParentHeight = parent?.Height ?? template.Height;
        
        TextElement? TextElement = element as TextElement;
        SizeF? TextElementSize = TextElement?.Size;
        int Width = TextElementSize.HasValue ? Convert.ToInt32(TextElementSize.Value.Width) : element.Width;
        int Height = TextElementSize.HasValue ? Convert.ToInt32(TextElementSize.Value.Height) : element.Height;
        int X = CalculateAlignmentX(AlignmentX, ParentX, ParentWidth, Width);
        int Y = CalculateAlignmentY(AlignmentY, ParentY, ParentHeight, Height);
        
        return (X, Y);
    }
    
    public static int CalculateAlignmentX(AlignmentX alignment, int parentPos, int parentSize, int elementSize)
    {
        return alignment switch
        {
            AlignmentX.Center => parentPos + (parentSize - elementSize) / 2,
            AlignmentX.Right => parentPos + parentSize - elementSize,
            _ => parentPos
        };
    }

    public static int CalculateAlignmentY(AlignmentY alignment, int parentPos, int parentSize, int elementSize)
    {
        return alignment switch
        {
            AlignmentY.Middle => parentPos + (parentSize - elementSize) / 2,
            AlignmentY.Bottom => parentPos + parentSize - elementSize,
            _ => parentPos
        };
    }
}
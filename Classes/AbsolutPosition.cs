﻿using System.ComponentModel;
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
    {
        (int ParentX, int ParentY) = parent?.Position.GetPosition(template, parent, parent.Parent) ?? (0,0);
        return (ParentX + X, ParentY + Y);
    }
    
    public AbsolutPosition ShallowCopy()
        => (AbsolutPosition)MemberwiseClone();
}
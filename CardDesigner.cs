using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using CardDesigner.Classes;
using CardDesigner.Enums;
using CardDesigner.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CardDesigner;

[SuppressMessage("Interoperability", "CA1416:Plattformkompatibilität überprüfen")]
public static class CardDesigner
{
    private static readonly JsonSerializerSettings _JsonSerializerSettings = new JsonSerializerSettings
    {
        Formatting = Formatting.Indented,
        TypeNameHandling = TypeNameHandling.Auto,
        DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
        NullValueHandling = NullValueHandling.Ignore,
        Converters =
        [
            new StringEnumConverter()
        ],
        ContractResolver = new IgnoreEmptyCollectionContractResolver()
            .IgnoreEmptyCollections()
            .SetDefaultProperty(typeof(Element[]), Array.Empty<Element>())
            .SetDefaultProperty(typeof(IPosition), new AbsolutPosition())
    };
    
    public delegate Task Log(string message);
    public static event Log? OnLog;
    private static void SendLog(string message)
        => OnLog?.Invoke(message).GetAwaiter().GetResult();
    
    public static string GetJsonFromTemplate(Template template, Formatting formatting = Formatting.Indented)
        => JsonConvert.SerializeObject(template, formatting, _JsonSerializerSettings);

    public static void WriteTemplateToFile(Template template, string path, Formatting formatting = Formatting.Indented)
        => File.WriteAllText(path, GetJsonFromTemplate(template, formatting));
    
    public static Template GetTemplateFromFile(string templatePath)
    {
        if (!File.Exists(templatePath))
            throw new FileNotFoundException("Template not found", templatePath);

        string JsonContent = File.ReadAllText(templatePath);
        Template Template = JsonConvert.DeserializeObject<Template>(JsonContent, _JsonSerializerSettings)!;

        return Template;
    }
    
    public static void GenerateImage(string templatePath, string outputPath, ImageFormat imageFormat)
        => GenerateImage(GetTemplateFromFile(templatePath), outputPath, imageFormat);
    public static void GenerateImage(Template template, string outputPath, ImageFormat imageFormat)
    {
        using Bitmap Bitmap = GenerateBitmap(template);
        Bitmap.Save(outputPath, imageFormat);
    }
    
    public static Bitmap GenerateBitmap(string templatePath)
        => GenerateBitmap(GetTemplateFromFile(templatePath));
    public static Bitmap GenerateBitmap(Template template)
    {
        Bitmap Bitmap = new(template.Width, template.Height);
        template.Graphics = Graphics.FromImage(Bitmap);
        template.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        template.Graphics.CompositingQuality = CompositingQuality.HighQuality;
        template.Graphics.SmoothingMode = SmoothingMode.HighQuality;
        template.Graphics.Clear(template.BackgroundColor);

        foreach (Element Element in template.Children)
            DrawElement(template, Element);
        
        template.Graphics.Flush();
        template.Graphics.Dispose();
        
        return Bitmap;
    }

    private static void DrawElement(Template template, Element element, Element? parent = null)
    {
        element.Template = template;
        element.Parent = parent;

        (int X, int Y) = element.Position.GetPosition(template, element, parent);
        SendLog($"Drawing element at ({X}, {Y}) with size {element.Width}x{element.Height}");

        SolidBrush BackgroundBrush = element.BackgroundBrush;
        
        switch (element)
        {
            case TextElement TextElement:
                SendLog($"Text: {TextElement.Content}, Color: {TextElement.Color}");
                template.Graphics.FillRectangle(BackgroundBrush, X, Y, element.Width, element.Height);
                
                Regex NewLineRegex = RegexHelper.NewLineRegex;

                using (Font Font = TextElement.Font)
                using (Brush TextBrush = TextElement.Brush)
                {
                    List<(string, SizeF)> Lines = NewLineRegex
                        .Split(TextElement.Content)
                        .Select(line => (line, template.Graphics.MeasureString(line, Font)))
                        .ToList();
                    
                    int MaxWidth = Convert.ToInt32(Lines.Max(entry => entry.Item2.Width));
                    float MaxHeight = Lines.Max(entry => entry.Item2.Height);
                    float CurrentY = Y;
                    foreach ((string Line, SizeF Size) in Lines)
                    {
                        template.Graphics.DrawString(Line, Font, TextBrush, RelativePosition.CalculateAlignmentX(TextElement.TextAlignment, X, MaxWidth, Convert.ToInt32(Size.Width)), CurrentY);
                        CurrentY += MaxHeight;
                    }
                }
                break;
            
            case ImageElement ImageElement:
                SendLog($"Path: {ImageElement.Path}, Shape: {ImageElement.Shape}");
                using (Bitmap Img = LoadImage(ImageElement.Path))
                {
                    switch (ImageElement.Shape)
                    {
                        case Shape.Rectangle:
                            template.Graphics.FillRectangle(BackgroundBrush, X, Y, element.Width, element.Height);
                            template.Graphics.DrawImage(Img, X, Y, ImageElement.Width, ImageElement.Height);
                            break;
                        
                        case Shape.Circle:
                            using (GraphicsPath Path = new GraphicsPath())
                            {
                                Path.AddEllipse(X, Y, ImageElement.Width, ImageElement.Height);
                                template.Graphics.SetClip(Path);
                                template.Graphics.FillRectangle(BackgroundBrush, X, Y, element.Width, element.Height);
                                template.Graphics.DrawImage(Img, X, Y, ImageElement.Width, ImageElement.Height);
                                template.Graphics.ResetClip();
                            }
                            break;
                            
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                break;
            
            case ShapeElement ShapeElement:
                SendLog($"Shape: {ShapeElement.Shape}, Border Thickness: {ShapeElement.BorderThickness}");
                SendLog($"Background: {ShapeElement.Background}, Color: {ShapeElement.Color}");
                Pen ShapePen = ShapeElement.Pen;
                switch (ShapeElement.Shape)
                {
                    case Shape.Rectangle:
                            template.Graphics.FillRectangle(BackgroundBrush, X, Y, element.Width, element.Height);
                            if (ShapeElement.BorderThickness > 0) 
                                template.Graphics.DrawRectangle(ShapePen, X, Y, ShapeElement.Width, ShapeElement.Height); 
                            break;
                    
                    case Shape.Circle:
                        template.Graphics.FillEllipse(BackgroundBrush, X, Y, ShapeElement.Width, ShapeElement.Height);
                        if (ShapeElement.BorderThickness > 0)
                            template.Graphics.DrawEllipse(ShapePen, X, Y, ShapeElement.Width, ShapeElement.Height);
                        break;
                        
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                break;
            
            case ProgressBarElement ProgressBarElement:
                SendLog($"Shape: {ProgressBarElement.Shape}, Value: {ProgressBarElement.Value}");
                SendLog($"Background: {ProgressBarElement.Background}, Color: {ProgressBarElement.Color}");
                float FilledWidth = (ProgressBarElement.Value / 100f) * ProgressBarElement.Width;
                using (SolidBrush FillBrush = new SolidBrush(ColorTranslator.FromHtml(ProgressBarElement.Color)))
                using (Pen BorderPen = new Pen(ColorTranslator.FromHtml(ProgressBarElement.BorderColor), ProgressBarElement.BorderWidth))
                {
                    switch (ProgressBarElement.Shape)
                    {
                        case Shape.Rectangle:
                            template.Graphics.FillRectangle(BackgroundBrush, X, Y, ProgressBarElement.Width, ProgressBarElement.Height);
                            template.Graphics.FillRectangle(FillBrush, X, Y, FilledWidth, ProgressBarElement.Height);
                            template.Graphics.DrawRectangle(BorderPen, X, Y, ProgressBarElement.Width, ProgressBarElement.Height);
                            break;
                        
                        case Shape.Circle:
                            using (GraphicsPath Border = new GraphicsPath())
                            {
                                Border.StartFigure();
                                Border.AddArc(X, Y, ProgressBarElement.Height, ProgressBarElement.Height, 90, 180);
                                Border.AddArc(X + ProgressBarElement.Width - ProgressBarElement.Height, Y, ProgressBarElement.Height, ProgressBarElement.Height, 270, 180);
                                Border.CloseFigure();
                                
                                // Background
                                template.Graphics.FillPath(BackgroundBrush, Border);
                                
                                // Progress
                                if (FilledWidth > 0)
                                {
                                    using GraphicsPath FilledPath = new GraphicsPath();
                                    template.Graphics.SetClip(Border);
                                    FilledPath.StartFigure();
                                    
                                    FilledPath.AddArc(X, Y, ProgressBarElement.Height, ProgressBarElement.Height,
                                        90, 180);
                                    if (FilledWidth > ProgressBarElement.Height)
                                    {
                                        float Radius = ProgressBarElement.Height / 2f;
                                        float FilledRight = X + FilledWidth;
                                        FilledPath.AddLine(X + Radius, Y, FilledRight - Radius, Y);
                                        FilledPath.AddArc(FilledRight - ProgressBarElement.Height, Y,
                                            ProgressBarElement.Height, ProgressBarElement.Height, 270, 180);
                                    }
                                    else
                                    {
                                        FilledPath.AddLine(X + (FilledWidth / 2f), Y, X + (FilledWidth / 2f), Y + ProgressBarElement.Height);
                                    }

                                    FilledPath.CloseFigure();

                                    template.Graphics.FillPath(FillBrush, FilledPath);
                                    template.Graphics.ResetClip();
                                }

                                // Border
                                template.Graphics.DrawPath(BorderPen, Border);
                            }
                            break;
                        
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                break;
                }
        }
        
        foreach (Element Child in element.Children)
            DrawElement(template, Child, element);
    }

    private static Bitmap LoadImage(string path)
    {
        try
        {
            if (Uri.IsWellFormedUriString(path, UriKind.Absolute))
            {
                using HttpClient Client = new HttpClient();
                using HttpRequestMessage Request = new HttpRequestMessage(HttpMethod.Get, path);
                using HttpResponseMessage Response = Client
                    .Send(Request)
                    .EnsureSuccessStatusCode();
                
                using Stream ImageStream = Response.Content.ReadAsStream();
                return new Bitmap(ImageStream);
            }
            
            if (File.Exists(path))
                return new Bitmap(path);
            
            throw new FileNotFoundException("Image was not found: " + path);
        }
        catch (Exception Ex)
        {
            SendLog("An error occured while trying to load an image: " + Ex.Message);
            return new Bitmap(1, 1);
        }
    }
}
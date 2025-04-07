using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using CardDesigner.Classes;
using CardDesigner.Enums;
using Newtonsoft.Json;

namespace CardDesigner;

[SuppressMessage("Interoperability", "CA1416:Plattformkompatibilität überprüfen")]
public static class CardDesigner
{
    public static Template GetTemplateFromFile(string templatePath)
    {
        if (!File.Exists(templatePath))
            throw new FileNotFoundException("Template not found", templatePath);

        string JsonContent = File.ReadAllText(templatePath);
        Template Template = JsonConvert.DeserializeObject<Template>(JsonContent, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            DefaultValueHandling = DefaultValueHandling.Ignore
        })!;

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
        Console.WriteLine($"Zeichne Element an ({X}, {Y}) mit Größe {element.Width}x{element.Height}");

        SolidBrush BackgroundBrush = element.BackgroundBrush;
        
        switch (element)
        {
            case TextElement TextElement:
                Console.WriteLine($"Text: {TextElement.Content}, Farbe: {TextElement.Color}");
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
                Console.WriteLine($"Bildpfad: {ImageElement.Path}, Shape: {ImageElement.Shape}");
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
                Console.WriteLine($"Form: {ShapeElement.Shape}, Dicke: {ShapeElement.BorderThickness}");
                Console.WriteLine($"Hintergrund: {ShapeElement.Background}, Farbe: {ShapeElement.BorderThickness}");
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
                                using (GraphicsPath FilledPath = new GraphicsPath())
                                {
                                    if (FilledWidth > 0)
                                    {
                                        FilledPath.StartFigure();
                                        FilledPath.AddArc(X, Y, ProgressBarElement.Height, ProgressBarElement.Height, 90, 180);
                                        if (FilledWidth > ProgressBarElement.Height)
                                        {
                                            float Radius = ProgressBarElement.Height / 2f;
                                            float FilledRight = X + FilledWidth;
                                            FilledPath.AddLine(X + Radius, Y, FilledRight - Radius, Y);
                                            FilledPath.AddArc(FilledRight - ProgressBarElement.Height, Y, ProgressBarElement.Height, ProgressBarElement.Height, 270, 180);
                                        }
                                        else
                                        {
                                            FilledPath.AddArc(X, Y, FilledWidth * 2, ProgressBarElement.Height, 90, -180);
                                        }
                                        FilledPath.CloseFigure();
                                        
                                        template.Graphics.FillPath(FillBrush, FilledPath);
                                    }
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
            
            throw new FileNotFoundException("Bild konnte nicht geladen werden: " + path);
        }
        catch (Exception Ex)
        {
            Console.WriteLine("Fehler beim Laden des Bildes: " + Ex.Message);
            return new Bitmap(1, 1);
        }
    }
}
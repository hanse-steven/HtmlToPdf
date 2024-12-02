using System.Text;
using DinkToPdf;
using DinkToPdf.Contracts;
using HtmlToPdf.TemplateRapport;
using Microsoft.AspNetCore.Mvc;
using Razor.Templating.Core;

namespace HtmlToPdf.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PdfController : ControllerBase
{
    private readonly IConverter _converter;
    public PdfController(IConverter converter)
    {
        _converter = converter;
    }
    
    [HttpGet]
    public async Task<IActionResult> DonneMoiMonPdf()
    {
        var model = new PdfTemplate();
        string html = await RazorTemplateEngine.RenderAsync("~/TemplateRapport/PdfTemplate.cshtml", model);
        
        GlobalSettings globalSettings = new GlobalSettings
        {
            ColorMode = ColorMode.Color,
            Orientation = Orientation.Portrait,
            PaperSize = PaperKind.A4,
            Margins = new MarginSettings { Top = 10, Bottom = 10, Left = 10, Right = 10 }
        };
        
        ObjectSettings objectSettings = new ObjectSettings
        {
            PagesCount = true,
            HtmlContent = html,
            WebSettings = { DefaultEncoding = "utf-8" },
            // HeaderSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "Test"},
            // FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "[page]" }
            HeaderSettings = new HeaderSettings
            {
                FontName = "Arial",
                FontSize = 9,
                Line = true,
                Center = "Rapport Financier",
                Right = "[date] [time]",
                Spacing = 2.5
            },
            FooterSettings = new FooterSettings
            {
                FontName = "Arial",
                FontSize = 9,
                Line = true,
                Center = "[page] de [toPage]",
                Left = "Confidentiel",
                Right = "PlayZone2024",
                Spacing = 2.5
            }
        };
        
        HtmlToPdfDocument pdf = new HtmlToPdfDocument()
        {
            GlobalSettings = globalSettings,
            Objects = { objectSettings }
        };
        
        byte[] pdfBytes = _converter.Convert(pdf);
        return File(pdfBytes, "application/pdf", "Rapport.pdf", true);
    }
}


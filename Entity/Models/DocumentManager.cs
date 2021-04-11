using System;
using System.Collections.Generic;
using System.Text;
using Timesheet.Entity.Entities;
using GemBox.Document;
using System.IO;
using System.Threading.Tasks;

namespace Timesheet.Entity.Models
{
    public class DocumentManager : IDocumentManager
    {
        public string Format { get; set; }
        public byte[] GetContract(Person person, DocumentStorage defaultDocument)
        {
            using (MemoryStream stream = new MemoryStream(defaultDocument.DocumentSource))
            {
                return stream.ToArray();
            }
        }
        public DocumentManager(string format = "PDF")
        {
            Format = format;
        }
        public SaveOptions Options => this.FormatMappingDictionary[this.Format];
        public IDictionary<string, SaveOptions> FormatMappingDictionary => new Dictionary<string, SaveOptions>()
        {
            ["DOCX"] = new DocxSaveOptions(),
            ["HTML"] = new HtmlSaveOptions() { EmbedImages = true },
            ["RTF"] = new RtfSaveOptions(),
            ["TXT"] = new TxtSaveOptions(),
            ["PDF"] = new PdfSaveOptions(),
            ["XPS"] = new XpsSaveOptions(),
            ["XML"] = new XmlSaveOptions(),
            ["BMP"] = new ImageSaveOptions(ImageSaveFormat.Bmp),
            ["PNG"] = new ImageSaveOptions(ImageSaveFormat.Png),
            ["JPG"] = new ImageSaveOptions(ImageSaveFormat.Jpeg),
            ["GIF"] = new ImageSaveOptions(ImageSaveFormat.Gif),
            ["TIF"] = new ImageSaveOptions(ImageSaveFormat.Tiff)
        };
    }
}

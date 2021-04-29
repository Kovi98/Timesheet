using System;
using System.Collections.Generic;
using System.Text;
using Timesheet.Entity.Entities;
using Timesheet.DocManager.Entities;
using System.IO;
using System.Threading.Tasks;
using Novacode;
using PuppeteerSharp;

namespace Timesheet.DocManager.Models
{
    public class DocumentManager : IDocumentManager
    {
        public ExportFormat Format { get; set; }

        public string ContentType
        {
            get
            {
                string result = string.Empty;
                switch (Format)
                {
                    case ExportFormat.Docx:
                        result = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                        break;
                    case ExportFormat.Pdf:
                        result = "application/pdf";
                        break;
                    case ExportFormat.Rtf:
                        result = "text/richtext";
                        break;
                    default:
                        result = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                        break;

                }
                return result;
            }
        }
        public byte[] GetContract(Person person, DocumentStorage defaultDocument)
        {
            using (MemoryStream streamResult = new MemoryStream())
            {
                using (MemoryStream streamLoad = new MemoryStream(defaultDocument.DocumentSource))
                {
                    using (DocX doc = DocX.Load(streamLoad))
                    {
                        doc.ReplaceText("%Name%", person.FullName);
                        doc.ReplaceText("%DateBirth%", person.DateBirth.ToString("dd.MM.yyyy"));
                        doc.ReplaceText("%Address%", person.FullAddress);
                        doc.ReplaceText("%HourReward%", person.Job.HourReward.ToString());
                        doc.ReplaceText("%BankAccount%", person.FullBankAccount);
                        doc.SaveAs(streamResult);
                    }
                }

                return streamResult.ToArray();
            }
        }
        public DocumentManager(ExportFormat format = ExportFormat.Docx)
        {
            Format = format;
        }
        public DocumentManager(string format)
        {
            switch (format)
            {
                case "DOCX":
                    Format = ExportFormat.Docx;
                    break;
                case "PDF":
                    Format = ExportFormat.Pdf;
                    break;
                case "RTF":
                    Format = ExportFormat.Rtf;
                    break;
            }
        }

        public async Task<byte[]> ConvertPageToPdfAsync(string url)
        {
            var browserFetcher = new BrowserFetcher();
            await browserFetcher.DownloadAsync();
            await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
            await using var page = await browser.NewPageAsync();
            await page.GoToAsync(url);
            var result = await page.PdfDataAsync();
            return result;
        }
        public async Task<byte[]> ConvertHtmlToPdfAsync(string html)
        {
            var browserFetcher = new BrowserFetcher();
            await browserFetcher.DownloadAsync();
            await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
            await using var page = await browser.NewPageAsync();
            await page.SetContentAsync(html);
            var result = await page.PdfDataAsync();
            return result;
        }
    }

    public enum ExportFormat
    {
        Docx,
        Pdf,
        Rtf
    }
}

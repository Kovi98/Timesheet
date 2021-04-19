using System;
using System.Collections.Generic;
using System.Text;
using Timesheet.Entity.Entities;
using Timesheet.DocManager.Entities;
using System.IO;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using System.Text.RegularExpressions;

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
            using(MemoryStream streamLoad = new MemoryStream(defaultDocument.DocumentSource))
            {
                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(streamLoad, true))
                {
                    string docText = null;
                    using (StreamReader sr = new StreamReader(wordDoc.MainDocumentPart.GetStream()))
                    {
                        docText = sr.ReadToEnd();
                    }

                    docText = new Regex("%Name%", RegexOptions.IgnoreCase)
                        .Replace(docText, person.FullName);

                    docText = new Regex("%DateBirth%", RegexOptions.IgnoreCase)
                        .Replace(docText, person.DateBirth.ToString("dd.MM.yyyy"));

                    docText = new Regex("%Address%", RegexOptions.IgnoreCase)
                        .Replace(docText, person.FullAddress);

                    docText = new Regex("%HourReward%", RegexOptions.IgnoreCase)
                        .Replace(docText, person.Job.HourReward.ToString());

                    docText = new Regex("%BankAccount%", RegexOptions.IgnoreCase)
                        .Replace(docText, person.FullBankAccount);

                    using (StreamWriter sw = new StreamWriter(wordDoc.MainDocumentPart.GetStream(FileMode.Create)))
                    {
                        sw.Write(docText);
                    }
                }

                return streamLoad.ToArray();
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
    }

    public enum ExportFormat
    {
        Docx,
        Pdf,
        Rtf
    }
}

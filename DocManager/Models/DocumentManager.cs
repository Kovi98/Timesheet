using System;
using System.Collections.Generic;
using System.Text;
using Timesheet.Entity.Entities;
using Timesheet.DocManager.Entities;
using System.IO;
using System.Threading.Tasks;
using Spire.Doc;

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
        public FileFormat FileFormat
        {
            get
            {
                FileFormat result;
                switch (Format)
                {
                    case ExportFormat.Docx:
                        result = FileFormat.Docx2013;
                        break;
                    case ExportFormat.Pdf:
                        result = FileFormat.PDF;
                        break;
                    case ExportFormat.Rtf:
                        result = FileFormat.Rtf;
                        break;
                    default:
                        result = FileFormat.Docx2013;
                        break;
                }
                return result;
            }
        }
        public byte[] GetContract(Person person, DocumentStorage defaultDocument)
        {
            Document doc;
            using (MemoryStream streamLoad = new MemoryStream(defaultDocument.DocumentSource))
            {
                doc = new Document(streamLoad);
            }
            //Naplnění šablony
            doc.Replace("%Name%", person.FullName, true, false);
            doc.Replace("%DateBirth%", person.DateBirth.ToString("dd.MM.yyyy"), true, false);
            doc.Replace("%Address%", person.FullAddress, true, false);
            doc.Replace("%HourReward%", person.Job.HourReward.ToString(), true, false);
            doc.Replace("%BankAccount%", person.FullBankAccount, true, false);

            using (MemoryStream streamSave = new MemoryStream())
            {
                doc.SaveToFile(streamSave, FileFormat);
                return streamSave.ToArray();
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

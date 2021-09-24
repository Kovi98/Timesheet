using Microsoft.EntityFrameworkCore;
using Novacode;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Timesheet.DocManager.Entities;
using Timesheet.Entity.Entities;

namespace Timesheet.DocManager.Services
{
    public class DocumentManager : IDocumentManager
    {
        private readonly DocumentContext _context;
        public async Task<byte[]> GenerateContract(Person person, DocumentStorage defaultDocument = null)
        {
            if (defaultDocument == null) defaultDocument = await GetDefaultDocumentAsync();
            using (MemoryStream streamResult = new MemoryStream())
            {
                using (MemoryStream streamLoad = new MemoryStream(defaultDocument.DocumentSource))
                {
                    using (DocX doc = DocX.Load(streamLoad))
                    {
                        doc.ReplaceText("%Name%", person.FullName);
                        doc.ReplaceText("%Job%", person.Job.Name);
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
        public DocumentManager(DocumentContext dbContext)
        {
            _context = dbContext;
        }
        public string GetContentType(ExportFormat format)
        {
            string result = format switch
            {
                ExportFormat.Docx => "application/vnd.openxmlformats-officedocument.wordprocessingml.document"//,
                //ExportFormat.Pdf => "application/pdf",
                //ExportFormat.Rtf => "text/richtext"
            };
            return result;
        }
        public async Task<DocumentStorage> GetDefaultDocumentAsync()
        {
            return await _context.DocumentStorage.Where(x => x.IsDefault).FirstOrDefaultAsync();
        }
        public async Task<List<DocumentStorage>> GetDocumentsAsync(bool asNoTracking = true)
        {
            var storage = _context.DocumentStorage;

            return await (asNoTracking ? storage.AsNoTracking().ToListAsync() : storage.AsTracking().ToListAsync());
        }
        public async Task SaveAsync(DocumentStorage document)
        {
            if (document.Id > 0)
            {
                _context.Entry<DocumentStorage>(document).State = EntityState.Modified;
            }
            else
            {
                _context.DocumentStorage.Add(document);
            }
            await _context.SaveChangesAsync();
        }
    }

    public enum ExportFormat
    {
        Docx//,
        //Pdf,
        //Rtf
    }
}

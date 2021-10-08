using Microsoft.EntityFrameworkCore;
using Novacode;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Timesheet.Common;
using Timesheet.Db;

namespace Timesheet.Business
{
    public class DocumentManager : IDocumentManager
    {
        private readonly TimesheetContext _context;
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
        public DocumentManager(TimesheetContext dbContext)
        {
            _context = dbContext;
        }
        public string GetContentType(ExportFormat format)
        {
            return format switch
            {
                ExportFormat.Docx => "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
            };
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
        public async Task<DocumentStorage> GetAsync(int id)
        {
            return await _context.DocumentStorage.FindAsync(id);
        }
        public async Task RemoveAsync(DocumentStorage document)
        {
            _context.DocumentStorage.Remove(document);
            await _context.SaveChangesAsync();
        }
    }
}

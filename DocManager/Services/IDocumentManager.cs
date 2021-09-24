using System.Collections.Generic;
using System.Threading.Tasks;
using Timesheet.DocManager.Entities;
using Timesheet.Entity.Entities;

namespace Timesheet.DocManager.Services
{
    public interface IDocumentManager
    {
        Task<DocumentStorage> GetDefaultDocumentAsync();
        Task<List<DocumentStorage>> GetDocumentsAsync(bool asNoTracking = true);
        string GetContentType(ExportFormat format);
        Task<byte[]> GenerateContract(Person person, DocumentStorage defaultDocument = null);
        Task SaveAsync(DocumentStorage document);
    }
}

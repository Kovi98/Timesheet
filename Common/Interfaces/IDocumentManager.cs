using System.Collections.Generic;
using System.Threading.Tasks;

namespace Timesheet.Common
{
    public interface IDocumentManager
    {
        Task<DocumentStorage> GetDefaultDocumentAsync();
        Task<List<DocumentStorage>> GetDocumentsAsync(bool asNoTracking = true);
        string GetContentType(ExportFormat format);
        string GetContentType(string format);
        Task<byte[]> GenerateContract(Person person, DocumentStorage defaultDocument = null);
        Task SaveAsync(DocumentStorage document);
        Task<DocumentStorage> GetAsync(int id);
        Task RemoveAsync(DocumentStorage document);
    }
}

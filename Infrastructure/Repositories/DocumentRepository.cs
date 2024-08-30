using Domain.Interfaces;
using Domain.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class DocumentRepository :  GenericRepository<Document>, IDocumentRepository
    {
        public DocumentRepository(DataContext context)  : base(context) 
        {

        }

        public async Task<ICollection<Document>> GetDocumentsByWorkspaceId(int workspaceId)
        {
           return await _context.Documents.Where(d => d.Folder.WorkspaceId == workspaceId && !d.IsDeleted).ToListAsync();
        }
    }
}

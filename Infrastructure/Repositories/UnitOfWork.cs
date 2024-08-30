using Domain.Interfaces;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _context;
        public IUserRepository User { get; }
        public IWorkspaceRepository Workspace { get; }
        public IFolderRepository Folder { get; }
        public IDocumentRepository Document { get; }


        public UnitOfWork(DataContext context,IUserRepository userRepository,
                           IWorkspaceRepository workspaceRepository,
                           IFolderRepository folderRepository, IDocumentRepository documentRepository) 
        {

            _context = context;
            User = userRepository;
            Workspace = workspaceRepository;
            Folder = folderRepository;
            Document = documentRepository;
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose (bool disposing)
        {
            _context.Dispose();
        }

        public int Save()
        {
           return  _context.SaveChanges();
        }
    }
}

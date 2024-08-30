using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository User { get; }
        IWorkspaceRepository Workspace { get; }
        IFolderRepository Folder { get; }
        IDocumentRepository Document { get; }

        int Save();

    }
}

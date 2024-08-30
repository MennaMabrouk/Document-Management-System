using Application.Dto;
using Application.Interfaces;
using AutoMapper;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class DocumentService :BaseService, IDocumentService
    {
        public DocumentService(IUnitOfWork unitOfWork, IMapper mapper, IFileService fileService)
            : base(unitOfWork, mapper, fileService)
        {
        }

        public async Task<ICollection<DocumentDto>> GetDocumentsByWorkspaceId(int workspaceId)
        {
            if(workspaceId >0)
            {
                var documents = await _unitOfWork.Document.GetDocumentsByWorkspaceId(workspaceId);
                if(documents !=null)
                {
                    return _mapper.Map<ICollection<DocumentDto>>(documents);
                }
            }

            return new List<DocumentDto>();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Application.Dto;
using AutoMapper;
using Domain.Models;



namespace Application.Helper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles() 
        {
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();

            CreateMap<Workspace, WorkspaceDto>();
            CreateMap<WorkspaceDto, Workspace>();

            CreateMap<Folder, FolderDto>();
            CreateMap<FolderDto, Folder>();

            CreateMap<Domain.Models.Document, DocumentDto>();
            CreateMap<DocumentDto, Domain.Models.Document>();
        }



    }
}

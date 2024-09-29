using Application.Dto;
using Application.Dto.User;
using Application.Interfaces;
using AutoMapper;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class UserService :BaseService, IUserService
    {
        public UserService(IUnitOfWork unitOfWork, IMapper mapper, IFileService fileService)
            : base(unitOfWork, mapper, fileService)
        {
        }

        public async Task<ICollection<UserDto>> GetAllUsers()
        {
           var users =  await _unitOfWork.User.GetAllNonAdminUsers();

            if (!users.Any())
                return new List<UserDto>();

            return  _mapper.Map<ICollection<UserDto>>(users); 
        }

        public async Task<UserDto> GetUserById(int userClaims,int userId,string roleClaims)
        {

            if (userId <= 0)
                throw new ArgumentException("Invalid user ID", nameof(userId));

            if(userClaims <=0)
                throw new ArgumentException("Invalid user claims ID.", nameof(userClaims));

            //Admin can access anyway
            if (userClaims != userId && roleClaims == "User")
                throw new UnauthorizedAccessException("You are not authorized to access this user's information.");


            var user = await _unitOfWork.User.GetById(userId);
            if (user == null)
                throw new KeyNotFoundException("User with the provided ID was not found.");

             var workspace = await _unitOfWork.Workspace.GetWorkspaceByUserId(userClaims);
            var userDto = _mapper.Map<UserDto>(user);
            // Check if the workspace exists before assigning the name
            if (workspace != null)
            {
                userDto.WorkspaceName = workspace.Name;
            }

            return userDto;

        }


        public async Task<bool> UpdateUser(int userClaims,UserDto userDto)
        {
            if (userDto == null)
                throw new ArgumentNullException(nameof(userDto), "User data cannot be null.");

            if (userClaims <=0)
                throw new ArgumentException("Invalid user claims ID.", nameof(userClaims));

            if(userDto.Id != userClaims)
                throw new UnauthorizedAccessException("You are not authorized to access this user's information.");

            var user = await _unitOfWork.User.GetById(userDto.Id);
            if (user == null)
                throw new KeyNotFoundException("User with the provided ID was not found.");


            _mapper.Map(userDto, user);
            _unitOfWork.User.Update(user);
            var result = _unitOfWork.Save();
            return result > 0;

        }

        public async Task<bool> CreateWorkspacePathForUser(string workspaceName)
        {
            return await _fileService.CreateWorkspacePath(workspaceName);

        }

        public async Task<UserDto> GetUserByWorkspaceId(int workspaceId)
        {
            if (workspaceId > 0)
            {
                var user = await _unitOfWork.User.GetUserByWorkspaceId(workspaceId);
                if (user != null)
                {
                    return _mapper.Map<UserDto>(user);
                }
            }

            return null;
        }

        public async Task<PaginatedResult<UserDto>> GetPaginatedUsers(int pageNumber, int pageSize)
        {
            var allUsers = await _unitOfWork.User.GetAllNonAdminUsers();


            var paginatedUsers = allUsers
                  .Skip((pageNumber - 1) * pageSize)
                  .Take(pageSize)
                  .ToList();

            var userDtos = _mapper.Map<ICollection<UserDto>>(paginatedUsers);

            return new PaginatedResult<UserDto>
            {
                Items = userDtos
            };
        }
    }
}

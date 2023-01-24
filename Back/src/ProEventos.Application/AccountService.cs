using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProEventos.Application.Contratos;
using ProEventos.Application.DTOs;
using ProEventos.Domain.Identity;
using ProEventos.Persistence.Contratos;
using AutoMapper;


namespace ProEventos.Application
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly IMapper mapper;
        private readonly IUserPersist userPersist;

        public AccountService(UserManager<User> userManager, SignInManager<User> signInManager, IMapper mapper,IUserPersist userPersist)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.mapper = mapper;
            this.userPersist = userPersist;
        }
        public async Task<SignInResult> CheckUserPasswordAsync(UserUpdateDto userUpdateDto, string password)
        {
            try
            {
                 var user = await userManager.Users.SingleOrDefaultAsync(user => user.UserName == userUpdateDto.UserName);

                 return await signInManager.CheckPasswordSignInAsync(user, password, false);
            }
            catch (Exception ex)
            {                
                throw new Exception($"Erro ao tentar verificar o password. Erro: {ex.Message}");
            }
        }

        public async Task<UserUpdateDto> CreateAccountAsync(UserDto userDto)
        {
            try
            {
                 var user = mapper.Map<User>(userDto);
                 var result = await userManager.CreateAsync(user, userDto.Password);

                 if (result.Succeeded){                    
                    var userToReturn = mapper.Map<UserUpdateDto>(user);
                    return userToReturn;
                 }
                 return null;
            }
            catch (Exception ex)
            {                
                throw new Exception($"Erro ao tentar criar usuário. Erro: {ex.Message}");
            }
        }

        public async Task<UserUpdateDto> GetUserByUserNameAsync(string username)
        {
            try
            {
                 var user = await userPersist.GetUserByUserNameAsync(username);
                 if (user == null) return null;
                 var userUpdateDto = mapper.Map<UserUpdateDto>(user);
                 return userUpdateDto;
            }
            catch (Exception ex)
            {                
                throw new Exception($"Erro ao tentar obter o usuário por username. Erro: {ex.Message}");
            }
        }

        public async Task<UserUpdateDto> UpdateAccount(UserUpdateDto userUpdateDto)
        {
            try
            {
                var user = await userPersist.GetUserByUserNameAsync(userUpdateDto.UserName);
                if (user == null) return null;

                userUpdateDto.Id = user.Id;
                if (String.IsNullOrEmpty(userUpdateDto.imagemURL))
                    userUpdateDto.imagemURL = user.ImagemURL;

                mapper.Map(userUpdateDto, user);

                if (!String.IsNullOrEmpty(userUpdateDto.Password)){
                    var token = await userManager.GeneratePasswordResetTokenAsync(user);
                    var result = await userManager.ResetPasswordAsync(user, token, userUpdateDto.Password);
                }
                
                userPersist.Update<User>(user);

                if (await userPersist.SaveChangesAsync()){
                   var userRetorno = await userPersist.GetUserByUserNameAsync(user.UserName);
                   return mapper.Map<UserUpdateDto>(user);
                }
                
                return null;
            }
            catch (Exception ex)
            {                
                throw new Exception($"Erro ao tentar atualizar o usuário. Erro: {ex.Message}");
            }
        }

        public async Task<bool> UserExists(string userName)
        {
            try
            {
                 return await userManager.Users.AnyAsync(user => user.UserName == userName.ToLower());
            }
            catch (Exception ex)
            {                
                throw new Exception($"Erro ao tentar verificar se usuário existe. Erro: {ex.Message}");
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ProEventos.Application.Contratos;
using ProEventos.Application.DTOs;
using System.Security.Claims;
using ProEventos.API.Extensions;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using ProEventos.API.Helpers;

namespace ProEventos.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService accountService;
        private readonly ITokenService tokenService;
        private readonly IUtil util;

        private readonly string destino = "Images/Users";

        public AccountController(IAccountService accountService, ITokenService tokenService, IUtil util)
        {
            this.util = util;
            this.accountService = accountService;
            this.tokenService = tokenService;
        }

        [HttpGet("GetUser")]
        public async Task<IActionResult> GetUser()
        {
            try
            {
                var userName = User.GetUserName();
                var user = await accountService.GetUserByUserNameAsync(userName);
                return Ok(user);                 
            }
            catch (Exception ex)
            {                
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao tentar recuperar o usuário: Erro: {ex.Message}");
            }
        }

        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(UserDto userDto)
        {
            try
            {
                if (await accountService.UserExists(userDto.UserName))
                    return BadRequest("Usuário já existe");               

                var user = await accountService.CreateAccountAsync(userDto);
                if (user != null)                    
                    return Ok( new{
                        userName = user.UserName,
                        PrimeiroNome = user.PrimeiroNome,
                        token = tokenService.CreateToken(user).Result
                    });
                

                return BadRequest("Usuário não criado. Tente novamente mais tarde.");
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao tentar registrar um novo usuário. Erro: {ex.Message}");
                
            }
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserLoginDto userLogin)
        {
            try
            {
                var user = await accountService.GetUserByUserNameAsync(userLogin.UserName);
                if (user == null) return Unauthorized("Usuário e/ou senha inválidos");

                var result = await accountService.CheckUserPasswordAsync(user, userLogin.Password);
                if (!result.Succeeded)
                   return Unauthorized("Usuário e/ou senha inválidos");

                return Ok( new{
                    userName = user.UserName,
                    PrimeiroNome = user.PrimeiroNome,
                    token = tokenService.CreateToken(user).Result
                });
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao tentar realizar o login. Erro: {ex.Message}");
            }
        }

        [HttpPut("UpdateUser")]
        public async Task<IActionResult> UpdateUser(UserUpdateDto userUpdate)
        {
            try
            {
                var userName = User.GetUserName();
                //Avoid updating the wrong user by another user
                if (userUpdate.UserName != userName)
                    return Unauthorized("Usuário inválido");    

                var user = await accountService.GetUserByUserNameAsync(userName);
                if (user == null) return Unauthorized("Usuário inválido");                   

                var userReturn = await accountService.UpdateAccount(userUpdate);
                if (userReturn == null) return NoContent();

                return Ok(userReturn);

            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao tentar atualizar o usuário. Erro: {ex.Message}");
            }
        }

        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadImage() {
            try
            {
                var userName = User.GetUserName();
                var user = await accountService.GetUserByUserNameAsync(userName);
                if (user == null) return BadRequest();

                var file = Request.Form.Files[0];
                if (file.Length > 0){                    
                    util.DeleteImage(user.imagemURL, destino);                        
                    user.imagemURL = await util.SaveImage(file, destino, userName);//await SaveImage(userName, file);                                 
                }
                var userRetorno = await this.accountService.UpdateAccount(user);

                return Ok(userRetorno);
            }
            catch (Exception ex)
            {                
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro: {ex.Message}");
            }
           
        }
        
    }
}
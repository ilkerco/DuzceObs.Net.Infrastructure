using AutoMapper;
using DuzceObs.Core.Model.Entities;
using DuzceObs.WebApi.Dto;
using DuzceObs.WebApi.Helpers;
using DuzceObs.WebApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuzceObs.WebApi.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class AuthController:Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IAuthHelper _authHelper;
        private readonly IMapper _mapper;

        public AuthController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IAuthHelper authHelper,
            IMapper mapper
            )
        {
            _authHelper = authHelper;
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] InstructorRegisterDto instractorRegisterDto)
        {
            try
            {
                
                var userToCreate = _mapper.Map<Instructor>(instractorRegisterDto);
                
                userToCreate.UserName = TextHelper.TurkishCharacterToEnglish(instractorRegisterDto.FirstName.ToLower())
                    + TextHelper.TurkishCharacterToEnglish(instractorRegisterDto.LastName.ToLower()) + "81";
                var result = await _userManager.CreateAsync(userToCreate, instractorRegisterDto.Password);
                //var deneme  = _userManager.Users.Where(x => x.Id != null).ToList();
                if (result.Succeeded)
                {
                    var user =  _userManager.FindByEmailAsync(userToCreate.Email).Result;
                    return Ok(user);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        [HttpPost("register/student")]
        public async Task<IActionResult> Register([FromBody] StudentDto studentRegisterDto)
        {
            try
            {

                var userToCreate = _mapper.Map<Student>(studentRegisterDto);
                userToCreate.Email = TextHelper.TurkishCharacterToEnglish(studentRegisterDto.FirstName.ToLower()) +
                    "." + studentRegisterDto.OgrNo + "@ogr.duzce.edu.tr";
                userToCreate.UserName = studentRegisterDto.Tc+"_ogr";
                var result = await _userManager.CreateAsync(userToCreate, studentRegisterDto.OgrNo);
                //var deneme  = _userManager.Users.Where(x => x.Id != null).ToList();
                if (result.Succeeded)
                {
                    var user = _userManager.FindByEmailAsync(userToCreate.Email).Result;
                    return Ok(user);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto userLoginDto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(userLoginDto.Email);
                if (user == null)
                {
                    return Ok(new {
                        success = false,
                        message = "There is no user with this mail " + userLoginDto.Email
                    });
                }
                var loginResult = await _signInManager.CheckPasswordSignInAsync(user, userLoginDto.Password, false);
                if (!loginResult.Succeeded)
                {
                    return Ok(new
                    {
                        success = false,
                        message = "Wrong password"
                    });
                }
                var appUser = await _userManager.Users.FirstOrDefaultAsync(
                    u => u.Email == userLoginDto.Email);
                if(appUser.UserType == "Student")
                {
                    var userToReturn = _mapper.Map<StudentDto>(appUser);
                    return Ok(new
                    {
                        success=true,
                        token = _authHelper.GenerateJwtToken(appUser).Result,
                        user = userToReturn,
                    });
                }
                else
                {
                    var userToReturn = _mapper.Map<InstructorDto>(appUser);
                    return Ok(new
                    {
                        success=true,
                        token = _authHelper.GenerateJwtToken(appUser).Result,
                        user = userToReturn,
                    });
                }
                
            }
            catch (Exception ex)
            {
                return Ok(new { 
                success=false,
                message = ex.Message
                });
            }
        }
    }
}

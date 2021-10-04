using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SEDC.NoteApp2.Dto.Models;
using SEDC.NoteApp2.Dto.ValidationModels;
using SEDC.NoteApp2.Services.Interfaces;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace SEDC.NoteApp2.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserService _userService;
        private IEntityValidationService _entityValidationService;

        public UserController(IUserService userService, IEntityValidationService entityValidationService)
        {
            _userService = userService;
            _entityValidationService = entityValidationService;
        }

        [HttpGet("")]
        public ActionResult<List<UserDto>> GetAllUsers()
        {
            List<UserDto> users = _userService.GetAllUsers();
            return StatusCode(StatusCodes.Status200OK, users);
        }

        [HttpGet("all/notes")]
        public ActionResult<List<UserDto>> GetAllUsersIncludeProperties()
        {
            List<UserDto> users = _userService.GetAllUsersIncludeNotes();
            return StatusCode(StatusCodes.Status200OK, users);
        }

        [HttpGet("{id}")]
        public ActionResult<List<UserDto>> GetUserById(int id)
        {
            UserDto user = _userService.GetUserById(id);
            return StatusCode(StatusCodes.Status200OK, user);
        }

        [HttpGet("{id}/notes")]
        public ActionResult<List<UserDto>> GetUserByIdIncludeProperties(int id)
        {
            UserDto user = _userService.GetUserByIdIncludeNotes(id);
            return StatusCode(StatusCodes.Status200OK, user);
        }

        [AllowAnonymous]
        [HttpPost("")]
        public ActionResult AddUser(RegisterUserDto userDto)
        {
            ValidationResponse validationResponse = _entityValidationService.ValidateRegisterUser(userDto);

            if (validationResponse.HasError)
            {
                return StatusCode(StatusCodes.Status400BadRequest, validationResponse);
            }

            _userService.AddUser(userDto);
            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpPost("update")]
        public ActionResult UpdateUser(UserDto userDto)
        {
            _userService.UpdateUser(userDto);
            return StatusCode(StatusCodes.Status202Accepted);
        }

        [HttpDelete("{id}/delete")]
        public ActionResult DeleteUser(int id)
        {
            _userService.DeleteUser(id);
            return StatusCode(StatusCodes.Status202Accepted);
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public ActionResult<TokenDto> AuthenticateUser(LoginDto model)
        {
            TokenDto token = _userService.Authenticate(model.Username, model.Password);

            if (token == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Invalid Username Or Password");
            }

            return StatusCode(StatusCodes.Status200OK, token);
        }

        [HttpGet("whoami")]
        public ActionResult<string> WhoAmI()
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            string username = User.FindFirst(ClaimTypes.Name).Value;
            string userAddress = User.FindFirst("CustomClaimTypeUserAddress").Value;

            return StatusCode(StatusCodes.Status200OK, $"{userId} - {username} ({userAddress})");
        }

        // TODO: Add ChangeUserPassword logic
        // TODO: Additional Complexity without sending userId from QueryParams nor RequestBody
        
        public class ChangePasswordViewModel
        {
            [Required]
            public string password { get; set; }
            [Required]
            public string newPassword { get; set; }
        }
        
        [HttpPost("changePassword")]
        public ActionResult<string> ChangeUserPassword([FromBody] ChangePasswordViewModel viewModel)
        {
            if (viewModel.password == viewModel.newPassword)
                return BadRequest("The old and the new password must be different.");

            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            bool isPasswordChanged = _userService.ChangeUserPassword(userId, viewModel.password, viewModel.newPassword);
            if (isPasswordChanged) return Ok("Successfully changed password.");
            return BadRequest("Incorrect password.");
        }
    }
}

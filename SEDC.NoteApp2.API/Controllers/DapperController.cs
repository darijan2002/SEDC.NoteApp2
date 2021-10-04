using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SEDC.NoteApp2.Dto.Models;
using SEDC.NoteApp2.Dto.ValidationModels;
using SEDC.NoteApp2.Services.Interfaces;
using SEDC.NoteApp2.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SEDC.NoteApp2.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DapperController : ControllerBase
    {
        private IEntityValidationService _entityValidationService;
        private static readonly string _connectionString = "Server=.\\SQLEXPRESS;Database=Notes;Trusted_Connection=True;";

        public DapperController(IEntityValidationService entityValidationService)
        {
            _entityValidationService = entityValidationService;
        }

        [HttpGet("")]
        public ActionResult<List<UserDto>> GetAllUsers()
        {
            IDbConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            List<UserDto> userDtos =
                connection.Query<UserDto>("SELECT Id, FirstName, LastName, Username, Address, Age FROM Users")
                .Select(
                    x =>
                    {
                        x.Notes = new List<NoteDto>();
                        return x;
                    })
                .ToList();

            connection.Close();

            return StatusCode(StatusCodes.Status200OK, userDtos);
        }

        [HttpGet("{id}")]
        public ActionResult<UserDto> GetUserById(int id)
        {
            IDbConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            object parameters = new { userId = id };
            UserDto userDto = connection
                .QueryFirstOrDefault<UserDto>("SELECT Id, FirstName, LastName, Username, Address, Age FROM Users WHERE Id = @userId", parameters);

            userDto.Notes = new List<NoteDto>();

            return StatusCode(StatusCodes.Status200OK, userDto);
        }

        [HttpGet("notes")]
        public ActionResult<List<UserDto>> GetAllUsersWithNotes()
        {
            IDbConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            SqlMapper.GridReader multiQueryData = connection
                .QueryMultiple("SELECT Id, FirstName, LastName, Username, Address, Age FROM Users;" +
                "SELECT Id, Text, Color, Tag, UserId FROM Notes");

            List<UserDto> userDtos = multiQueryData.Read<UserDto>().ToList();
            List<NoteDto> noteDtos = multiQueryData.Read<NoteDto>().ToList();

            foreach (UserDto item in userDtos)
            {
                item.Notes = noteDtos
                    .Where(q => q.UserId == item.Id)
                    .Select(
                        x =>
                        {
                            x.UserFullName = $"{item.FirstName} {item.LastName}";
                            return x;
                        })
                    .ToList();
            }

            return StatusCode(StatusCodes.Status200OK, userDtos);
        }

        [HttpGet("{id}/notes")]
        public ActionResult<List<UserDto>> GetAllUsersWithNotes(int id)
        {
            IDbConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            object parameters = new { userId = id };
            SqlMapper.GridReader multiQueryData = connection
                .QueryMultiple("SELECT Id, FirstName, LastName, Username, Address, Age FROM Users WHERE Id = @userId;" +
                "SELECT Id, Text, Color, Tag, UserId FROM Notes WHERE UserId = @userId", parameters);

            UserDto userDto = multiQueryData.ReadFirstOrDefault<UserDto>();

            userDto.Notes = multiQueryData.Read<NoteDto>()
                .Select(
                x =>
                {
                    x.UserFullName = $"{userDto.FirstName} {userDto.LastName}";
                    return x;
                })
                .ToList();

            return StatusCode(StatusCodes.Status200OK, userDto);
        }

        [AllowAnonymous]
        [HttpPost("")]
        public ActionResult<int> CreateNewUser(RegisterUserDto userDto)
        {
            ValidationResponse validationResponse = _entityValidationService.ValidateRegisterUser(userDto);

            if (validationResponse.HasError)
            {
                return StatusCode(StatusCodes.Status400BadRequest, validationResponse);
            }

            IDbConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            IDbTransaction transaction = connection.BeginTransaction();
            int userId;

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@firstName", userDto.FirstName);
            parameters.Add("@lastName", userDto.LastName);
            parameters.Add("@username", userDto.Username);
            parameters.Add("@password", userDto.Password.GenerateMD5());
            parameters.Add("@address", userDto.Address);
            parameters.Add("@age", userDto.Age);
            parameters.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            try
            {
                connection.Execute("sp_CreateNewUser", parameters, transaction, 30, CommandType.StoredProcedure);
                userId = parameters.Get<int>("@id");
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }

            return StatusCode(StatusCodes.Status201Created, userId);
        }
    }
}

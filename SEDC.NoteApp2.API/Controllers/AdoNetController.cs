using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SEDC.NoteApp2.Dto.Models;
using SEDC.NoteApp2.Dto.ValidationModels;
using SEDC.NoteApp2.Services.Interfaces;
using SEDC.NoteApp2.Shared.Enums;
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
    public class AdoNetController : ControllerBase
    {
        private IEntityValidationService _entityValidationService;
        private static readonly string _connectionString = "Server=.\\SQLEXPRESS;Database=Notes;Trusted_Connection=True;";

        public AdoNetController(IEntityValidationService entityValidationService)
        {
            _entityValidationService = entityValidationService;
        }

        [HttpGet("")]
        public ActionResult<List<UserDto>> GetAllUsers()
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = "SELECT Id, FirstName, LastName, Username, Address, Age FROM Users";
            SqlDataReader dr = cmd.ExecuteReader();

            List<UserDto> userDtos = new List<UserDto>();

            while (dr.Read())
            {
                UserDto userDto = new UserDto()
                {
                    Id = dr.GetInt32(0),
                    FirstName = dr.GetFieldValue<string>(1),
                    LastName = (string)dr["LastName"],
                    Username = dr.GetString(3),
                    Address = dr.GetString(4),
                    Age = dr.GetFieldValue<int>(5),
                    Notes = new List<NoteDto>()
                };

                userDtos.Add(userDto);
            }

            return StatusCode(StatusCodes.Status200OK, userDtos);
        }

        [HttpGet("{id}")]
        public ActionResult<UserDto> GetUserById(int id)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = "SELECT Id, FirstName, LastName, Username, Address, Age FROM Users WHERE Id = @userId";
            cmd.Parameters.AddWithValue("@userId", id);

            SqlDataReader dr = cmd.ExecuteReader();

            UserDto userDto = new UserDto();

            while (dr.Read())
            {
                userDto = new UserDto()
                {
                    Id = dr.GetInt32(0),
                    FirstName = dr.GetFieldValue<string>(1),
                    LastName = (string)dr["LastName"],
                    Username = dr.GetString(3),
                    Address = dr.GetString(4),
                    Age = dr.GetFieldValue<int>(5),
                    Notes = new List<NoteDto>()
                };
            }

            return StatusCode(StatusCodes.Status200OK, userDto);
        }

        [HttpGet("notes")]
        public ActionResult<List<UserDto>> GetAllUsersWithNotes()
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = "SELECT Id, Text, Color, Tag, UserId FROM Notes";

            SqlDataReader dr = cmd.ExecuteReader();

            List<NoteDto> noteDtos = new List<NoteDto>();

            while (dr.Read())
            {
                NoteDto noteDto = new NoteDto()
                {
                    Id = dr.GetInt32(0),
                    Text = dr.GetFieldValue<string>(1),
                    Color = (string)dr["Color"],
                    Tag = dr.GetFieldValue<TagType>(3),
                    UserId = dr.GetInt32(4),
                    UserFullName = string.Empty
                };

                noteDtos.Add(noteDto);
            }

            dr.Close();

            cmd = new SqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = "SELECT Id, FirstName, LastName, Username, Address, Age FROM Users";
            dr = cmd.ExecuteReader();

            List<UserDto> userDtos = new List<UserDto>();

            while (dr.Read())
            {
                List<NoteDto> notesForUser =
                    noteDtos
                    .Where(x => x.UserId == dr.GetInt32(0))
                    .Select(
                        x =>
                        {
                            x.UserFullName = $"{dr.GetFieldValue<string>(1)} {(string)dr["LastName"]}";
                            return x;
                        })
                    .ToList();

                UserDto userDto = new UserDto()
                {
                    Id = dr.GetInt32(0),
                    FirstName = dr.GetFieldValue<string>(1),
                    LastName = (string)dr["LastName"],
                    Username = dr.GetString(3),
                    Address = dr.GetString(4),
                    Age = dr.GetFieldValue<int>(5),
                    Notes = notesForUser
                };

                userDtos.Add(userDto);
            }

            return StatusCode(StatusCodes.Status200OK, userDtos);
        }

        [HttpGet("{id}/notes")]
        public ActionResult<List<UserDto>> GetAllUsersWithNotes(int id)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = "SELECT Id, Text, Color, Tag, UserId FROM Notes WHERE UserId = @userId";
            cmd.Parameters.AddWithValue("@userId", id);

            SqlDataReader dr = cmd.ExecuteReader();

            List<NoteDto> noteDtos = new List<NoteDto>();

            while (dr.Read())
            {
                NoteDto noteDto = new NoteDto()
                {
                    Id = dr.GetInt32(0),
                    Text = dr.GetFieldValue<string>(1),
                    Color = (string)dr["Color"],
                    Tag = dr.GetFieldValue<TagType>(3),
                    UserId = dr.GetInt32(4),
                    UserFullName = string.Empty
                };

                noteDtos.Add(noteDto);
            }

            dr.Close();

            cmd = new SqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = "SELECT Id, FirstName, LastName, Username, Address, Age FROM Users WHERE Id = @userId";
            cmd.Parameters.AddWithValue("@userId", id);
            dr = cmd.ExecuteReader();

            UserDto userDto = new UserDto();

            while (dr.Read())
            {
                userDto = new UserDto()
                {
                    Id = dr.GetInt32(0),
                    FirstName = dr.GetFieldValue<string>(1),
                    LastName = (string)dr["LastName"],
                    Username = dr.GetString(3),
                    Address = dr.GetString(4),
                    Age = dr.GetFieldValue<int>(5),
                    Notes = noteDtos
                    .Select(
                        x =>
                        {
                            x.UserFullName = $"{dr.GetFieldValue<string>(1)} {(string)dr["LastName"]}";
                            return x;
                        })
                    .ToList()
                };
            }

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

            SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            SqlTransaction transaction = connection.BeginTransaction();

            int userId;

            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = connection;
                cmd.Transaction = transaction;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "sp_CreateNewUser";
                cmd.Parameters.AddWithValue("@firstName", userDto.FirstName);
                cmd.Parameters.AddWithValue("@lastName", userDto.LastName);
                cmd.Parameters.AddWithValue("@username", userDto.Username);
                cmd.Parameters.AddWithValue("@password", userDto.Password.GenerateMD5());
                cmd.Parameters.AddWithValue("@address", userDto.Address);
                cmd.Parameters.AddWithValue("@age", userDto.Age);

                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@id",
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Output,
                    Value = 0
                });

                cmd.ExecuteNonQuery();

                userId = (int)cmd.Parameters["@id"].Value;

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

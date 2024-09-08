using Microsoft.AspNetCore.Mvc;
using Npgsql;
using NpgsqlTypes;
using SpliterX_API.DataAccess;
using SpliterX_API.Models;
using System;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

[Route("SpliterX/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly string _connectionString;

    public UserController(string connectionString)
    {
        _connectionString = connectionString;
    }

    [HttpPost("login")]
    public ActionResult Login([FromBody] LoginRequest loginRequest)
    {
        DateTime requestTime = TimeZoneIST.now();

        try
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                Console.WriteLine(_connectionString);
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT * FROM USER_LOGIN(@in_phonenumber,@in_password);", connection))
                {
                    command.Parameters.AddWithValue("in_phonenumber", loginRequest.PhoneNumberOrEmail);
                    command.Parameters.AddWithValue("in_password", loginRequest.Password);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (reader.GetBoolean(0))
                            {
                                return Ok(new
                                {
                                    success = true,
                                    header = new { requestTime, responseTime = TimeZoneIST.now() },
                                    body = new
                                    {
                                        message = reader.GetString(1),
                                        userId = reader.IsDBNull(2) ? (long?)null : reader.GetInt64(2)
                                    }
                                });
                            }
                            else
                            {
                                return Ok(new
                                {
                                    success = false,
                                    header = new { requestTime, responseTime = TimeZoneIST.now() },
                                    body = new
                                    {
                                        message = reader.GetString(1)
                                    }
                                });
                            }
                        }
                    }
                }
            }
            return Ok(new
            {
                success = false,
                header = new { requestTime, responseTime = TimeZoneIST.now() },
                body = new { message = "Something went wrong. Please try again later." }
            });
        }
        catch (NpgsqlException)
        {
            return Ok(new
            {
                success = false,
                header = new { requestTime, responseTime = TimeZoneIST.now() },
                body = new { message = "Something went wrong. Please try again later." }
            });
        }
        catch (Exception)
        {
            return Ok(new
            {
                success = false,
                header = new { requestTime, responseTime = TimeZoneIST.now() },
                body = new { message = "Something went wrong. Please try again later." }
            });
        }
    }

    [HttpPost("signup")]
    public ActionResult Signup([FromBody] SignupRequest signupRequest)
    {
        DateTime requestTime = TimeZoneIST.now();
        signupRequest.BirthDate = signupRequest.BirthDate.Split('T')[0];
        try
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT * FROM USER_SIGNUP(@in_phonenumber, @in_firstname, @in_lastname, @in_email, @in_birthdate, @in_gender, @in_password);", connection))
                {
                    command.Parameters.AddWithValue("in_phonenumber", signupRequest.PhoneNumber);
                    command.Parameters.AddWithValue("in_firstname", signupRequest.FirstName);
                    command.Parameters.AddWithValue("in_lastname", signupRequest.LastName);
                    command.Parameters.AddWithValue("in_email", signupRequest.Email);
                    command.Parameters.Add(new NpgsqlParameter("in_birthdate", NpgsqlDbType.Date) { Value = DateOnly.Parse(signupRequest.BirthDate) });
                    command.Parameters.AddWithValue("in_gender", signupRequest.Gender);
                    command.Parameters.AddWithValue("in_password", signupRequest.Password);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (reader.GetBoolean(0))
                            {
                                return Ok(new
                                {
                                    success = true,
                                    header = new { requestTime, responseTime = TimeZoneIST.now() },
                                    body = new
                                    {
                                        message = reader.GetString(1),
                                        userId = reader.IsDBNull(2) ? (long?)null : reader.GetInt64(2)
                                    }
                                });
                            }
                            else
                            {
                                return Ok(new
                                {
                                    success = false,
                                    header = new { requestTime, responseTime = TimeZoneIST.now() },
                                    body = new
                                    {
                                        message = reader.GetString(1)
                                    }
                                });
                            }
                        }
                    }
                }
                return Ok(new
                {
                    success = false,
                    header = new { requestTime, responseTime = TimeZoneIST.now() },
                    body = new { message = "Something went wrong. Please try again later." }
                });
            }
        }
        catch (NpgsqlException)
        {
            return Ok(new
            {
                success = false,
                header = new { requestTime, responseTime = TimeZoneIST.now() },
                body = new { message = "Something went wrong. Please try again later." }
            });
        }
        catch (Exception)
        {
            return Ok(new
            {
                success = false,
                header = new { requestTime, responseTime = TimeZoneIST.now() },
                body = new { message = "Something went wrong. Please try again later." }
            });
        }
    }

    [HttpGet("{id}")]
    public ActionResult GetUserDetails(long id)
    {
        DateTime requestTime = TimeZoneIST.now();
        UserDetails userDetails = new UserDetails();
        try
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT * FROM USER_FETCHDETAILS", conn))
                {
                    cmd.Parameters.AddWithValue("in_userid", id);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            userDetails = new UserDetails
                            {
                                Id = reader.GetInt64(0),
                                PhoneNumber = reader.GetString(1),
                                FirstName = reader.GetString(2),
                                LastName = reader.GetString(3),
                                Email = reader.GetString(4),
                                BirthDate = reader.IsDBNull(5) ? (DateTime?)null : reader.GetDateTime(5),
                                Gender = reader.GetString(6)
                            };
                            return Ok(new
                            {
                                success = true,
                                header = new { requestTime, responseTime = TimeZoneIST.now() },
                                body = new { data = userDetails }
                            });
                        }
                        else
                        {
                            return Ok(new
                            {
                                success = false,
                                header = new { requestTime, responseTime = TimeZoneIST.now() },
                                body = new { message = "User details is unavailable" }
                            });
                        }
                    }
                }
            }
        }
        catch (NpgsqlException)
        {
            return Ok(new
            {
                success = false,
                header = new { requestTime, responseTime = TimeZoneIST.now() },
                body = new { message = "Something went wrong. Please try again later." }
            });
        }
        catch (Exception)
        {
            return Ok(new
            {
                success = false,
                header = new { requestTime, responseTime = TimeZoneIST.now() },
                body = new { message = "Something went wrong. Please try again later." }
            });
        }
    }

    [HttpPut]
    public ActionResult UpdateUserDetails([FromBody] UserUpdateModel model)
    {
        DateTime requestTime = TimeZoneIST.now();
        try
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand("SELECT * FROM USER_UPDATE", conn))
                {
                    cmd.Parameters.AddWithValue("in_userid", model.UserId);
                    cmd.Parameters.AddWithValue("in_phonenumber", (object)model.PhoneNumber ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("in_firstname", (object)model.FirstName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("in_lastname", (object)model.LastName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("in_email", (object)model.Email ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("in_birthdate", (object)model.BirthDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("in_gender", (object)model.Gender ?? DBNull.Value);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return Ok(new
                            {
                                success = reader.GetBoolean(0),
                                header = new { requestTime, responseTime = TimeZoneIST.now() },
                                body = new { message = reader.GetString(1) }
                            });
                        }
                        else
                        {
                            return Ok(new
                            {
                                success = false,
                                header = new { requestTime, responseTime = TimeZoneIST.now() },
                                body = new { message = "Something went wrong. Please try again later." }
                            });
                        }
                    }
                }
            }
        }
        catch (NpgsqlException)
        {
            return Ok(new
            {
                success = false,
                header = new { requestTime, responseTime = TimeZoneIST.now() },
                body = new { message = "Something went wrong. Please try again later." }
            });
        }
        catch (Exception)
        {
            return Ok(new
            {
                success = false,
                header = new { requestTime, responseTime = TimeZoneIST.now() },
                body = new { message = "Something went wrong. Please try again later." }
            });
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Npgsql;
using SpliterX_API.DataAccess;
using SpliterX_API.Models;
using System;

[Route("SpliterX/[controller]")]
[ApiController]
public class MemberController : ControllerBase
{
    private readonly string _connectionString;

    public MemberController(string connectionString)
    {
        _connectionString = connectionString;
    }

    [HttpGet("fetchall/{roomId}")]
    public ActionResult FetchAllMembers(long roomId)
    {
        DateTime requestTime = TimeZoneIST.now();

        try
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT * FROM members_fetchall(@in_roomid);", connection))
                {
                    command.Parameters.AddWithValue("in_roomid", roomId);

                    using (var reader = command.ExecuteReader())
                    {
                        var members = new List<MemberResponse>();

                        while (reader.Read())
                        {
                            members.Add(new MemberResponse
                            {
                                Id = reader.GetInt64(0),
                                Name = reader.GetString(1),
                                PhoneNumber = reader.GetString(2)
                            });
                        }

                        return Ok(new
                        {
                            success = true,
                            header = new { requestTime, responseTime = TimeZoneIST.now() },
                            body = new { data=members }
                        });
                    }
                }
            }
            return Ok(new
            {
                success = false,
                header = new { requestTime, responseTime = TimeZoneIST.now() },
                body = new { message = "No members found or failed to retrieve members." }
            });
        }
        catch (NpgsqlException)
        {
            return Ok(new
            {
                success = false,
                header = new { requestTime, responseTime = TimeZoneIST.now() },
                body = new { message = "Database error occurred. Please try again later." }
            });
        }
        catch (Exception)
        {
            return Ok(new
            {
                success = false,
                header = new { requestTime, responseTime = TimeZoneIST.now() },
                body = new { message = "An error occurred. Please try again later." }
            });
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Npgsql;
using SpliterX_API.DataAccess;
using SpliterX_API.Models;
using System;
using System.Collections.Generic;

[Route("SpliterX/[controller]")]
[ApiController]
public class GroupController : ControllerBase
{
    private readonly string _connectionString;

    public GroupController(string connectionString)
    {
        _connectionString = connectionString;
    }

    [HttpPost("creategroup")]
    public ActionResult CreateGroup([FromForm] GroupCreateRequest groupCreateRequest)
    {
        DateTime requestTime = TimeZoneIST.now();

        try
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT * FROM groups_create(@in_roomid, @in_name, @in_description);", connection))
                {
                    command.Parameters.AddWithValue("in_roomid", groupCreateRequest.RoomId);
                    command.Parameters.AddWithValue("in_name", groupCreateRequest.Name);
                    command.Parameters.AddWithValue("in_description", groupCreateRequest.Description);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var response = new ResponseModel
                            {
                                Success = reader.GetBoolean(0),
                                Message = reader.GetString(1)
                            };

                            return Ok(new
                            {
                                success = response.Success,
                                header = new { requestTime, responseTime = TimeZoneIST.now() },
                                body = new { message = response.Message }
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

    [HttpGet("fetchallgroups/{roomId}")]
    public ActionResult FetchAllGroups(long roomId)
    {
        DateTime requestTime = TimeZoneIST.now();

        try
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT * FROM groups_fetchall(@in_roomid);", connection))
                {
                    command.Parameters.AddWithValue("in_roomid", roomId);

                    using (var reader = command.ExecuteReader())
                    {
                        var groups = new List<GroupFetchAllResponse>();
                        while (reader.Read())
                        {
                            groups.Add(new GroupFetchAllResponse
                            {
                                Id = reader.GetInt64(0),
                                Name = reader.GetString(1),
                                Description = reader.GetString(2),
                                StartDate = DateOnly.FromDateTime(reader.GetDateTime(3)),
                                EndDate = reader.IsDBNull(4) ? (DateOnly?)null : DateOnly.FromDateTime(reader.GetDateTime(4)),
                                Status = reader.GetBoolean(5),
                                CreatedOn = reader.GetDateTime(6),
                                TotalTransactions = reader.GetInt64(7)
                            });
                        }

                        return Ok(new
                        {
                            success = true,
                            header = new { requestTime, responseTime = TimeZoneIST.now() },
                            body = new
                            {
                                data = groups
                            }
                        });
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

    [HttpPut("update")]
    public ActionResult UpdateGroup(
        [FromQuery] long id,
        [FromQuery] string name,
        [FromQuery] string description,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] long? roomId,
        [FromQuery] bool? status)
    {
        DateTime requestTime = TimeZoneIST.now();

        try
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT * FROM groups_update(@in_id, @in_name, @in_description, @in_startdate, @in_enddate, @in_roomid, @in_status);", connection))
                {
                    command.Parameters.AddWithValue("in_id", id);
                    command.Parameters.AddWithValue("in_name", (object)name ?? DBNull.Value);
                    command.Parameters.AddWithValue("in_description", (object)description ?? DBNull.Value);
                    command.Parameters.AddWithValue("in_startdate", (object)startDate ?? DBNull.Value);
                    command.Parameters.AddWithValue("in_enddate", (object)endDate ?? DBNull.Value);
                    command.Parameters.AddWithValue("in_roomid", (object)roomId ?? DBNull.Value);
                    command.Parameters.AddWithValue("in_status", (object)status ?? DBNull.Value);

                    using (var reader = command.ExecuteReader())
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
                    }
                }
            }
            return Ok(new
            {
                success = false,
                header = new { requestTime, responseTime = TimeZoneIST.now() },
                body = new { message = "Group update failed. Please try again later." }
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

    [HttpDelete("delete/{groupId}")]
    public ActionResult DeleteGroup(long groupId)
    {
        DateTime requestTime = TimeZoneIST.now();

        try
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT * FROM group_delete(@in_id);", connection))
                {
                    command.Parameters.AddWithValue("in_id", groupId);

                    using (var reader = command.ExecuteReader())
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
                    }
                }
            }
            return Ok(new
            {
                success = false,
                header = new { requestTime, responseTime = TimeZoneIST.now() },
                body = new { message = "Group deletion failed. Please try again later." }
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


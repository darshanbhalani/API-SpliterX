using Microsoft.AspNetCore.Mvc;
using Npgsql;
using SpliterX_API.DataAccess;
using SpliterX_API.Models;

[Route("SpliterX/[controller]")]
[ApiController]
public class RoomController : ControllerBase
{
    private readonly string _connectionString;

    public RoomController(string connectionString)
    {
        _connectionString = connectionString;
    }

    [HttpPost("createroom")]
    public ActionResult CreateRoom([FromBody] RoomCreateRequest roomCreateRequest)
    {
        DateTime requestTime = TimeZoneIST.now();

        try
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT * FROM room_create(@in_adminid, @in_name, @in_description);", connection))
                {
                    command.Parameters.AddWithValue("in_adminid", roomCreateRequest.AdminId);
                    command.Parameters.AddWithValue("in_name", roomCreateRequest.Name);
                    command.Parameters.AddWithValue("in_description", roomCreateRequest.Description);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var response = new RoomCreateResponse
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

    [HttpGet("fetchallrooms/{userId}")]
    public ActionResult FetchAllRooms(long userId)
    {
        DateTime requestTime = TimeZoneIST.now();

        try
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT * FROM room_fetchall(@in_id);", connection))
                {
                    command.Parameters.AddWithValue("in_id", userId);

                    using (var reader = command.ExecuteReader())
                    {
                        var rooms = new List<RoomFetchAllResponse>();
                        while (reader.Read())
                        {
                            rooms.Add(new RoomFetchAllResponse
                            {
                                Id = reader.GetInt64(0),
                                Name = reader.GetString(1),
                                Description = reader.GetString(2),
                                AdminName = reader.GetString(3),
                                AdminId = reader.GetInt64(4),
                                CreatedOn = reader.GetDateTime(5),
                                TotalMembers = reader.GetInt64(6)
                            });
                        }

                        return Ok(new
                        {
                            success = true,
                            header = new { requestTime, responseTime = TimeZoneIST.now() },
                            body = rooms
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

    [HttpDelete("delete/{roomId}")]
    public ActionResult DeleteRoom(long roomId)
    {
        DateTime requestTime = TimeZoneIST.now();

        try
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT * FROM room_delete(@in_id);", connection))
                {
                    command.Parameters.AddWithValue("in_id", roomId);

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
                body = new { message = "Room deletion failed. Please try again later." }
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

    [HttpPut("update")]
    public ActionResult UpdateRoom(RoomUpdateRequest roomUpdateRequest)
    {
        DateTime requestTime = TimeZoneIST.now();

        try
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT * FROM room_update(@in_id, @in_roomname, @in_description, @in_adminid);", connection))
                {
                    command.Parameters.AddWithValue("in_id", roomUpdateRequest.RoomId);
                    command.Parameters.AddWithValue("in_roomname", string.IsNullOrEmpty(roomUpdateRequest.Name) ? (object)DBNull.Value : roomUpdateRequest.Name);
                    command.Parameters.AddWithValue("in_description", string.IsNullOrEmpty(roomUpdateRequest.Description) ? (object)DBNull.Value : roomUpdateRequest.Description);
                    command.Parameters.AddWithValue("in_adminid", roomUpdateRequest.AdminId);

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
                body = new { message = "Room update failed. Please try again later." }
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

    [HttpPost("removemember")]
    public ActionResult RemoveMember([FromBody] RoomRemoveMemberRequest roomRemoveMemberRequest)
    {
        DateTime requestTime = TimeZoneIST.now();

        try
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT * FROM room_removemember(@in_id, @in_roomid);", connection))
                {
                    command.Parameters.AddWithValue("in_id", roomRemoveMemberRequest.UserId);
                    command.Parameters.AddWithValue("in_roomid", roomRemoveMemberRequest.RoomId);

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

    [HttpPost("addmember")]
    public ActionResult AddMember([FromBody] RoomAddMemberRequest roomAddMemberRequest)
    {
        DateTime requestTime = TimeZoneIST.now();

        try
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT * FROM room_addmember(@in_id, @in_roomid);", connection))
                {
                    command.Parameters.AddWithValue("in_id", roomAddMemberRequest.UserId);
                    command.Parameters.AddWithValue("in_roomid", roomAddMemberRequest.RoomId);

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

    [HttpPut("changeadmin")]
    public ActionResult ChangeAdmin(long adminId, long roomId)
    {
        DateTime requestTime = TimeZoneIST.now();

        try
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT * FROM room_changeadmin(@in_id, @in_roomid);", connection))
                {
                    command.Parameters.AddWithValue("in_id", adminId);
                    command.Parameters.AddWithValue("in_roomid", roomId);

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
                body = new { message = "Admin change failed. Please try again later." }
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

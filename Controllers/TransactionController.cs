using Microsoft.AspNetCore.Mvc;
using Npgsql;
using SpliterX_API.DataAccess;
using SpliterX_API.Models;
using System;
using System.Reflection;
using System.Transactions;

[Route("SpliterX/[controller]")]
[ApiController]
public class TransactionController : ControllerBase
{
    private readonly string _connectionString;

    public TransactionController(string connectionString)
    {
        _connectionString = connectionString;
    }

    [HttpPost("createtransaction")]
    public ActionResult CreateTransaction([FromBody] TransactionCreateRequest transactionCreateRequest)
    {
        DateTime requestTime = TimeZoneIST.now();

        try
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT * FROM TRANSACTION_CREATE(@in_groupid, @in_description, @in_amount, @in_transactiontime, @in_paymentmode, @in_createdby, @in_memberids, @in_spent, @in_expand);", connection))
                {
                    command.Parameters.AddWithValue("in_groupid", transactionCreateRequest.GroupId);
                    command.Parameters.AddWithValue("in_description", transactionCreateRequest.Description);
                    command.Parameters.AddWithValue("in_amount", transactionCreateRequest.Amount);
                    command.Parameters.AddWithValue("in_transactiontime", transactionCreateRequest.TransactionTime);
                    command.Parameters.AddWithValue("in_paymentmode", transactionCreateRequest.PaymentMode);
                    command.Parameters.AddWithValue("in_createdby", transactionCreateRequest.CreatedBy);
                    command.Parameters.AddWithValue("in_memberids", transactionCreateRequest.MemberIds);
                    command.Parameters.AddWithValue("in_spent", transactionCreateRequest.Spent);
                    command.Parameters.AddWithValue("in_expand", transactionCreateRequest.Expand);

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

    [HttpPut("update")]
    public ActionResult UpdateTransaction(TransactionUpdateRequest transactionUpdateRequest)
    {
        DateTime requestTime = TimeZoneIST.now();

        try
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT * FROM transaction_update(@in_id, @in_description, @in_amount, @in_paymentmode, @in_transactiontime, @in_updatedby);", connection))
                {
                    command.Parameters.AddWithValue("in_id", transactionUpdateRequest.Id);
                    command.Parameters.AddWithValue("in_description", transactionUpdateRequest.Description ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("in_amount", transactionUpdateRequest.Amount);
                    command.Parameters.AddWithValue("in_paymentmode", transactionUpdateRequest.PaymentMode);
                    command.Parameters.AddWithValue("in_transactiontime", transactionUpdateRequest.TransactionTime);
                    command.Parameters.AddWithValue("in_updatedby", transactionUpdateRequest.UpdatedBy);

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
                body = new { message = "Transaction update failed. Please try again later." }
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

    [HttpDelete("delete")]
    public ActionResult DeleteTransaction([FromQuery] long id)
    {
        DateTime requestTime = TimeZoneIST.now();

        try
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT * FROM transaction_delete(@in_id);", connection))
                {
                    command.Parameters.AddWithValue("in_id", id);

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
                body = new { message = "Transaction deletion failed. Please try again later." }
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

    [HttpGet("fetchallbyuserid/{userId}")]
    public ActionResult FetchAllByUserId(long userId)
    {
        DateTime requestTime = TimeZoneIST.now();
        List<TransactionFetchAllByUserIdResponseModel>  transactions = new List<TransactionFetchAllByUserIdResponseModel>();
        try
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT * FROM TRANSACTION_FETCHALLBYUSERID(@in_id);", connection))
                {
                    command.Parameters.AddWithValue("in_id", userId);

                    using (var reader = command.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            transactions.Add(new TransactionFetchAllByUserIdResponseModel
                            {
                                Id = reader.GetInt64(0),
                                Description = reader.GetString(1),
                                RoomName = reader.GetString(2),
                                GroupName = reader.GetString(3),
                                Amount = reader.GetInt32(4),
                                Spent = reader.GetInt32(5),
                                Expand = reader.GetInt32(6),
                                PaymentMode = reader.GetString(7),
                                TransactionTime = reader.GetDateTime(8),
                                CreatedOn = reader.GetDateTime(9)
                            });
                        }
                    }
                }
            }
            return Ok(new
            {
                success = true,
                header = new { requestTime, responseTime = TimeZoneIST.now() },
                body = new { data = transactions }
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

    [HttpGet("fetchallbygroupid/{groupId}")]
    public async Task<ActionResult<IEnumerable<TransactionFetchAllByGroupIdResponseModel>>> FetchAllByGroupId(long groupId)
    {
        DateTime requestTime = TimeZoneIST.now();

        List<TransactionFetchAllByGroupIdResponseModel> transactions = new List<TransactionFetchAllByGroupIdResponseModel>();
        try
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand("SELECT * FROM transaction_fetchallbygroupid(@in_groupid);", connection))
                {
                    command.Parameters.AddWithValue("in_groupid", groupId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            transactions.Add(new TransactionFetchAllByGroupIdResponseModel
                            {
                                Id = reader.GetInt64(0),
                                Description = reader.GetString(1),
                                Amount = reader.GetInt32(2),
                                PaymentMode = reader.GetString(3),
                                TransactionTime = reader.GetDateTime(4),
                                CreatedOn = reader.GetDateTime(5),
                                IsDeleted = reader.GetBoolean(6)
                            });
                        }
                    }
                }
            }
            return Ok(new
            {
                success = true,
                header = new { requestTime, responseTime = TimeZoneIST.now() },
                body = new { data = transactions }
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

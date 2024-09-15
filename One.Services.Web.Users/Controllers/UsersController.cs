using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using One.Database.Users;
using One.Database.Users.DbContext;
using One.Database.Users.Models;
using One.Models.Users;
using One.Models.Users.Payloads;
using One.Services.Web.Users.BL;
using User = One.Database.Users.Models.User;

namespace One.Services.Web.Users.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController(UserDbContext userDbContext, IMapper mapper, ILogger<UsersController> logger) : ControllerBase
    {
        /// <summary>
        /// Get list of all users 
        /// </summary> 
        /// <returns>List of all users if successful, if not it returns the error </returns>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                logger.LogInformation($"get full user list at {DateTime.UtcNow}");

                var users =await  userDbContext.Users.ToListAsync();
                if (users.Count == 0)
                {
                    logger.LogInformation($"no users found at {DateTime.UtcNow}");
                    return NotFound(new ApiResponse<object>
                    {
                        Status = "error",
                        Error = new ApiError
                        {
                            Code = "NOT_FOUND",
                            Message = "Users not found"
                        }
                    });
                }
                logger.LogInformation($"{users.Count} users found at {DateTime.UtcNow}");
                return Ok(new ApiResponse<List<Models.Users.User>>
                {
                    Status = "success",
                    Data = mapper.Map<List<Models.Users.User>>(users),
                    Message = "User list retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while processing the get user list request.");

                return StatusCode(500, new ApiResponse<object>
                {
                    Status = "error",
                    Error = new ApiError
                    {
                        Code = "INTERNAL_SERVER_ERROR",
                        Message = "An error occurred while retrieving users."
                    }
                });
            }
        }

        /// <summary>
        /// Get one individual user by id 
        /// </summary> 
        /// <returns>One individual user by id if successful, if not it returns the error  </returns>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {

            try
            {
                logger.LogInformation($"get user by id ={id} at {DateTime.UtcNow}");
                var user =await userDbContext.Users.Where(u => u.Id == id).ToListAsync();
                if (user.Count == 0)
                {
                    logger.LogInformation($"user with  id ={id} not found at {DateTime.UtcNow}");
                    return NotFound(new ApiResponse<object>
                    {
                        Status = "error",
                        Error = new ApiError
                        {
                            Code = "NOT_FOUND",
                            Message = "User not found"
                        }
                    });
                }
                logger.LogInformation($"user {user[0].FirstName} with  id ={id} found at {DateTime.UtcNow}");
                return Ok(new ApiResponse<Models.Users.User>
                {
                    Status = "success",
                    Data = mapper.Map<Models.Users.User>(user[0]),
                    Message = "User  retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while processing the get user by id request.");

                return StatusCode(500, new ApiResponse<object>
                {
                    Status = "error",
                    Error = new ApiError
                    {
                        Code = "INTERNAL_SERVER_ERROR",
                        Message = "An error occurred while retrieving user by id."
                    }
                });
            }

        }

        /// <summary>
        /// Get one individual user by email 
        /// </summary> 
        /// <returns>One individual user by email if successful, if not it returns the error  </returns>
        [HttpGet("{email}")]
        public async Task<IActionResult> Get(string email)
        {
            try
            {
                logger.LogInformation($"get user by email at {DateTime.UtcNow}");
                var user =await userDbContext.Users.Where(u => u.Email.ToLower() == email.ToLower()).ToListAsync();
                if (user.Count == 0)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Status = "error",
                        Error = new ApiError
                        {
                            Code = "NOT_FOUND",
                            Message = "User not found"
                        }
                    });
                }

                return Ok(new ApiResponse<Models.Users.User>
                {
                    Status = "success",
                    Data = mapper.Map<Models.Users.User>(user.FirstOrDefault()),
                    Message = "User  retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while processing the get user by email request.");
                return StatusCode(500, new ApiResponse<object>
                {
                    Status = "error",
                    Error = new ApiError
                    {
                        Code = "INTERNAL_SERVER_ERROR",
                        Message = "An error occurred while retrieving user by email."
                    }
                });
            }

        }

        /// <summary>
        /// Create new user
        /// </summary> 
        /// <returns>The information of the new user, if successful, if not it returns the error </returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Models.Users.Payloads.UserPayload user)
        {
            logger.LogInformation($"Create new user at {DateTime.UtcNow}");

            if (!ModelState.IsValid)
            {
                // Extract validation errors
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new { Errors = errors });
            }
            try
            {
                var dbUser = mapper.Map<User>(user);

                userDbContext.Users.Add(dbUser);
                await userDbContext.SaveChangesAsync();


                return CreatedAtAction(nameof(Post), new { id = dbUser.Id }, new ApiResponse<Models.Users.User>
                {
                    Status = "success",
                    Data = mapper.Map<Models.Users.User>(dbUser),
                    Message = "User created successfully"
                });
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("unique index") == true)
            {
                logger.LogError(ex, $"Cannot create new user  as there is already another user with the same email {SensitiveInfoMaskHelper.MaskEmail(user.Email)}.");

                // Handle unique constraint violation
                return Conflict(new ApiResponse<object>
                {
                    Status = "error",
                    Error = new ApiError
                    {
                        Code = "CONFLICT",
                        Message = "A user with this email already exists."
                    }
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while processing the add new user request.");

                return StatusCode(500, new ApiResponse<object>
                {
                    Status = "error",
                    Error = new ApiError
                    {
                        Code = "INTERNAL_SERVER_ERROR",
                        Message = "An error occurred while adding a new user."
                    }
                });
            }



        }

        /// <summary>
        /// Update an existing user  
        /// </summary> 
        /// <returns>The information of the updated user, if successful, if not it returns the error </returns>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] Models.Users.Payloads.UserPayload user)
        {
            logger.LogInformation($"Update new user at {DateTime.UtcNow}");
            if (!ModelState.IsValid)
            {
                // Extract validation errors
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new { Errors = errors });
            }
            try
            {

                var dbUser = userDbContext.Users.Where(u => u.Id == id).ToList();
                if (dbUser.Count == 0)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Status = "error",
                        Error = new ApiError
                        {
                            Code = "NOT_FOUND",
                            Message = "User not found"
                        }
                    });
                }

                dbUser[0].FirstName = user.FirstName;
                dbUser[0].LastName = user.LastName;
                dbUser[0].Email = user.Email;
                dbUser[0].PhoneNumber = user.PhoneNumber;
                dbUser[0].DateOfBirth = user.DateOfBirth;

                userDbContext.Users.Entry(dbUser[0]).State = EntityState.Modified;
                await userDbContext.SaveChangesAsync();


                return CreatedAtAction(nameof(Put), new { id = dbUser[0].Id }, new ApiResponse<Models.Users.User>
                {
                    Status = "success",
                    Data = mapper.Map<Models.Users.User>(dbUser[0]),
                    Message = "User updated successfully"
                });
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("unique index") == true)
            {
                logger.LogError(ex, $"Cannot update the email of this user as there is already another user with the same email  {SensitiveInfoMaskHelper.MaskEmail(user.Email)}.");
                // Handle unique constraint violation
                return Conflict(new ApiResponse<object>
                {
                    Status = "error",
                    Error = new ApiError
                    {
                        Code = "CONFLICT",
                        Message = "A user with this email already exists."
                    }
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while processing the update user request.");

                return StatusCode(500, new ApiResponse<object>
                {
                    Status = "error",
                    Error = new ApiError
                    {
                        Code = "INTERNAL_SERVER_ERROR",
                        Message = "An error occurred while updating a user."
                    }
                });
            }
        }

        /// <summary>
        /// Deletes a user
        /// </summary> 
        /// <returns> the id of the deleted user, or an error if delete was not successful  </returns>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            logger.LogInformation($"Delete user at {DateTime.UtcNow}");

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var dbUser = userDbContext.Users.Where(u => u.Id == id).ToList();
                if (dbUser.Count == 0)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Status = "error",
                        Error = new ApiError
                        {
                            Code = "NOT_FOUND",
                            Message = "User not found"
                        }
                    });
                }

                userDbContext.Users.Remove(dbUser[0]);
                await userDbContext.SaveChangesAsync();


                return CreatedAtAction(nameof(Delete), new { id = id }, new ApiResponse<int>
                {
                    Status = "success",
                    Data = id,
                    Message = "User deleted successfully"
                });
            }

            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while processing the delete user request.");

                return StatusCode(500, new ApiResponse<object>
                {
                    Status = "error",
                    Error = new ApiError
                    {
                        Code = "INTERNAL_SERVER_ERROR",
                        Message = "An error occurred while deleting an user."
                    }
                });
            }
        }
    }
}

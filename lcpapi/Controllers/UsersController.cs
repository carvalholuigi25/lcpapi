using Microsoft.AspNetCore.Mvc;
using lcpapi.Models;
using lcpapi.Interfaces;
using lcpapi.Authorization;
using lcpapi.Models.QParams;

namespace lcpapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUsersRepo _usersRepo;

        public UsersController(IUsersRepo usersRepo)
        {
            _usersRepo = usersRepo;
        }

        /// <summary>
        /// Gets all users infos.
        /// </summary>
        /// <param name="queryParams"></param>
        /// <returns>Gets all users infos</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/users
        ///     {
        ///         [
        ///             {
        ///                 "id": 0,
        ///                 "username": "string",
        ///                 "password": "string",
        ///                 "displayName": "string",
        ///                 "role": "guest"
        ///             }
        ///         ]
        ///     }
        ///
        /// </remarks>
        /// <response code="201">Returns the all infos about users</response>
        /// <response code="400">If the users infos are empty</response>
        [HttpGet]
        [Authorize(Policy = "AllUsers")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers([FromQuery] QueryParams queryParams)
        {
            var users = await _usersRepo.GetUsers(queryParams);
            var totalCount = await _usersRepo.GetTotalCountAsync(queryParams);
            var response = new QueryParamsResp<User> {
                TotalCount = totalCount,
                Page = queryParams.Page,
                PageSize = queryParams.PageSize,
                Data = users.Value!.ToList()
            };

            return Ok(response);
        }

        /// <summary>
        /// Gets user info by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Gets user info by id</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/users
        ///     {
        ///         "id": 0,
        ///         "username": "string",
        ///         "password": "string",
        ///         "displayName": "string",
        ///         "role": "guest"
        ///     }
        ///
        /// </remarks>
        /// <response code="201">Returns the info about user</response>
        /// <response code="400">If the user info is empty</response>
        [HttpGet("{id}")]
        [Authorize(Policy = "StaffOnly")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<User>> GetUser(int? id)
        {
            return await _usersRepo.GetUser(id);
        }
        
        /// <summary>
        /// Creates a user.
        /// </summary>
        /// <param name="user"></param>
        /// <returns>A newly created user</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/users
        ///     {
        ///         "id": 0,
        ///         "username": "string",
        ///         "password": "string",
        ///         "displayName": "string",
        ///         "role": "guest"
        ///     }
        ///
        /// </remarks>
        /// <response code="201">Returns the newly created user info</response>
        /// <response code="400">If the user info is empty</response>
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            return await _usersRepo.PostUser(user);
        }

        /// <summary>
        /// Updates specific user info by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="user"></param>
        /// <returns>Updates specific user info by id</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /api/users
        ///     {
        ///         "id": 0,
        ///         "username": "string",
        ///         "password": "string",
        ///         "displayName": "string",
        ///         "role": "guest"
        ///     }
        ///
        /// </remarks>
        /// <response code="201">Returns the all users infos updated by id and its body</response>
        /// <response code="400">If the users infos updated are empty by id and its body</response>
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Policy = "StaffOnly")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PutUser(int? id, User user)
        {
            return await _usersRepo.PutUser(id, user);
        }

        /// <summary>
        /// Deletes specific user info by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Deletes the specific user info by id</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /api/users/{id}
        ///
        /// </remarks>
        /// <response code="201">Returns the all users infos are deleted by id</response>
        /// <response code="400">If the users infos are deleted by id</response>
        [HttpDelete("{id}")]
        [Authorize(Policy = "StaffOnly")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteUser(int? id)
        {
            return await _usersRepo.DeleteUser(id);
        }
    }
}

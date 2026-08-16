using Microsoft.AspNetCore.Mvc;
using lcpapi.Models;
using lcpapi.Interfaces;
using lcpapi.Authorization;
using lcpapi.Models.QParams;

namespace lcpapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        private readonly ISettingsRepo _settingsRepo;

        public SettingsController(ISettingsRepo settingsRepo)
        {
            _settingsRepo = settingsRepo;
        }

        /// <summary>
        /// Gets all settings infos.
        /// </summary>
        /// <param name="queryParams"></param>
        /// <returns>Gets all settings infos</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/settings
        ///     {
        ///         [
        ///             {
        ///                 "settingsId": 0,
        ///                 "themeSettingName": "string",
        ///                 "realDataTimeEnabled": false,
        ///                 "isDarkMode": false,
        ///                 "autoRefreshInterval": 0,
        ///                 "notificationsEnabled": false,
        ///                 "enableLogging": false,
        ///             }
        ///         ]
        ///     }
        ///
        /// </remarks>
        /// <response code="201">Returns the all infos about settings</response>
        /// <response code="400">If the settings infos are empty</response>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<Settings>>> GetSettings([FromQuery] QueryParams queryParams)
        {
            var settings = await _settingsRepo.GetSettings(queryParams);
            var totalCount = await _settingsRepo.GetTotalCountAsync(queryParams);
            var response = new QueryParamsResp<Settings> {
                TotalCount = totalCount,
                Page = queryParams.Page,
                PageSize = queryParams.PageSize,
                Data = settings.Value!.ToList()
            };

            return Ok(response);
        }

        /// <summary>
        /// Gets setting info by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Gets setting info by id</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/settings
        ///     {
        ///          "settingsId": 0,
        ///          "themeSettingName": "string",
        ///          "realDataTimeEnabled": false,
        ///          "isDarkMode": false,
        ///          "autoRefreshInterval": 0,
        ///          "notificationsEnabled": false,
        ///          "enableLogging": false,
        ///     }
        ///
        /// </remarks>
        /// <response code="201">Returns the info about setting</response>
        /// <response code="400">If the setting info is empty</response>
        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Settings>> GetSetting(int? id)
        {
            return await _settingsRepo.GetSetting(id);
        }
        
        /// <summary>
        /// Creates a setting.
        /// </summary>
        /// <param name="setting"></param>
        /// <returns>A newly created setting</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/settings
        ///     {
        ///         "settingsId": 0,
        ///         "themeSettingName": "string",
        ///         "realDataTimeEnabled": false,
        ///         "isDarkMode": false,
        ///         "autoRefreshInterval": 0,
        ///         "notificationsEnabled": false,
        ///         "enableLogging": false,
        ///     }
        ///
        /// </remarks>
        /// <response code="201">Returns the newly created setting info</response>
        /// <response code="400">If the setting info is empty</response>
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Settings>> PostSetting(Settings setting)
        {
            return await _settingsRepo.PostSetting(setting);
        }

        /// <summary>
        /// Updates specific setting info by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="setting"></param>
        /// <returns>Updates specific setting info by id</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /api/settings
        ///     {
        ///         "settingsId": 0,
        ///         "themeSettingName": "string",
        ///         "realDataTimeEnabled": false,
        ///         "isDarkMode": false,
        ///         "autoRefreshInterval": 0,
        ///         "notificationsEnabled": false,
        ///         "enableLogging": false,
        ///     }
        ///
        /// </remarks>
        /// <response code="201">Returns the all settings infos updated by id and its body</response>
        /// <response code="400">If the settings infos updated are empty by id and its body</response>
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [AllowAnonymous]
        // [Authorize(Policy = "AllUsers")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PutSetting(int? id, Settings setting)
        {
            return await _settingsRepo.PutSetting(id, setting);
        }

        /// <summary>
        /// Deletes specific setting info by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Deletes the specific setting info by id</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /api/settings/{id}
        ///
        /// </remarks>
        /// <response code="201">Returns the all settings infos are deleted by id</response>
        /// <response code="400">If the settings infos are deleted by id</response>
        [HttpDelete("{id}")]
        [AllowAnonymous]
        // [Authorize(Policy = "AllUsers")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteSetting(int? id)
        {
            return await _settingsRepo.DeleteSetting(id);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using lcpapi.Context;
using lcpapi.Models;
using lcpapi.Interfaces;
using lcpapi.Models.QParams;

namespace lcpapi.Controllers
{
    // [Route("api/[controller]")]
    [Route("api/media")]
    [ApiController]
    public class UploadedFilesController : ControllerBase
    {
        // private readonly MyDBContext _context;
        private readonly IUploadedFilesRepo _iUploadedFilesRepo;

        public UploadedFilesController(IUploadedFilesRepo uploadedFilesRepo)
        {
            _iUploadedFilesRepo = uploadedFilesRepo;
        }

        /// <summary>
        /// Uploads the files.
        /// </summary>
        /// <param name="files"></param>
        /// <returns>Uploads the files</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="201">Returns the operation of upload files</response>
        /// <response code="400">If the file upload is empty</response>
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFiles([FromForm] FileUploadModel files)
        {
             if(files == null)
            {
                return BadRequest();
            }
            try
            {
                await _iUploadedFilesRepo.PostFileAsync(files.FileDetails, files.FileType);
                return Ok();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Downloads the uploaded file.
        /// </summary>
        /// <returns>Downloads the uploaded file</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="201">Returns the operation of download uploaded file</response>
        /// <response code="400">If the file uploaded is empty</response>
        [Route("download")]
        [HttpGet]
        public async Task<IActionResult> DownloadFile(List<FileUploadModel> fileDetails)
        {
            if (fileDetails == null)
            {
                return BadRequest();
            }
            try
            {
                await _iUploadedFilesRepo.PostMultiFileAsync(fileDetails);
                return Ok();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Downloads the uploaded files.
        /// </summary>
        /// <returns>Downloads the uploaded files</returns>
        /// <param name="id"></param>
        /// <remarks>
        /// </remarks>
        /// <response code="201">Returns the operation of download uploaded files</response>
        /// <response code="400">If the files uploaded are empty</response>
        [Route("download/id")]
        [HttpGet]
        public async Task<ActionResult> DownloadFile(int id)
        {
            if (id < 1)
            {
                return BadRequest();
            }
            try
            {
                await _iUploadedFilesRepo.DownloadFileById(id);
                return Ok();
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}

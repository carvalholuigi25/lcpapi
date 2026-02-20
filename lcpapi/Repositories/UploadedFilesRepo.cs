// src: https://dzone.com/articles/upload-single-and-multiple-files-using-the-net-cor

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using lcpapi.Context;
using lcpapi.Models;
using lcpapi.Interfaces;
using lcpapi.Models.QParams;
using Microsoft.AspNetCore.SignalR;
using lcpapi.Hubs;

namespace lcpapi.Repositories;

public class UploadedFilesRepo : ControllerBase, IUploadedFilesRepo
{
    private readonly MyDBContext _context;
    private readonly IHubContext<ChatHub> _hubContext;

    public UploadedFilesRepo(MyDBContext context, IHubContext<ChatHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }

    public async Task PostFileAsync(IFormFile fileData, FileType fileType)
    {
        try
        {
            var fileDetails = new FileUploadInfo()
            {
                ID = 0,
                FileName = fileData.FileName,
                FileType = fileType,
            };

            using (var stream = new MemoryStream())
            {
                fileData.CopyTo(stream);
                fileDetails.FileData = stream.ToArray();
            }

            var result = _context.FileUploadInfos.Add(fileDetails);
            await CopyFileTo(fileDetails, "uploaded");
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", "File uploaded!");
            await _context.SaveChangesAsync();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task PostMultiFileAsync(List<FileUploadModel> fileData)
    {
        try
        {
            foreach (FileUploadModel file in fileData)
            {
                var fileDetails = new FileUploadInfo()
                {
                    ID = 0,
                    FileName = file.FileDetails.FileName,
                    FileType = file.FileType,
                };

                using (var stream = new MemoryStream())
                {
                    file.FileDetails.CopyTo(stream);
                    fileDetails.FileData = stream.ToArray();
                }

                var result = _context.FileUploadInfos.Add(fileDetails);
                await CopyFileTo(fileDetails, "uploaded");
            }

            await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", "Files uploaded!");
            await _context.SaveChangesAsync();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task DownloadFileById(int Id)
    {
        try
        {
            var file = _context.FileUploadInfos.Where(x => x.ID == Id).FirstOrDefaultAsync();
            var content = new System.IO.MemoryStream(file.Result!.FileData!);
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/assets/files/downloaded", file.Result.FileName!);
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", "System", "File(s) downloaded!");
            await CopyStream(content, path);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task CopyStream(Stream stream, string downloadPath)
    {
        using (var fileStream = new FileStream(downloadPath, FileMode.Create, FileAccess.Write))
        {
            await stream.CopyToAsync(fileStream);
        }
    }
    
    private async Task CopyFileTo(FileUploadInfo fileDetails, string path = "uploaded")
    {
        await CopyStream(new System.IO.MemoryStream(fileDetails.FileData!), Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/assets/files/"+path, fileDetails.FileName!));
    }
}

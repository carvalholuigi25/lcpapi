using lcpapi.Models;
using lcpapi.Models.QParams;
using Microsoft.AspNetCore.Mvc;

namespace lcpapi.Interfaces;

public interface IUploadedFilesRepo {
    public Task PostFileAsync(IFormFile fileData, FileType fileType);
        public Task PostMultiFileAsync(List<FileUploadModel> fileData);
        public Task DownloadFileById(int fileName);
}
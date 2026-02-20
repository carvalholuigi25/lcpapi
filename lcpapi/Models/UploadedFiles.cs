using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace lcpapi.Models;

public class FileUploadInfo
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ID { get; set; }
    public string? FileName { get; set; }
    public byte[]? FileData { get; set; }
    public FileType FileType { get; set; }
}

public class FileUploadModel
{
    public IFormFile FileDetails { get; set; } = null!;
    public FileType FileType { get; set; }
}

public enum FileType
{
    PDF = 1,
    DOCX = 2,
    PNG = 3,
JPG = 4
}
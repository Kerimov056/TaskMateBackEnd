using TaskMate.DTOs.AttachmentD;
using TaskMate.Entities;

namespace TaskMate.Service.Abstraction;

public interface IAttachmentService
{
    Task<List<Attachment>> GetAllAsync(Guid CardId);
    Task CreateAsync(CreateAttachmentDto createAttachmentDto);
    Task<string> UploadFileAsync(IFormFile file);

}

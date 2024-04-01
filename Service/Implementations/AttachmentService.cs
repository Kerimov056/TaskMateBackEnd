using Google.Cloud.Storage.V1;
using Microsoft.EntityFrameworkCore;
using TaskMate.Context;
using TaskMate.DTOs.AttachmentD;
using TaskMate.Entities;
using Microsoft.AspNetCore.Http;
using TaskMate.Exceptions;
using TaskMate.Service.Abstraction;

namespace TaskMate.Service.Implementations;

public class AttachmentService : IAttachmentService
{
    public AppDbContext _appDbContext { get; set; }
    public AttachmentService(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task CreateAsync(CreateAttachmentDto createAttachmentDto)
    {
        var attechment = new Attachment()
        {
            Link = createAttachmentDto.Link,
            Text = createAttachmentDto.Text,
            CardsId = createAttachmentDto.CardId,
        };

        var file = await UploadFileAsync(createAttachmentDto.FileName);
        attechment.FileName = file;

        await _appDbContext.AddAsync(attechment);
        await _appDbContext.SaveChangesAsync();
    }

    public async Task<string> UploadFileAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new DirectoryNotFoundException("Not Found Image");


        var storage = StorageClient.Create();
        var imageUrl = string.Empty;

        using (var memoryStream = new MemoryStream())
        {
            await file.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            var objectName = $"TaskMate/{Guid.NewGuid()}_{file.FileName}";
            var bucketName = "task_mate-f";

            await storage.UploadObjectAsync(bucketName, objectName, null, memoryStream);
            var url = $"https://storage.googleapis.com/{bucketName}/{objectName}";

            imageUrl = url;
        }

        return imageUrl;
    }

    public async Task<List<Attachment>> GetAllAsync(Guid CardId)
    {
        var card = await _appDbContext.Cards.FirstOrDefaultAsync(x=>x.Id==CardId);
        if (card == null) throw new NotFoundException("Not found");

        return await _appDbContext.Attachments.Where(x => x.CardsId == CardId).ToListAsync();
    }
}

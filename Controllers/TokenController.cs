using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskMate.Context;
using Token = TaskMate.Entities.Token;

namespace TaskMate.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TokenController : ControllerBase
{
    private readonly AppDbContext _appDbContext;
    private const int TokenValidityDurationSeconds = 300; // 3 dakika
    private string _token;
    private DateTime _tokenExpirationTime;
    public TokenController(AppDbContext appDbContext)
    => _appDbContext = appDbContext;


    [HttpPost]
    public async Task<IActionResult> GetToken()
    {
        if (DateTime.Now > _tokenExpirationTime)
        {
            // Token süresi dolmuş, yeni bir token oluştur.
            _token = GenerateToken();
            _tokenExpirationTime = DateTime.Now.AddSeconds(TokenValidityDurationSeconds);
            var newToken = new Token()
            {
                TokenId = _token,
                CreateToken = _tokenExpirationTime
            };
            await _appDbContext.Tokens.AddAsync(newToken);
            await _appDbContext.SaveChangesAsync();
        }

        return Ok(new { token = _token });
    }

    [HttpGet("[action]")]
    public async Task<bool> GetToken(string TokenId)
    {
        var token = await _appDbContext.Tokens.FirstOrDefaultAsync(x=>x.TokenId== TokenId);
        if (token is null) return false;
        return true;
    }

    private string GenerateToken()
    {
        // Burada token oluşturma mantığını gerçekleştirin, örneğin bir GUID kullanabilirsiniz.
        return Guid.NewGuid().ToString();
    }
}

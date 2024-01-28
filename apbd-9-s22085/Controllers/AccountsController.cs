using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using apbd_8_s22085.Database;
using apbd_8_s22085.Database.Entities;
using apbd_8_s22085.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace apbd_8_s22085.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountsController : ControllerBase
{
    private readonly ILogger<AccountsController> _logger;
    private readonly DatabaseContext _database;
    private readonly IConfiguration _configuration;

    public AccountsController(ILogger<AccountsController> logger, DatabaseContext database, IConfiguration configuration)
    {
        _logger = logger;
        _database = database;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto registerDto)
    {
        var salt = Guid.NewGuid().ToString();
        var password = registerDto.Password;
        var passwordHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: Encoding.UTF8.GetBytes(salt),
            prf: KeyDerivationPrf.HMACSHA512,
            iterationCount: 10000,
            numBytesRequested: 256 / 8
        ));
        
        var user = new User
        {
            Login = registerDto.Login,
            Password = passwordHash,
            Salt = salt,
        };
        await _database.Users.AddAsync(user);
        await _database.SaveChangesAsync();

        return Ok();
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
        var user = await _database.Users.FirstOrDefaultAsync(u => u.Login == loginDto.Login);
        if (user == null)
        {
            return Unauthorized();
        }

        var passwordHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: loginDto.Password,
            salt: Encoding.UTF8.GetBytes(user.Salt),
            prf: KeyDerivationPrf.HMACSHA512,
            iterationCount: 10000,
            numBytesRequested: 256 / 8
        ));
        if (passwordHash != user.Password)
        {
            return Unauthorized();
        }

        Claim[] userClaims = new[]
        {
            new Claim(ClaimTypes.Name, user.Login),
        };
        
        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["SecretKey"]));
        SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        JwtSecurityToken token = new JwtSecurityToken(
            issuer: "http://localhost:5258",
            audience: "http://localhost:5258",
            claims: userClaims,
            expires: DateTime.Now.AddMinutes(15),
            signingCredentials: credentials
        );
        
        var refreshToken = Guid.NewGuid().ToString();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExp = DateTime.Now.AddDays(7);
        await _database.SaveChangesAsync();
        
        return Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token),
            refreshToken
        });
    }
    
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(TokenDto tokenDto)
    {
        var principal = getUserDataFromToken(tokenDto.token);
        if (principal == null || principal.Identity == null)
        {
            return Unauthorized();
        }
        var user = await _database.Users.FirstOrDefaultAsync(u => u.Login == principal.Identity.Name);
        
        if (user == null || user.RefreshToken != tokenDto.refreshToken || user.RefreshTokenExp < DateTime.Now)
        {
            return Unauthorized();
        }
        
        Claim[] userClaims = new[]
        {
            new Claim(ClaimTypes.Name, user.Login),
        };
        
        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["SecretKey"]));
        SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        JwtSecurityToken token = new JwtSecurityToken(
            issuer: "http://localhost:5258",
            audience: "http://localhost:5258",
            claims: userClaims,
            expires: DateTime.Now.AddMinutes(15),
            signingCredentials: credentials
        );
        
        var refreshTokenNew = Guid.NewGuid().ToString();
        user.RefreshToken = refreshTokenNew;
        user.RefreshTokenExp = DateTime.Now.AddDays(7);
        await _database.SaveChangesAsync();
        
        return Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token),
            refreshToken = refreshTokenNew
        });
    }
    
    private ClaimsPrincipal? getUserDataFromToken(string token)
    {
        var tokenValidationParams = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["SecretKey"])),
            ValidateLifetime = true
        };
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParams, out SecurityToken securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");

        return principal;
    }
}
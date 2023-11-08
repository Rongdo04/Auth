using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using PhanQuyenAPI.Models;
using PhanQuyenAPI.Services;
using PhanQuyenAPI.IServices;
using PhanQuyenAPI.Constants;
using Microsoft.AspNetCore.Identity;
using System.Net.Mail;
using MimeKit;
using MailKit.Security;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase {

    IPhanQuyenAPIServices services = new PhanQuyenAPIServices();
    private readonly IConfiguration _config;
    private readonly AppDbContext dbContext;
    
    public AuthController(IConfiguration config) {
        _config = config;
        dbContext = new AppDbContext();
       
    }
    [HttpPost("forgot-password")]
    public IActionResult ForgotPassword([FromBody] ForgotPasswordRequest request) {
        var user =  dbContext.Account.FirstOrDefault(x => x.email == request.email);
        if (user == null) {
            return BadRequest("User not found");
        }
        user.ResetPasswordToken = CreateToken(request.email, true);
        user.ResetPasswordTokenExpiry = DateTime.Now.AddHours(1);
            var smtpClient = new SmtpClient("smtp.gmail.com") {
                Port = 587,
                Credentials = new NetworkCredential("danghienxk@gmail.com", "ajrz dtdl kmkm hxvn"),
                EnableSsl = true,
                UseDefaultCredentials = false
            };

            var mailMessage = new MailMessage {
                From = new MailAddress(request.email),
                Subject = "Password Reset",
                IsBodyHtml = true,
                Body = $"<a href=\"http://127.0.0.1:5500/reset-password.html?token={user.ResetPasswordToken}\">Reset Password</a>",
            };
            mailMessage.To.Add(request.email);
            smtpClient.Send(mailMessage);
        dbContext.SaveChanges();
        return Ok("You may reset password now");
    }
    [HttpPost("reset-password")]
    public  IActionResult ResetPassword([FromBody] ResetPasswordRequest req) {
        var user =  dbContext.Account.FirstOrDefault(x => x.ResetPasswordToken == req.Token);
        if (user == null || user.ResetPasswordTokenExpiry < DateTime.Now) {
            return BadRequest("Invalid Token");
        }
        
        
        string newPassword = BCrypt.Net.BCrypt.HashPassword(req.Password);
        user.password = newPassword;
        user.ResetPasswordToken = null;
        user.ResetPasswordTokenExpiry = null;
        dbContext.SaveChanges();
        return Ok("Password successfully reset");
    }
    [HttpPost("Register")]
    public IActionResult Register([FromBody] RegisterRequest acc)  {
        // Xử lý thông tin đăng ký tài khoản ở đây
        var registrationResult = services.Register(acc.userName, acc.password,acc.confirmPassword);

        if (registrationResult == ErorrType.ThanhCong) {
            return Ok();
        } else if (registrationResult == ErorrType.TaiKhoanDaTonTai) {
            return BadRequest("TK da ton tai");
        }
        else {
            return BadRequest();

        }
    }
    [HttpPut("BanAcc")]
    public IActionResult BanAcc([FromBody] BanAccRequest acc) {
        // Xử lý thông tin đăng ký tài khoản ở đây
        var banAcc = services.BanAccount(acc.username);
        if (banAcc == ErorrType.TaiKhoanBiBan) {
            return Ok(acc.username);
        } else if (banAcc == ErorrType.TaiKhoanKhongTonTai) {
            return BadRequest("TK khong ton tai");
        } else if (banAcc == ErorrType.ThanhCong){
            return Ok(acc.username);
        }
        else {
            return BadRequest();
        }
    }

    [HttpPut("ChangePassword")]
    public IActionResult UpdatePass([FromBody] ChangePasswordRequest changePasswordRequest) {
        var decent = dbContext.Decentralization.Where(x => x.AuthorityName.ToLower() == "user").Any();
        if (decent) {
            var update = services.ChangePassWord(changePasswordRequest.Username, changePasswordRequest.OldPassword, changePasswordRequest.NewPassword);
            if (update == ErorrType.ThanhCong) {
                return Ok(update);
            } else {
                return BadRequest(update);
            }
        }
        return BadRequest();

    }
    [HttpPut("ChangeStt")]
    public IActionResult UpdateSTT([FromBody] SttRequest stt) {
        var updateStt = services.ChangeStatus(stt.Username, stt.Status);
        if(updateStt == ErorrType.ThanhCong) {
            return Ok(updateStt);
        }
        return BadRequest();
    }
    [HttpGet("DS")]
    public IActionResult GetPasswords() {
        var accounts = services.DSAccounts();
        return Ok(accounts);
    }
    [HttpPost("login")]
    public ActionResult<object> Authenticate([FromBody] Accounts acc) {
        var loginResponse = new LoginResponse { };
        var login = services.Login(acc);
        var roles = dbContext.Decentralization.Where(x => x.DecentralizationsID == acc.DecentralizationsID).Select(x => x.AuthorityName);
        foreach (var role in roles) {
            loginResponse.Role = role;
        }


        // if credentials are valid
        if (login == ErorrType.AdminLogin) {
           
            string token = CreateToken(acc.userName);
          
            loginResponse.Token = token;
            loginResponse.responseMsg = new HttpResponseMessage() {
                StatusCode = HttpStatusCode.OK
            };
           

            //return the token
            return Ok(loginResponse);
        } else if(login == ErorrType.UserLogin) {
            var status = dbContext.Account.FirstOrDefault(x => x.userName == acc.userName);
            if (status.status != null) {
                loginResponse.status = status.status;

            }

            string token = CreateToken(acc.userName);
            
            loginResponse.Token = token;
            loginResponse.responseMsg = new HttpResponseMessage() {
                StatusCode = HttpStatusCode.OK
            };
            return Ok(loginResponse);
        }
        else {
            return Unauthorized("fail to login");
        }
    }
    [HttpPost]
    private string CreateToken(string username, bool isResetPasswordToken = false) {
        List<Claim> claims = new()
            {
            //list of Claims - we only checking username - more claims can be added.
            new Claim("username", Convert.ToString(username)),

        };
        if (isResetPasswordToken) {
            claims.Add(new Claim("resetPasswordToken", "true"));
        }
        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));
        var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddHours(2),
            signingCredentials: cred
        );
        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return jwt;
    }
   

}

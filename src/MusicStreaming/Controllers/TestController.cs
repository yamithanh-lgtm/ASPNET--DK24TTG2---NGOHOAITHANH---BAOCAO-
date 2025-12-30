using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace MusicStreaming.Controllers
{
    public class TestController : Controller
    {
        // GET: Test/HashPassword
        public IActionResult HashPassword(string password = "123456")
        {
            var hash = HashPasswordMethod(password);
            
            ViewBag.Password = password;
            ViewBag.Hash = hash;
            
            return Content($@"
                <h1>Password Hash Test</h1>
                <p><strong>Password:</strong> {password}</p>
                <p><strong>Hash:</strong> {hash}</p>
                <hr>
                <p>SQL Command:</p>
                <pre>
UPDATE Users SET PasswordHash = '{hash}' WHERE Username = 'admin';
UPDATE Users SET PasswordHash = '{hash}' WHERE Username = 'user1';
UPDATE Users SET PasswordHash = '{hash}' WHERE Username = 'user2';
                </pre>
            ", "text/html");
        }
        
        private string HashPasswordMethod(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}

using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp_UnderTheHood.Pages.Accounts;

public class Login : PageModel
{
    [BindProperty]
    public Credential Credential { get; set; } = new Credential();
    
    public void OnGet()
    {
        
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();
        
        // verify creds
        if (Credential.Username == "admin" && Credential.Password == "password")
        {
            // creating context
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, Credential.Username),
                new Claim(ClaimTypes.Email, "admin@admin.com"),
                new Claim("Department", "HR"),
                new Claim("Admin", "true"),
                new Claim("Manager", "true"),
                new Claim("EmploymentDate", "2025-05-01")
            };
            var identity = new ClaimsIdentity(claims, "MyCookieAuth");
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);

            var authProps = new AuthenticationProperties
            {
                IsPersistent = Credential.RememberMe,
            };
            
            await HttpContext.SignInAsync("MyCookieAuth", claimsPrincipal, authProps);
            
            return RedirectToPage("/Index");
        }
        
        return Page();
    }
}

public class Credential
{
    [Required]
    [Display(Name = "Username or email address")]
    public string Username { get; set; } = string.Empty;
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } =  string.Empty;
    
    [Display(Name = "Remember me")]
    public bool RememberMe { get; set; } = false;
}
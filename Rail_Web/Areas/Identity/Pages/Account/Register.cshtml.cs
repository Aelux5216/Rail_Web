using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Rail_Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Rail_Web.Areas.Identity.Data;

namespace Rail_Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<Rail_WebUser> _signInManager;
        private readonly UserManager<Rail_WebUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<Rail_WebUser> userManager,
            SignInManager<Rail_WebUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "{0} cannot be blank")]
            [Display(Name = "Username")]
            [StringLength(15, ErrorMessage = "{0} must be between {2} and {1} characters long.", MinimumLength = 3)]
            public string Username { get; set; }

            [Required(ErrorMessage = "{0} cannot be blank")]
            [EmailAddress(ErrorMessage = "This {0} is not a valid e-mail address.")]
            [StringLength(255, ErrorMessage = "{0} must be under {1} characters long")]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required(ErrorMessage = "{0} cannot be blank")]
            [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-.]).{8,20}$", ErrorMessage = "Password must have at least one lower case character, one upper case character, one number & one special character.")]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [NotMapped]
            [Required(ErrorMessage = "{0} cannot be blank")]
            [Compare("Password", ErrorMessage = "The passwords do not match.")]
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            public string ConfirmPassword { get; set; }

            [Required(ErrorMessage = "{0} cannot be blank")]
            [StringLength(35, ErrorMessage = "{0} must be under {1} characters long")]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [StringLength(35, ErrorMessage = "{0} must be under {1} characters long")]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Required(ErrorMessage = "{0} cannot be blank")]
            [RegularExpression(@"^(?:(?:\(?(?:0(?:0|11)\)?[\s-]?\(?|\+)44\)?[\s-]?(?:\(?0\)?[\s-]?)?)|(?:\(?0))(?:(?:\d{5}\)?[\s-]?\d{4,5})|(?:\d{4}\)?[\s-]?(?:\d{5}|\d{3}[\s-]?\d{3}))|(?:\d{3}\)?[\s-]?\d{3}[\s-]?\d{3,4})|(?:\d{2}\)?[\s-]?\d{4}[\s-]?\d{4}))(?:[\s-]?(?:x|ext\.?|\#)\d{3,4})?$",
                ErrorMessage = "{0} must be in UK format.")]
            [DataType(DataType.PhoneNumber)]
            [Display(Name = "Phone/Mobile Number")]
            public string Phone { get; set; }

            [Required(ErrorMessage = "{0} cannot be blank")]
            [StringLength(35, ErrorMessage = "{0} must be under {1} characters long")]
            [Display(Name = "House Name/Number")]
            public string HouseName { get; set; }

            [Required(ErrorMessage = "{0} cannot be blank")]
            [StringLength(35, ErrorMessage = "{0} must be under {1} characters long")]
            [Display(Name = "Address Line 1")]
            public string Address1 { get; set; }

            [StringLength(35, ErrorMessage = "{0} must be under {1} characters long")]
            [Display(Name = "Address Line 2")]
            public string Address2 { get; set; }

            [Required(ErrorMessage = "{0} cannot be blank")]
            [RegularExpression(@"([Gg][Ii][Rr] 0[Aa]{2})|((([A-Za-z][0-9]{1,2})|(([A-Za-z][A-Ha-hJ-Yj-y][0-9]{1,2})|(([A-Za-z][0-9][A-Za-z])|([A-Za-z][A-Ha-hJ-Yj-y][0-9]?[A-Za-z]))))\s?[0-9][A-Za-z]{2})",
                ErrorMessage = "{0} Must be in UK format.")]
            [DataType(DataType.PostalCode)]
            [Display(Name = "Postcode")]
            public string Postcode { get; set; }
        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                var user = new Rail_WebUser { UserName = Input.Username, Email = Input.Email, FirstName = Input.FirstName, LastName = Input.LastName,
                    PhoneNumber = Input.Phone, HouseName = Input.HouseName, Address1 = Input.Address1, Address2 = Input.Address2, Postcode = Input.Postcode};

                try
                {
                    var result = await _userManager.CreateAsync(user, Input.Password);

                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User created a new account with password.");

                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        var callbackUrl = Url.Page(
                            "/Account/ConfirmEmail",
                            pageHandler: null,
                            values: new { userId = user.Id, code = code },
                            protocol: Request.Scheme);

                        await _emailSender.SendEmailAsync(Input.Email, "Confirm your account",
                            $"Please confirm your account by clicking <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'> here</a>.");

                        ViewData["StatusMessage"] = "Please make sure to confirm your account with the email you just recieved.";

                        //await _signInManager.SignInAsync(user, isPersistent: false); //Removed unless we need the user to be auto logged in.
                        return LocalRedirect(returnUrl);
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }

                catch
                {
                    ModelState.AddModelError(string.Empty, "Login Failed, we maybe experiencing issues at the moment.");
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}

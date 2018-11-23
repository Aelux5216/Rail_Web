using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Rail_Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Rail_Web.Areas.Identity.Data;

namespace Rail_Web.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<Rail_WebUser> _userManager;
        private readonly SignInManager<Rail_WebUser> _signInManager;
        private readonly IEmailSender _emailSender;

        public IndexModel(
            UserManager<Rail_WebUser> userManager,
            SignInManager<Rail_WebUser> signInManager,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        public string Username { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "{0} cannot be blank")]
            [EmailAddress(ErrorMessage = "This {0} is not a valid e-mail address.")]
            [StringLength(255, ErrorMessage = "{0} must be under {1} characters long")]
            [Display(Name = "Email")]
            public string Email { get; set; }

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
            public string PhoneNumber { get; set; }

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

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var userName = await _userManager.GetUserNameAsync(user);
            var email = user.Email;
            var phoneNumber = user.PhoneNumber;
            var firstName = user.FirstName;
            var lastName = user.LastName;
            var houseName = user.HouseName;
            var address1 = user.Address1;
            var address2 = user.Address2;
            var postcode = user.Postcode;

            Username = userName;

            Input = new InputModel
            {
                Email = email,
                PhoneNumber = phoneNumber,
                FirstName = firstName,
                LastName = lastName,
                HouseName = houseName,
                Address1 = address1,
                Address2 = address2,
                Postcode = postcode
            };

            IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            try
            {
                user.UserName = await _userManager.GetUserNameAsync(user);
                user.Email = Input.Email;
                user.FirstName = Input.FirstName;
                user.LastName = Input.LastName;
                user.PhoneNumber = Input.PhoneNumber;
                user.HouseName = Input.HouseName;
                user.Address1 = Input.Address1;
                user.Address2 = Input.Address2;
                user.Postcode = Input.Postcode;

                await _userManager.UpdateAsync(user);

                await _signInManager.RefreshSignInAsync(user);

                StatusMessage = "Your profile has been updated";
            }

            catch
            {
                throw new InvalidOperationException($"Unexpected error occurred while updating details.");
            }
            

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostSendVerificationEmailAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var userId = await _userManager.GetUserIdAsync(user);
            var email = await _userManager.GetEmailAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { userId = userId, code = code },
                protocol: Request.Scheme);
            await _emailSender.SendEmailAsync(
                email,
                "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            StatusMessage = "Verification email sent. Please check your email.";
            return RedirectToPage();
        }
    }
}

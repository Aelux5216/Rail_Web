using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Rail_Web.Areas.Identity.Data;
using Rail_Web.Models;

namespace Rail_Web.Areas.Identity.Pages.Account.Manage
{
    public class OrderHistoryModel : PageModel
    {
        private readonly UserManager<Rail_WebUser> _userManager;
        private readonly SignInManager<Rail_WebUser> _signInManager;
        private readonly ILogger<ChangePasswordModel> _logger;
        private readonly Rail_WebContext _context;

        public OrderHistoryModel(
            UserManager<Rail_WebUser> userManager,
            SignInManager<Rail_WebUser> signInManager,
            ILogger<ChangePasswordModel> logger, Rail_WebContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _context = context;
        }

        public string Username { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var userName = await _userManager.GetUserNameAsync(user);

            Username = userName;

            var orderhistorys = from b in _context.OrderHistory
                          select b;

            orderhistorys = orderhistorys.Where(s => s.Username.Equals(Username));

            Input = orderhistorys.ToList();

            return Page();
        }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public List<OrderHistory> Input { get; set; }

        public class OrderHistory
        {
            public int Id { get; set; }

            public string Username { get; set; }

            public string TicketRef { get; set; }

            public string TicketDet { get; set; }
        }
    }
}

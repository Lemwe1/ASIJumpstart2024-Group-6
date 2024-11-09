using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.Services;
using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ASI.Basecode.WebApp.Controllers
{
    /// <summary>
    /// Home Controller
    /// </summary>
    public class HomeController : ControllerBase<HomeController>
    {

        private readonly IWalletService _walletService;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="configuration"></param>
        /// <param name="localizer"></param>
        /// <param name="mapper"></param>
        public HomeController(IHttpContextAccessor httpContextAccessor,
                              ILoggerFactory loggerFactory,
                              IConfiguration configuration,
                              IWalletService walletService,
                              IMapper mapper = null) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            _walletService = walletService ?? throw new ArgumentNullException(nameof(walletService));
        }


        public async Task<IActionResult> Index(bool json = false)
        {
            // Get the logged-in user's ID
            int userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId").Value);

            // Fetch the debit liabilities belonging to the logged-in user
            var wallet = await _walletService.GetWalletAsync(userId);

            // Filter the list after awaiting the async method and convert to a List
            var userWallet = wallet
                .Where(x => x.UserId == userId)
                .ToList(); // Convert to List<DebitLiabilityViewModel>

            if (json)
            {
                return Json(userWallet); // Return JSON data if requested
            }

            // Calculate totals
            var totalDebit = userWallet.Sum(x => x.WalletBalance);

            ViewBag.TotalDebit = totalDebit;

            return View(userWallet); // Return the view otherwise
        }
    }
}

using AcmeLib;
using AcmeWebsite.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SecurityUtility;

namespace AcmeWebsite.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private AcmeLib.User BankUser => BankService.GetUser(User.Identity.Name);
        private int UserId => BankUser.Id;

        /// <summary>
        /// Landing page for authenticated bank user.  Displays user information and account summary.
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            ViewData["Title"] = @"Account Home";
    
				ViewBag.User = BankUser;
                var accts = BankService.GetAccounts(UserId);
                return View(accts);            
        }

        /// <summary>
        /// Initializes the user profile page
        /// </summary>
        /// <returns></returns>
        public IActionResult Profile()
        {

            ViewBag.user = BankUser;
            
                return View();
          
        }

        /// <summary>
        /// Action handler for saving the profile page.
        /// </summary>
        /// <param name="firstname"></param>
        /// <param name="lastname"></param>
        /// <param name="email"></param>
        /// <param name="phonenum"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SaveProfile(string firstname, string lastname, string email, string phonenum)
        {
           
                var user = BankUser;
                user.Firstname = firstname;
                user.Lastname = lastname;
                user.Email = email;
                user.Phone = phonenum;
                BankService.SaveProfile(user);
                return Redirect("~/account/Index");
        }

        /// <summary>
        /// Initializes the account detail page for the specified account
        /// </summary>
        /// <param name="acctId">The id of the account to display</param>
        /// <returns></returns>
        public IActionResult AccountDetail(int acctId)
        {
            ViewBag.account = BankService.GetAccount(acctId);
            ViewBag.transactions = BankService.GetTransactions(acctId);
            return View();
        }

        /// <summary>
        /// Initializes the account transfer page.
        /// </summary>
        /// <returns></returns>
		public IActionResult Transfer()
		{
            AccessRefMap<int> map = new AccessRefMap<int>();
            var accts = BankService.GetAccounts(UserId);
            var model = new TransferPayModel(accts, map);
            HttpContext.Session.SetJsonObject("map", map);
				
			return View(model);

		}
        
        /// <summary>
        /// Transfers funds from one user account to another
        /// </summary>
        /// <param name="fromAcct">The id of the account to debit</param>
        /// <param name="toAcct">The id of the account to credit</param>
        /// <param name="amount">The amount to transfer</param>
        /// <returns></returns>
		public IActionResult DoTransfer (TransferPayModel model)
		{
            var map = HttpContext.Session.GetJsonObject<AccessRefMap<int>>("map");
            BankService.Transfer(map.GetDirectReference(model.FromAccount), 
                map.GetDirectReference(model.ToAccount), model.Amount);
            HttpContext.Session.Remove("map");
            return Redirect("~/account/Index");
        }

        /// <summary>
        /// Initializes the Bill pay page.
        /// </summary>
        /// <returns></returns>
		public IActionResult BillPay()
		{
				ViewBag.Accts = BankService.GetAccounts(UserId);
				return View();
		}

        /// <summary>
        /// Handler for the bill pay submit.
        /// </summary>
        /// <param name="payee">The person to pay</param>
        /// <param name="amt">The amount to pay them</param>
        /// <param name="fromAcct">The account from which to pay</param>
        /// <returns></returns>
		public IActionResult DoBillPay(string payee, float amt, int fromAcct)
		{
			BankService.PayBill(fromAcct, payee, amt);
			return Redirect("~/account/Index");
		}

	}
}
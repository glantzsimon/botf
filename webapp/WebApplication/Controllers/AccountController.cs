﻿using K9.Base.DataAccessLayer.Models;
using K9.Base.Globalisation;
using K9.Base.WebApplication.Config;
using K9.Base.WebApplication.Controllers;
using K9.Base.WebApplication.Enums;
using K9.Base.WebApplication.Filters;
using K9.Base.WebApplication.Options;
using K9.Base.WebApplication.Services;
using K9.SharedLibrary.Authentication;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using NLog;
using System;
using System.Linq;
using System.Web.Mvc;
using WebMatrix.WebData;

namespace K9.WebApplication.Controllers
{
    public class AccountController : BaseController
	{
		private readonly IRepository<User> _repository;
		private readonly ILogger _logger;
	    private readonly IAccountService _accountService;
        
		public AccountController(IRepository<User> repository, ILogger logger, IMailer mailer, IOptions<WebsiteConfiguration> websiteConfig, IDataSetsHelper dataSetsHelper, IRoles roles, IAccountService accountService, IAuthentication authentication)
			: base(logger, dataSetsHelper, roles, authentication)
		{
			_repository = repository;
			_logger = logger;
		    _accountService = accountService;
		}
        
		#region Membership

		public ActionResult Login(string returnUrl)
		{
			if (WebSecurity.IsAuthenticated)
			{
				return RedirectToAction("Index", "Home");
			}

			TempData["ReturnUrl"] = returnUrl;
			return View(new UserAccount.LoginModel());
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Login(UserAccount.LoginModel model)
		{
			if (ModelState.IsValid)
			{
				switch (_accountService.Login(model.UserName, model.Password, model.RememberMe))
				{
					case ELoginResult.Success:
						if (TempData["ReturnUrl"] != null)
						{
							return Redirect(TempData["ReturnUrl"].ToString());
						}
						return RedirectToAction("Index", "Home");

					case ELoginResult.AccountLocked:
						return RedirectToAction("AccountLocked");

					default:
						ModelState.AddModelError("", Dictionary.UsernamePasswordIncorrectError);
						break;
				}
			}
			else
			{
				ModelState.AddModelError("", Dictionary.UsernamePasswordIncorrectError);
			}

			return View(model);
		}

		public ActionResult AccountLocked()
		{
			return View();
		}

		[Authorize]
		public ActionResult LogOff()
		{
			_accountService.Logout();
			return RedirectToAction("Index", "Home");
		}

		public ActionResult Register()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Register(UserAccount.RegisterModel model)
		{
			if (ModelState.IsValid)
			{
				var result = _accountService.Register(model);

				if (result.IsSuccess)
				{
					return RedirectToAction("AccountCreated", "Account");
				}

				foreach (var registrationError in result.Errors)
				{
					ModelState.AddModelError(registrationError.FieldName, registrationError.ErrorMessage);
				}
			}

			return View(model);
		}

		[Authorize]
		public ActionResult UpdatePassword()
		{
			return View();
		}

		[Authorize]
		public ActionResult UpdatePasswordSuccess()
		{
			return View();
		}

		[Authorize]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult UpdatePassword(UserAccount.LocalPasswordModel model)
		{
			if (ModelState.IsValid)
			{
				var result = _accountService.UpdatePassword(model);

				if (result.IsSuccess)
				{
					return RedirectToAction("UpdatePasswordSuccess", "Account");
				}

				foreach (var registrationError in result.Errors)
				{
					ModelState.AddModelError(registrationError.FieldName, registrationError.ErrorMessage);
				}
			}

			return View(model);
		}

		[Authorize]
		public ActionResult MyAccount()
		{
			var user = _repository.Find(u => u.Username == User.Identity.Name).FirstOrDefault();
			return View(user);
		}

		[Authorize]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult UpdateAccount(User model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					_repository.Update(model);
					ViewBag.IsPopupAlert = true;
					ViewBag.AlertOptions = new AlertOptions
					{
						AlertType = EAlertType.Success,
						Message = Dictionary.Success,
						OtherMessage = Dictionary.AccountUpdatedSuccess
					};
				}
				catch (Exception ex)
				{
					_logger.Error(ex.GetFullErrorMessage());
					ModelState.AddModelError("", Dictionary.FriendlyErrorMessage);
				}
			}

			return View("MyAccount", model);
		}

		#endregion


		#region Password Reset

		public ActionResult PasswordResetEmailSent()
		{
			return View();
		}

		public ActionResult PasswordResetRequest()
		{
			if (WebSecurity.IsAuthenticated)
			{
				return RedirectToAction("Index", "Home");
			}

			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult PasswordResetRequest(UserAccount.PasswordResetRequestModel model)
		{
			if (ModelState.IsValid)
			{
				var result = _accountService.PasswordResetRequest(model);
				if (result.IsSuccess)
				{
					return RedirectToAction("PasswordResetEmailSent", "Account", new { userName = model.UserName, result.Data });
				}

				return RedirectToAction("ResetPasswordFailed");
			}

			return View(model);
		}

		public ActionResult ResetPassword(string username, string token)
		{
			if (!_accountService.ConfirmUserFromToken(username, token))
			{
				return RedirectToAction("ResetPasswordFailed");
			}

			var model = new UserAccount.ResetPasswordModel
			{
				UserName = username,
				Token = token
			};

			return View(model);
		}

		public ActionResult ResetPasswordFailed()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult ResetPassword(UserAccount.ResetPasswordModel model)
		{
			if (ModelState.IsValid)
			{
				var result = _accountService.ResetPassword(model);
				if (result.IsSuccess)
				{
					return RedirectToAction("ResetPasswordSuccess");
				}

				foreach (var registrationError in result.Errors)
				{
					ModelState.AddModelError(registrationError.FieldName, registrationError.ErrorMessage);
				}
			}

			return View(model);
		}

		[Authorize]
		public ActionResult ResetPasswordSuccess()
		{
			return View();
		}

		#endregion


		#region Account Activation

		[AllowAnonymous]
		public ActionResult AccountCreated(string userName)
		{
			return View();
		}

		[AllowAnonymous]
		public ActionResult AccountActivated(string userName)
		{
			return View();
		}

		[AllowAnonymous]
		public ActionResult AccountActivationFailed()
		{
			return View();
		}

		[AllowAnonymous]
		public ActionResult AccountAlreadyActivated()
		{
			return View();
		}

		[AllowAnonymous]
		public ActionResult ActivateAccount(string userName, string token)
		{
			var result = _accountService.ActivateAccount(userName, token);

			switch (result.Result)
			{
				case EActivateAccountResult.Success:
					return RedirectToAction("AccountActivated", "Account", new { userName });

				case EActivateAccountResult.AlreadyActivated:
					return RedirectToAction("AccountAlreadyActivated", "Account");

				default:
					return RedirectToAction("AccountActivationFailed", "Account");
			}
		}

		[RequirePermissions(Permission = Permissions.Edit)]
		public ActionResult ActivateUserAccount(int userId)
		{
			var result = _accountService.ActivateAccount(userId);

			switch (result.Result)
			{
				case EActivateAccountResult.Success:
					var user = _repository.Find(userId);
					return RedirectToAction("AccountActivated", "Account", new { userName = user.Username });

				case EActivateAccountResult.AlreadyActivated:
					return RedirectToAction("AccountAlreadyActivated", "Account");

				default:
					return RedirectToAction("AccountActivationFailed", "Account");
			}
		}

		#endregion


		#region Helpers

		public override string GetObjectName()
		{
			return typeof(User).Name;
		}

		#endregion

	}
}
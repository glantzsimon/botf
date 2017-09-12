﻿using K9.WebApplication.Controllers;
using K9.WebApplication.Extensions;
using Xunit;

namespace K9.WebApplication.Tests.Unit.Extensions
{
	public class ExtensionTests
	{
		
		[Fact]
		public void GetControllerName_ShouldReturnNameOfController_ForUseInRouting()
		{
			Assert.Equal("Messages", typeof(MessagesController).GetControllerName());
		}
		
	}

}

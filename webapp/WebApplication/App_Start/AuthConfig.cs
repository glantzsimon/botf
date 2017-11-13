
using K9.Base.DataAccessLayer.Database;
using K9.SharedLibrary.Helpers;
using K9.WebApplication.Config;
using System;
using System.IO;

namespace K9.WebApplication
{
    public class AuthConfig
	{
		public static void InitialiseWebSecurity()
		{
			DatabaseInitialiser<Db>.InitialiseWebsecurity();

			var json = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config/appsettings.json"));
			var config = ConfigHelper.GetConfiguration<OAuthConfiguration>(json).Value;
		}
	}
}
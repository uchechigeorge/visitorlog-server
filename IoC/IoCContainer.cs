
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using VisitorLog.Server.Models;

namespace VisitorLog.Server
{

  /// <summary>
  /// The dependency injection container making use of the built .NET CORE service provider
  /// </summary>
  public static class IoCContainer
  {
    /// <summary>
    /// The service provider of this application
    /// </summary>
    public static IServiceProvider Provider { get; set; }

    /// <summary>
    /// The configuration manager for the application
    /// </summary>
    public static IConfiguration Configuration { get; set; }

  }

	/// <summary>
	/// A shorthand access class to get DI services with clean short code 
	/// </summary>
	public static class IoC
	{
		/// <summary>
		/// A scoped instance of the <see cref="ApplicationDbContext"/>
		/// </summary>
		public static ApplicationDbContext ApplicationDbContext => IoCContainer.Provider.GetService<ApplicationDbContext>();

	}

}
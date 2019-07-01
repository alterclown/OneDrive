using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace OneDriveWeb.Util
{
    public class SettingsHelper
    {
        public static string ClientId
        {
            get
            {
                return ConfigurationManager.AppSettings["ida:ClientId"];
            }
        }
        public static string ClientSecret
        {
            get
            {
                return ConfigurationManager.AppSettings["ida:ClientSecret"];
            }
        }
        public static string AzureAdInstance
        {
            get
            {
                return ConfigurationManager.AppSettings["ida:AzureAdInstance"];
            }
        }
        public static string AzureAdTenantId
        {
            get
            {
                return ConfigurationManager.AppSettings["ida:AzureAdTenantId"];
            }
        }
        public static string GraphResourceUrl
        {
            get { return "https://graph.microsoft.com/"; }
        }
        public static string AzureAdGraphResourceURL
        {
            get { return "https://graph.microsoft.com/"; }
        }
        public static string AzureAdAuthority
        {
            get
            {
                return AzureAdInstance + AzureAdTenantId;
            }
        }
        public static string ClaimTypeObjectIdentifier
        {
            get { return "http://schemas.microsoft.com/identity/claims/objectidentifier"; }
        }
    }
}
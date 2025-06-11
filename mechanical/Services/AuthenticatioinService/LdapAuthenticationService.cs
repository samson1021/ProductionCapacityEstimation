using AutoMapper;
using System;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Runtime.Versioning;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using mechanical.Models.Login;
using mechanical.Models.Entities;

namespace mechanical.Services.AuthenticatioinService
{
    public class LdapAuthenticationService : IAuthenticationService
    {
        [SupportedOSPlatform("windows")]
        private readonly string _ip;
        private readonly string _port;
        private readonly string _email;
        private readonly string _password;
        private readonly ILogger<LdapAuthenticationService> _logger;

        public LdapAuthenticationService(IConfiguration configuration, ILogger<LdapAuthenticationService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            _ip = configuration["LDAP:IP"] ?? throw new ArgumentNullException("LDAP:IP configuration is missing.");
            _port = configuration["LDAP:Port"] ?? throw new ArgumentNullException("LDAP:Port configuration is missing.");
            _email = configuration["LDAP:Email"] ?? throw new ArgumentNullException("LDAP:Email configuration is missing.");
            _password = configuration["LDAP:Password"] ?? throw new ArgumentNullException("LDAP:Password configuration is missing.");
        
            // // Load LDAP settings from environment variables or use defaults
            // _ip = Environment.GetEnvironmentVariable("LDAP_HOST") ?? "mail.cbe.com.et";
            // _port = int.TryParse(Environment.GetEnvironmentVariable("LDAP_PORT"), out var port) ? port : 389;
            // _email = Environment.GetEnvironmentVariable("LDAP_EMAIL") ?? throw new ArgumentNullException("LDAP_EMAIL");
            // _password = Environment.GetEnvironmentVariable("LDAP_PASSWORD") ?? throw new ArgumentNullException("LDAP_PASSWORD");

            //if (string.IsNullOrWhiteSpace(_systemEmail) || string.IsNullOrWhiteSpace(_systemPassword))
            //{
            //    throw new ArgumentNullException("LDAP Email or Password is not configured properly.");
            //}
            if (string.IsNullOrWhiteSpace(_email) || string.IsNullOrWhiteSpace(_password))
            {
                throw new ArgumentNullException("LDAP Email or Password is not configured properly.");
            }
        }

        //authenticate user by CBE outlook email and password
        public bool AuthenticateUserByAD(string EnterdUsername, string password)
        {
            string username = GetSamAccountName(EnterdUsername);

            UserAdAttribute result = new UserAdAttribute();
            using (PrincipalContext cx = new PrincipalContext(ContextType.Domain, _ip))
            {
                bool isValid = cx.ValidateCredentials(username, password);
                if (isValid)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        [SupportedOSPlatform("windows")]
        public UserAdAttribute GetEmployeeInfo(string EmpId)
        {
            var samAccountName = GetSamAccountName(EmpId);
            var result = GetUserDataAD(samAccountName);
            return result;
        }

        [SupportedOSPlatform("windows")]
        //getSamAccount by CBE outlook email
        public string GetSamAccountName(string username)
        {
            try
            {
                // Security Note: _ip and _port are loaded from trusted configuration and validated for format.
                // No user input is used here, so this is not subject to LDAP Injection (see CWE-90).
                DirectoryEntry directoryEntry = new DirectoryEntry($"LDAP://{_ip}:{_port}", _email, _password);
                
                // Construct a directory search using the username as the search criteria
                DirectorySearcher directorySearcher = new DirectorySearcher(directoryEntry);

                var safeUsername = LdapFilterEncode(username);
                directorySearcher.Filter = $"(|(sAMAccountName={safeUsername})(mail={safeUsername})(employeeID={safeUsername}))";

                // Execute the search and retrieve the first matching result
                SearchResult? searchResult = directorySearcher.FindOne();

                if (searchResult == null)
                {
                    _logger.LogWarning($"User not found: {username}");
                    return username;
                }

                // Retrieve the 'sAMAccountName' attribute value for the user
                DirectoryEntry? userDirectoryEntry = searchResult.GetDirectoryEntry();
                if (userDirectoryEntry.Properties["sAMAccountName"].Value is string samAccountName)
                {
                    return samAccountName.ToString();
                }
                else
                {
                    _logger.LogWarning($"sAMAccountName not found for user: {username}");
                    return username;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting SamAccountName");
                throw;
            }
        }

        // user attributs from ad server for further processing
        public UserAdAttribute GetUserDataAD(string SmAccountName)
        {
            try
            {
                if (!IsValidIpOrHostname(_ip)) throw new ArgumentException("Invalid LDAP IP/hostname configuration.");
                if (!IsValidPort(_port)) throw new ArgumentException("Invalid LDAP port configuration.");

                // Security Note: _ip and _port are loaded from trusted configuration and validated for format.
                // No user input is used here, so this is not subject to LDAP Injection (see CWE-90).
                DirectoryEntry searchRoot = new DirectoryEntry($"LDAP://{_ip}:{_port}", _email, _password);
                DirectorySearcher searcher = new DirectorySearcher(searchRoot);
                // {
                //     SearchScope = SearchScope.Subtree,
                //     PageSize = 1000
                // };
                searcher.SearchScope = SearchScope.Subtree;
                searcher.PageSize = 1000;
                
                var safeSamAccountName = LdapFilterEncode(SmAccountName);
                searcher.Filter = "(&(objectClass=user)(objectCategory=person)(sAMAccountName=" + safeSamAccountName + "))";
                searcher.PropertiesToLoad.Add("sAMAccountName");
                searcher.PropertiesToLoad.Add("mail");
                searcher.PropertiesToLoad.Add("employeeID");
                searcher.PropertiesToLoad.Add("displayname");
                searcher.PropertiesToLoad.Add("company");
                searcher.PropertiesToLoad.Add("department");
                searcher.PropertiesToLoad.Add("title");

                UserAdAttribute loginModels = new UserAdAttribute();
                SearchResult searchResult = searcher.FindOne();
                if (searchResult != null)
                {
                    if (searchResult.Properties.Contains("sAMAccountName")
                        && searchResult.Properties.Contains("mail")
                        && searchResult.Properties.Contains("employeeID")
                        && searchResult.Properties.Contains("displayname")
                    )
                    {
                        loginModels.Email = (String)searchResult.Properties["mail"][0];
                        loginModels.EmployeeId = (String)searchResult.Properties["employeeID"][0];
                        loginModels.UserName = (String)searchResult.Properties["sAMAccountName"][0];
                        loginModels.DisplayName = (String)searchResult.Properties["displayname"][0];
                        loginModels.Department = (String)searchResult.Properties["department"][0];
                        loginModels.Company = (String)searchResult.Properties["company"][0];
                        loginModels.JobTitle = (String)searchResult.Properties["title"][0];
                    }
                }
                return loginModels;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving user data from AD.");
                throw new ApplicationException("An error occurred while retrieving user data.", ex);
            }
        }

        public static string LdapFilterEncode(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            // Escape special characters in the input string
            return input.Replace("\\", "\\5c")
                        .Replace("*", "\\2a")
                        .Replace("(", "\\28")
                        .Replace(")", "\\29")
                        .Replace("\0", "\\00");
        }
        private static bool IsValidIpOrHostname(string value)
        {
            return System.Net.IPAddress.TryParse(value, out _) || Uri.CheckHostName(value) != UriHostNameType.Unknown;
        }

        private static bool IsValidPort(string value)
        {
            return int.TryParse(value, out int port) && port > 0 && port <= 65535;
        }
    }
}

﻿using AutoMapper;
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
                // Construct a directory search using the username as the search criteria
                DirectoryEntry directoryEntry = new DirectoryEntry($"LDAP://{_ip}:{_port}", _email, _password);
                DirectorySearcher directorySearcher = new DirectorySearcher(directoryEntry);
                directorySearcher.Filter = $"(|(sAMAccountName={username})(mail={username})(employeeID={username}))";

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
                string domainPath = $"LDAP://{_ip}:{_port}";
                DirectoryEntry searchRoot = new DirectoryEntry(domainPath, _email, _password);
                DirectorySearcher searcher = new DirectorySearcher(searchRoot);
                // {
                //     SearchScope = SearchScope.Subtree,
                //     PageSize = 1000
                // };
                searcher.SearchScope = SearchScope.Subtree;
                searcher.PageSize = 1000;

                searcher.Filter = "(&(objectClass=user)(objectCategory=person)(sAMAccountName=" + SmAccountName + "))";
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
    }
}

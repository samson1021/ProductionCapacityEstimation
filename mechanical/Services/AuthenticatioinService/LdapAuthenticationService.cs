using AutoMapper;
using mechanical.Models.Entities;
using mechanical.Models.Login;
using Microsoft.Extensions.Options;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;

namespace mechanical.Services.AuthenticatioinService
{
    public class LdapAuthenticationService : IAuthenticationService
    {
        //authenticate user by CBE outlook email and password 
        public bool AuthenticateUserByAD(string EnterdUsername, string password)
        {
            string username = GetSamAccountName(EnterdUsername);

            UserAdAttribute result = new UserAdAttribute();
            using (PrincipalContext cx = new PrincipalContext(ContextType.Domain, "10.1.11.13"))
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

        public UserAdAttribute GetEmployeeInfo(string EmpId)
        {
            var samAccountName = GetSamAccountName(EmpId);
            var result = GetUserDataAD(samAccountName);
            return result;
        }

        //getSamAccount by CBE outlook email
        public string GetSamAccountName(string username)
        {
            // Construct a directory search using the username as the search criteria
            DirectoryEntry directoryEntry = new DirectoryEntry("LDAP://10.1.11.13:389", "yohannessintayhu@cbe.com.et", "True@12346");
            DirectorySearcher directorySearcher = new DirectorySearcher(directoryEntry);
            directorySearcher.Filter = $"(|(sAMAccountName={username})(mail={username})(employeeID={username}))";

            // Execute the search and retrieve the first matching result
            SearchResult searchResult = directorySearcher.FindOne();

            if (searchResult == null)
            {
                return username;
            }

            // Retrieve the 'sAMAccountName' attribute value for the user
            DirectoryEntry userDirectoryEntry = searchResult.GetDirectoryEntry();
            string samAccountName = userDirectoryEntry.Properties["sAMAccountName"].Value.ToString();

            return samAccountName;
        }
        // user attributs from ad server for further processing 
        public UserAdAttribute GetUserDataAD(string SmAccountName)
        {
            try
            {
                UserAdAttribute loginModels = new UserAdAttribute();
                string domainPath = "LDAP://10.1.11.13:389";
                DirectoryEntry searchRoot = new DirectoryEntry(domainPath, "yohannessintayhu@cbe.com.et", "True@12346");
                DirectorySearcher searcher = new DirectorySearcher(searchRoot);
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
                SearchResult searchResult;
                SearchResult collection = searcher.FindOne();
                if (collection != null)
                {
                    searchResult = collection;
                    if (searchResult.Properties.Contains("sAMAccountName") && searchResult.Properties.Contains("mail")
                     && searchResult.Properties.Contains("employeeID") && searchResult.Properties.Contains("displayname"))
                    {
                        UserAdAttribute ObjLogininfo = new UserAdAttribute();
                        ObjLogininfo.Email = (String)searchResult.Properties["mail"][0];
                        ObjLogininfo.EmployeeID = (String)searchResult.Properties["employeeID"][0];
                        ObjLogininfo.UserName = (String)searchResult.Properties["sAMAccountName"][0];
                        ObjLogininfo.DisplayName = (String)searchResult.Properties["displayname"][0];
                        ObjLogininfo.Department = (String)searchResult.Properties["department"][0];
                        ObjLogininfo.company = (String)searchResult.Properties["company"][0];
                        ObjLogininfo.jobTitle = (String)searchResult.Properties["title"][0];
                        loginModels = ObjLogininfo;
                    }
                }
                return loginModels;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

}

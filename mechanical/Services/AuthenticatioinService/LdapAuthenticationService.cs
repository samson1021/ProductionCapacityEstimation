using AutoMapper;
using mechanical.Models.Login;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Runtime.Versioning;

namespace mechanical.Services.AuthenticatioinService
{
    public class LdapAuthenticationService : IAuthenticationService
    {
        [SupportedOSPlatform("windows")]
        public bool AuthenticateUserByAD(string EnterdUsername, string password)
        {
            string username = GetSamAccountName(EnterdUsername);

            UserAdAttribute result = new UserAdAttribute();
            using (var cx = new PrincipalContext(ContextType.Domain, "10.1.11.13"))
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
        public string GetSamAccountName(string username)
        {
            var directoryEntry = new DirectoryEntry("LDAP://10.1.11.13:389", "yohannessintayhu@cbe.com.et", "True@12346");
            var directorySearcher = new DirectorySearcher(directoryEntry);
            directorySearcher.Filter = $"(|(sAMAccountName={username})(mail={username})(employeeID={username}))";

            var searchResult = directorySearcher.FindOne();

            if (searchResult == null)
            {
                return username;
            }

            var userDirectoryEntry = searchResult.GetDirectoryEntry();
            if (userDirectoryEntry.Properties["sAMAccountName"].Value == null)
            {
                return username;
            }

            string samAccountName = userDirectoryEntry.Properties["sAMAccountName"].Value?.ToString() ?? username;

            return samAccountName;
        }

        [SupportedOSPlatform("windows")]
        public UserAdAttribute GetUserDataAD(string SmAccountName)
        {
            try
            {
                UserAdAttribute loginModels = new UserAdAttribute();
                string domainPath = "LDAP://10.1.11.13:389";
                var searchRoot = new DirectoryEntry(domainPath, "yohannessintayhu@cbe.com.et", "True@12346");
                var searcher = new DirectorySearcher(searchRoot);
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
                SearchResult? searchResult;
                SearchResult? collection = searcher.FindOne();
                if (collection != null)
                {
                    searchResult = collection;
                    if (searchResult.Properties.Contains("sAMAccountName") && searchResult.Properties.Contains("mail")
                     && searchResult.Properties.Contains("employeeID") && searchResult.Properties.Contains("displayname"))
                    {
                        UserAdAttribute ObjLogininfo = new UserAdAttribute();
                        ObjLogininfo.Email = (String)searchResult.Properties["mail"][0];
                        ObjLogininfo.emp_ID = (String)searchResult.Properties["employeeID"][0];
                        ObjLogininfo.DisplayName = (String)searchResult.Properties["sAMAccountName"][0];
                        ObjLogininfo.Name = (String)searchResult.Properties["displayname"][0];
                        ObjLogininfo.Branch = (String)searchResult.Properties["department"][0];
                        ObjLogininfo.company = (String)searchResult.Properties["company"][0];
                        ObjLogininfo.title = (String)searchResult.Properties["title"][0];
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

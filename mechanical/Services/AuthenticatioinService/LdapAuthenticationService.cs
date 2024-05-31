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
        public string AuthenticateUserByAD(string EnterdUsername, string password)
        {
            string username;

            // 
            #region to check weather the user Enter mail Domain or not
            if (ContainsAtSymbol(EnterdUsername))
            {
                username = EnterdUsername;
            }
            else
            {
                username = AddDomain(EnterdUsername, "@cbe.com.et");
            }
            bool ContainsAtSymbol(string email)
            {
                return email.Contains("@");
            }

            string AddDomain(string email, string domain)
            {
                if (!email.EndsWith(domain))
                {
                    email += domain;
                }
                return email;
            }
            #endregion

            UserAdAttribute result = new UserAdAttribute();
            using (PrincipalContext cx = new PrincipalContext(ContextType.Domain, "10.1.11.13"))
            {
                bool isValid = cx.ValidateCredentials(username, password);
                if (isValid)
                {
                    string SmAccountName = GetSamAccountName(username);
                    List<UserAdAttribute> result2 = GetUserDataAD("10.1.11.13", username, password, SmAccountName);
                    result.DisplayName = result2[0].DisplayName;
                    result.EmployeeID = result2[0].EmployeeID;
                    result.Email = result2[0].Email;
                    result.company = result2[0].company;
                    result.Department = result2[0].Department;
                    result.jobTitle = result2[0].jobTitle;
                    result.UserName = result2[0].UserName;

                }
                else
                {
                    string message = "your credintial is not correct";

                    return null;
                }
            }
            //return resul
            return result.Email;

        }
        //getSamAccount by CBE outlook email
        public string GetSamAccountName(string username)
        {
            // Construct a directory search using the username as the search criteria
            DirectoryEntry directoryEntry = new DirectoryEntry("LDAP://10.1.11.13:389", "DigitalQuestionary@cbe.com.et", "Welcome2cbe");
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
        public List<UserAdAttribute> GetUserDataAD(string domainName, string userName, string password, string SmAccountName)
        {
            try
            {
                List<UserAdAttribute> loginModels = new List<UserAdAttribute>();
                string domainPath = "LDAP://" + domainName;
                DirectoryEntry searchRoot = new DirectoryEntry(domainPath, userName, password);
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
                SearchResultCollection collection = searcher.FindAll();
                if (collection != null)
                {
                    for (int count = 0; count < collection.Count; count++)
                    {
                        searchResult = collection[count];
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
                            loginModels.Add(ObjLogininfo);
                        }


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

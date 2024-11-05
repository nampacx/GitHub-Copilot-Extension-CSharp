using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Helpers.Extensions
{
    public static  class GitHubClientExtensions
    {
        public static  async Task<User> GetUser(this string tokenForUser)
        {
            // Identify the user, using the GitHub API token provided in the request headers.
            var client = new GitHubClient(new ProductHeaderValue("GitHubCopilotFunction"))
            {
                Credentials = new Credentials(tokenForUser)
            };
            return await client.User.Current();
        }
    }
}

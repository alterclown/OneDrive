using Microsoft.Graph;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using OneDriveWeb.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OneDriveWeb.Models
{
    public class FileRepository
    {
        private async Task<string> GetGraphAccessTokenAsync()
        {
            var signInUserId = ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier).Value;
            var userObjectId = ClaimsPrincipal.Current.FindFirst(SettingsHelper.ClaimTypeObjectIdentifier).Value;

            var clientCredential = new ClientCredential(SettingsHelper.ClientId, SettingsHelper.ClientSecret);
            var userIdentifier = new UserIdentifier(userObjectId, UserIdentifierType.UniqueId);

            AuthenticationContext authContext = new AuthenticationContext(SettingsHelper.AzureAdAuthority, new ADALTokenCache(signInUserId));
            var result = await authContext.AcquireTokenSilentAsync(SettingsHelper.AzureAdGraphResourceURL, clientCredential, userIdentifier);

            return result.AccessToken;
        }

        private async Task<GraphServiceClient> GetGraphServiceAsync() {
            var accessToken = await GetGraphAccessTokenAsync();
            var graphserviceClient = new GraphServiceClient(SettingsHelper.GraphResourceUrl, new DelegateAuthenticationProvider(
               (requestMessage) =>
               {
                   requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", accessToken);
                   return Task.FromResult(0);
               }));
            return graphserviceClient;
        }
        public async Task<List<DriveItem>> GetMyFiles(int pageIndex, int pageSize)
        {
            try
            {
                var graphServiceClient = await GetGraphServiceAsync();

                var requestFiles = await graphServiceClient.Me.Drive.Root.Children.Request().GetAsync();

                return requestFiles.CurrentPage.OrderBy(i => i.Name).Skip(pageIndex * pageSize).Take(pageSize).ToList();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task DeleteFile(string id) {
            try
            {
                var graphServiceClient = await GetGraphServiceAsync();
                await graphServiceClient.Me.Drive.Items[id].Request().DeleteAsync();
            }
            catch
            {
                throw;
            }
        }


        public async Task<DriveItem> UploadFile(System.IO.Stream fileStream, string filename) {
            try
            {
                var graphServiceClient = await GetGraphServiceAsync();
                var newItem = await graphServiceClient.Me.Drive.Root.Children.Request().AddAsync(new DriveItem {
                    Name = filename,
                File = new File()
            });

            return await graphServiceClient.Me.Drive.Items[newItem.Id].Content.Request().PutAsync<DriveItem>(fileStream);
            }

            catch {
                throw;
            }
        }


    }
}
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
// Contains the identity and role information about the user
using System.Security.Claims;
using System.Security.Principal;
using System.Text.Json;

namespace BlazorServerAuthenticationAndAuthorization.Authentication
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
		// ProtectedSessionStorage for storing user session data securely in the browser.
		private readonly ProtectedLocalStorage _localStorage;

		// _anonymous for unautheticated user. a "claim" is a piece of information about a user or system entity.
		// ClaimsPrincipal is used to represent an anonymous (unauthenticated) user, and designed to work with claims-based identity systems
		// A ClaimsPrincipal can be composed of multiple ClaimsIdentity instances. 
		private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());

        public CustomAuthenticationStateProvider(ProtectedLocalStorage localstorage)
        {
			_localStorage = localstorage;
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
			ClaimsIdentity identity;
			try
            {
                //await Task.Delay(5000);
				var jwt = await _localStorage.GetAsync<string>("jwt");
				string token = jwt.Success ? jwt.Value : "";

				if (token == "")
				{
					//return await Task.FromResult(new AuthenticationState(_anonymous));
					identity = new ClaimsIdentity();
					return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity)));
				}
					
			
				var payload = token.Split('.')[1];
				var jsonBytes = ParseBase64WithoutPadding(payload);
				var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

				var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
							{
					new Claim(ClaimTypes.Name, keyValuePairs["unique_name"].ToString()),
					new Claim("id", keyValuePairs["id"].ToString()),
					new Claim(ClaimTypes.Role, "user") //bueno
				            }, "CustomAuth"));



				//var claims = new[]
				//{
				//new Claim(ClaimTypes.Name, keyValuePairs["unique_name"].ToString()),
				//new Claim("id", keyValuePairs["id"].ToString()),
				//new Claim(ClaimTypes.Role, "user"), 
				//};

				//identity = new ClaimsIdentity(claims);


				return await Task.FromResult(new AuthenticationState(claimsPrincipal));

				//return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity)));
			}
            catch
            {
				identity = new ClaimsIdentity();
				return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity)));
				//return await Task.FromResult(new AuthenticationState(_anonymous));
            }
        }

		public void NotifyAuthenticationStateChanged()
		{
			NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
		}

		public async Task UpdateAuthenticationState(string token)
        {
            ClaimsPrincipal claimsPrincipal;

            if (token != "")
            {
                await _localStorage.SetAsync("blazorauth", token);
				var payload = token.Split('.')[1];
				var jsonBytes = ParseBase64WithoutPadding(payload);
				var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

				claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
				{
					new Claim(ClaimTypes.Name, keyValuePairs["unique_name"].ToString()),
					new Claim("id", keyValuePairs["id"].ToString()),
					new Claim(ClaimTypes.Role, "user") //bueno
                },"autenticado"));
			}
            else
            {
                await _localStorage.DeleteAsync("blazorauth");
                claimsPrincipal = _anonymous;
            }
			//NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
			NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
		}

		public byte[] ParseBase64WithoutPadding(string base64)
		{
			switch (base64.Length % 4)
			{
				case 2: base64 += "=="; break;
				case 3: base64 += "="; break;
			}
			return Convert.FromBase64String(base64);
		}
	
    
    }
}

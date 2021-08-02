using System;
using System.Configuration;
using System.IdentityModel.Metadata;
using System.Net;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Owin;
using Sustainsys.Saml2;
using Sustainsys.Saml2.Configuration;
using Sustainsys.Saml2.Owin;
using Sustainsys.Saml2.WebSso;
using Microsoft.Owin.Security.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;

[assembly: OwinStartup(typeof(MvcApplication.Startup))]

namespace MvcApplication
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            // Configure Auth0 parameters
            string auth0Domain = ConfigurationManager.AppSettings["auth0:Domain"];
            string auth0ClientId = ConfigurationManager.AppSettings["auth0:ClientId"];
            string auth0ReturnUrl = ConfigurationManager.AppSettings["auth0:ReturnUrl"];
            string auth0AppName = ConfigurationManager.AppSettings["auth0:AppName"];
            string auth0ClientSecret = ConfigurationManager.AppSettings["auth0:ClientSecret"];
            string auth0RedirectUri = ConfigurationManager.AppSettings["auth0:RedirectUri"];
            string auth0PostLogoutRedirectUri = ConfigurationManager.AppSettings["auth0:PostLogoutRedirectUri"];

            // Enable Kentor Cookie Saver middleware
            app.UseKentorOwinCookieSaver();

            // Set Cookies as default authentication type
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = CookieAuthenticationDefaults.AuthenticationType,
                LoginPath = new PathString("/Account/Login")
            });

            // Configure Auth0 authentication via SAML
            app.UseSaml2Authentication(CreateAuthServicesOptions());

            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                AuthenticationType = "Auth0",

                Authority = $"https://{auth0Domain}",

                ClientId = auth0ClientId,
                ClientSecret = auth0ClientSecret,

                RedirectUri = auth0RedirectUri,
                PostLogoutRedirectUri = auth0PostLogoutRedirectUri,

                ResponseType = OpenIdConnectResponseType.CodeIdToken,
                Scope = "openid profile",

                TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name"
                },

                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    RedirectToIdentityProvider = notification =>
                    {
                        if (notification.ProtocolMessage.RequestType == OpenIdConnectRequestType.Logout)
                        {
                            var logoutUri = $"https://{auth0Domain}/v2/logout?client_id={auth0ClientId}";

                            var postLogoutUri = notification.ProtocolMessage.PostLogoutRedirectUri;
                            if (!string.IsNullOrEmpty(postLogoutUri))
                            {
                                if (postLogoutUri.StartsWith("/"))
                                {
                                    // transform to absolute
                                    var request = notification.Request;
                                    postLogoutUri = request.Scheme + "://" + request.Host + request.PathBase + postLogoutUri;
                                }
                                logoutUri += $"&returnTo={ Uri.EscapeDataString(postLogoutUri)}";
                            }

                            notification.Response.Redirect(logoutUri);
                            notification.HandleResponse();
                        }
                        return Task.FromResult(0);
                    }

                }
            });
        }


        private static Saml2AuthenticationOptions CreateAuthServicesOptions()
        {
            // Configure Auth0 parameters
            string auth0Domain = ConfigurationManager.AppSettings["auth0:Domain"];
            string auth0ClientId = ConfigurationManager.AppSettings["auth0:ClientId"];
            string auth0ReturnUrl = ConfigurationManager.AppSettings["auth0:ReturnUrl"];
            string auth0AppName = ConfigurationManager.AppSettings["auth0:AppName"];

            var authServicesOptions = new Saml2AuthenticationOptions(false)
            {
                SPOptions = new SPOptions
                {
                    EntityId = new EntityId($"urn:{auth0AppName}"),
                    ReturnUrl = new Uri(auth0ReturnUrl),
                    MinIncomingSigningAlgorithm = "http://www.w3.org/2000/09/xmldsig#rsa-sha1"
                }
            };

            authServicesOptions.IdentityProviders.Add(new IdentityProvider(new EntityId($"urn:{auth0Domain}"), authServicesOptions.SPOptions)
            {
                AllowUnsolicitedAuthnResponse = true,
                MetadataLocation = String.Format("https://{0}/samlp/metadata/{1}", auth0Domain, auth0ClientId),
                Binding = Saml2BindingType.HttpPost
            });

            return authServicesOptions;
        }
    }
}

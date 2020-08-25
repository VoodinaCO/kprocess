using Newtonsoft.Json;

namespace KProcess.KL2.APIClient
{
    /// <summary>
    /// The implementation of the authentication response from API
    /// </summary>
    public class ApiAuthenticationResponse
    {
        /// <summary>
        /// The access token.
        /// </summary>
        public string Access_Token { get; set; }

        /// <summary>
        /// The type of the token.
        /// </summary>
        public string Token_Type { get; set; }

        /// <summary>
        /// The token expiry in seconds.
        /// </summary>
        public int Expires_In { get; set; }

        [JsonProperty("refresh_token")]
        public string Refresh_Token { get; set; }
    }
}

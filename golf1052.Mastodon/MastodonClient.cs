using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Flurl;
using golf1052.Mastodon.Models.Accounts;
using golf1052.Mastodon.Models.Apps;
using golf1052.Mastodon.Models.Apps.OAuth;
using golf1052.Mastodon.Models.OEmbed;
using golf1052.Mastodon.Models.Statuses;
using golf1052.Mastodon.Models.Statuses.Media;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace golf1052.Mastodon
{
    public class MastodonClient
    {
        public string? ClientId { get; set; }
        public string? ClientSecret { get; set; }
        public string? AccessToken { get; set; }

        private readonly Uri endpoint;
        private readonly HttpClient httpClient;
        private readonly ILogger<MastodonClient> logger;
        private readonly JsonSerializerSettings serializer;

        public MastodonClient(string endpoint) : this(endpoint, new HttpClient(), NullLogger<MastodonClient>.Instance)
        {
        }

        public MastodonClient(string endpoint, ILogger<MastodonClient> logger) : this(endpoint, new HttpClient(), logger)
        {
        }

        public MastodonClient(string endpoint, HttpClient httpClient, ILogger<MastodonClient> logger) :
            this(new Uri(endpoint), httpClient, logger)
        {
        }

        public MastodonClient(Uri endpoint, HttpClient httpClient, ILogger<MastodonClient> logger)
        {
            this.endpoint = endpoint;
            this.httpClient = httpClient;
            this.logger = logger;
            serializer = new JsonSerializerSettings()
            {
                ContractResolver = new DefaultContractResolver()
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                }
            };
        }

        public async Task<MastodonApplication> CreateApplication(string clientName,
            string redirectUri,
            List<string>? scopes = null,
            string? website = null)
        {
            Func<HttpRequestMessage> getRequest = () =>
            {
                Url url = new Url(endpoint).AppendPathSegments("api", "v1", "apps");
                List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("client_name", clientName),
                    new KeyValuePair<string, string>("redirect_uris", redirectUri)
                };

                if (scopes != null && scopes.Count > 0)
                {
                    parameters.Add(new KeyValuePair<string, string>("scopes", string.Join(' ', scopes)));
                }

                if (!string.IsNullOrWhiteSpace(website))
                {
                    parameters.Add(new KeyValuePair<string, string>("website", website));
                }

                FormUrlEncodedContent content = new FormUrlEncodedContent(parameters);
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
                requestMessage.Content = content;
                return requestMessage;
            };

            HttpResponseMessage responseMessage = await SendRequest(getRequest);
            return await Deserialize<MastodonApplication>(responseMessage);
        }

        public string AuthorizeUser(string redirectUri, List<string>? scope = null, bool? forceLogin = null, string? lang = null)
        {
            Url url = new Url(endpoint).AppendPathSegments("oauth", "authorize")
                .SetQueryParam("response_type", "code")
                .SetQueryParam("client_id", ClientId)
                .SetQueryParam("redirect_uri", redirectUri);

            if (scope != null && scope.Count > 0)
            {
                url.SetQueryParam("scope", string.Join(' ', scope));
            }

            if (forceLogin != null)
            {
                url.SetQueryParam("force_login", forceLogin.Value);
            }

            if (!string.IsNullOrWhiteSpace(lang))
            {
                url.SetQueryParam("lang", lang);
            }

            return url;
        }

        public async Task<MastodonToken> ObtainToken(string grantType,
            string redirectUri,
            string? code = null,
            List<string>? scope = null)
        {
            if (string.IsNullOrWhiteSpace(ClientId))
            {
                throw new MastodonException("Client ID not set. Client ID must be set to obtain a token.");
            }

            if (string.IsNullOrWhiteSpace(ClientSecret))
            {
                throw new MastodonException($"Client secret not set. Client secret must be set to obtain a token.");
            }

            Func<HttpRequestMessage> getRequest = () =>
            {
                Url url = new Url(endpoint).AppendPathSegments("oauth", "token");
                List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("grant_type", grantType),
                    new KeyValuePair<string, string>("client_id", ClientId),
                    new KeyValuePair<string, string>("client_secret", ClientSecret),
                    new KeyValuePair<string, string>("redirect_uri", redirectUri)
                };

                if (!string.IsNullOrWhiteSpace(code))
                {
                    parameters.Add(new KeyValuePair<string, string>("code", code));
                }

                if (scope != null && scope.Count > 0)
                {
                    parameters.Add(new KeyValuePair<string, string>("scope", string.Join(' ', scope)));
                }

                FormUrlEncodedContent content = new FormUrlEncodedContent(parameters);
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
                requestMessage.Content = content;
                return requestMessage;
            };

            HttpResponseMessage responseMessage = await SendRequest(getRequest);
            return await Deserialize<MastodonToken>(responseMessage);
        }

        public async Task<MastodonAccount> VerifyCredentials()
        {
            Func<HttpRequestMessage> getRequest = () =>
            {
                Url url = new Url(endpoint).AppendPathSegments("api", "v1", "accounts", "verify_credentials");
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
                return requestMessage;
            };

            HttpResponseMessage responseMessage = await SendAuthorizedRequest(getRequest);
            return await Deserialize<MastodonAccount>(responseMessage);
        }

        public async Task<MastodonStatus> PublishStatus(string status,
            IEnumerable<string>? mediaIds = null,
            string? inReplyToId = null,
            string? visibility = null)
        {
            Func<HttpRequestMessage> getRequest = () =>
            {
                Url url = new Url(endpoint).AppendPathSegments("api", "v1", "statuses");
                List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("status", status)
                };
                if (mediaIds != null)
                {
                    foreach (var mediaId in mediaIds)
                    {
                        parameters.Add(new KeyValuePair<string, string>("media_ids[]", mediaId));
                    }
                }
                if (!string.IsNullOrEmpty(inReplyToId))
                {
                    parameters.Add(new KeyValuePair<string, string>("in_reply_to_id", inReplyToId));
                }
                if (!string.IsNullOrWhiteSpace(visibility))
                {
                    parameters.Add(new KeyValuePair<string, string>("visibility", visibility));
                }
                FormUrlEncodedContent content = new FormUrlEncodedContent(parameters);
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
                requestMessage.Content = content;
                return requestMessage;
            };

            HttpResponseMessage responseMessage = await SendAuthorizedRequest(getRequest);
            return await Deserialize<MastodonStatus>(responseMessage);
        }

        public async Task<MastodonStatus> ViewPublicStatus(string id)
        {
            Func<HttpRequestMessage> getRequest = GetStatusRequest(id);
            HttpResponseMessage responseMessage = await SendRequest(getRequest);
            return await Deserialize<MastodonStatus>(responseMessage);
        }

        public async Task<MastodonStatus> ViewPrivateStatus(string id)
        {
            Func<HttpRequestMessage> getRequest = GetStatusRequest(id);
            HttpResponseMessage responseMessage = await SendAuthorizedRequest(getRequest);
            return await Deserialize<MastodonStatus>(responseMessage);
        }

        private Func<HttpRequestMessage> GetStatusRequest(string id)
        {
            return () =>
            {
                Url url = new Url(endpoint).AppendPathSegments("api", "v1", "statuses", id);
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
                return requestMessage;
            };
        }

        public async Task<MastodonStatus> DeleteStatus(string id)
        {
            Func<HttpRequestMessage> getRequest = () =>
            {
                Url url = new Url(endpoint).AppendPathSegments("api", "v1", "statuses", id);
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Delete, url);
                return requestMessage;
            };

            HttpResponseMessage responseMessage = await SendAuthorizedRequest(getRequest);
            return await Deserialize<MastodonStatus>(responseMessage);
        }

        public async Task<MastodonAttachment> UploadMedia(Stream stream)
        {
            MemoryStream? newStream = null;
            Func<HttpRequestMessage> getRequest = () =>
            {
                newStream = new MemoryStream();
                stream.CopyTo(newStream);
                stream.Position = 0;
                newStream.Position = 0;
                Url url = new Url(endpoint).AppendPathSegments("api", "v2", "media");
                MultipartFormDataContent content = new MultipartFormDataContent();
                content.Add(new StreamContent(newStream), "file", "file");
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
                requestMessage.Content = content;
                return requestMessage;
            };

            HttpResponseMessage responseMessage = await SendAuthorizedRequest(getRequest);
            if (newStream != null)
            {
                newStream.Dispose();
            }
            return await Deserialize<MastodonAttachment>(responseMessage);
        }

        public async Task<MastodonAttachment?> GetAttachment(string id)
        {
            Func<HttpRequestMessage> getRequest = () =>
            {
                Url url = new Url(endpoint).AppendPathSegments("api", "v1", "media", id);
                return new HttpRequestMessage(HttpMethod.Get, url);
            };

            HttpResponseMessage responseMessage = await SendAuthorizedRequest(getRequest);
            if (responseMessage.StatusCode == System.Net.HttpStatusCode.PartialContent)
            {
                return null;
            }
            else
            {
                return await Deserialize<MastodonAttachment>(responseMessage);
            }
        }

        public async Task<MastodonOEmbed> GetOEmbed(string url, int? maxWidth = null, int? maxHeight = null)
        {
            Func<HttpRequestMessage> getRequest = () =>
            {
                Url uri = new Url(endpoint).AppendPathSegments("api", "oembed");
                uri.SetQueryParam("url", url);
                if (maxWidth.HasValue)
                {
                    uri.SetQueryParam("maxwidth", maxWidth.Value);
                }
                if (maxHeight.HasValue)
                {
                    uri.SetQueryParam("maxheight", maxHeight.Value);
                }
                return new HttpRequestMessage(HttpMethod.Get, uri);
            };

            HttpResponseMessage responseMessage = await SendRequest(getRequest);
            if (responseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new MastodonException("OEmbed not found.");
            }
            else
            {
                return await Deserialize<MastodonOEmbed>(responseMessage);
            }
        }

        private async Task<HttpResponseMessage> SendAuthorizedRequest(Func<HttpRequestMessage> getHttpRequestMessage)
        {
            if (string.IsNullOrWhiteSpace(AccessToken))
            {
                throw new MastodonException("Access token not set. Unable to send authorized request.");
            }

            Func<HttpRequestMessage> getRequest = () =>
            {
                HttpRequestMessage request = getHttpRequestMessage();
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
                return request;
            };

            return await SendRequest(getRequest);
        }

        private async Task<HttpResponseMessage> SendRequest(Func<HttpRequestMessage> getHttpRequestMessage)
        {
            int tries = 0;
            int maxRetries = 10;
            do
            {
                tries += 1;
                HttpRequestMessage request = getHttpRequestMessage();
                HttpResponseMessage response = await httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    string errorString = await response.Content.ReadAsStringAsync();
                    if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests || errorString.Contains("Too many requests", StringComparison.OrdinalIgnoreCase))
                    {
                        if (response.Headers.Contains("X-RateLimit-Reset"))
                        {
                            string rateLimitResetString = response.Headers.GetValues("X-RateLimit-Reset").First();
                            logger.LogDebug($"Rate limited. Limit will lift at {rateLimitResetString}");
                            if (DateTime.TryParse(rateLimitResetString, out DateTime rateLimitResetTime))
                            {
                                // For some reason TryParse returns a DateTime with Kind local
                                DateTime now = DateTime.Now;
                                TimeSpan waitTime = (rateLimitResetTime - now).Add(TimeSpan.FromSeconds(1));
                                await Task.Delay(waitTime);
                            }
                            else
                            {
                                logger.LogDebug("Couldn't parse rate limit string. Waiting 30 minutes");
                                await Task.Delay(TimeSpan.FromMinutes(30));
                            }
                        }
                        else
                        {
                            logger.LogDebug("Rate limited but couldn't find X-RateLimit-Reset header. Waiting 30 minutes.");
                            await Task.Delay(TimeSpan.FromMinutes(30));
                        }
                    }
                    else if (errorString.Contains("please try again", StringComparison.OrdinalIgnoreCase))
                    {
                        logger.LogDebug($"Waiting 1 second because of {errorString} with status code {response.StatusCode}");
                        await Task.Delay(TimeSpan.FromSeconds(1));
                    }
                    else
                    {
                        try
                        {
                            JObject obj = JObject.Parse(errorString);
                            if (obj["error"] != null)
                            {
                                throw new MastodonException((string)obj["error"]!);
                            }
                            else
                            {
                                throw new MastodonException(errorString);
                            }
                        }
                        catch (JsonReaderException)
                        {
                            throw new MastodonException(errorString);
                        }
                    }
                }
                else
                {
                    return response;
                }
            }
            while (tries <= maxRetries);
            throw new MastodonException($"Hit limit of {maxRetries}");
        }

        private async Task<T> Deserialize<T>(HttpResponseMessage responseMessage)
        {
            string responseString = await responseMessage.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(responseString, serializer)!;
        }
    }
}

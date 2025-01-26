using System.Net.Http.Headers;

namespace Photobox.Lib.RestApi;

public partial class ClientBase()
{
    partial void PrepareRequest(HttpClient request, ref string url)
    {
        request.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _requestContext.GetBearerTokenOrTriggerUnauthException());
    }
}
# Photobox.LocalServer.RestApi.Api.ApplicationApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**ApiApplicationShutDownGet**](ApplicationApi.md#apiapplicationshutdownget) | **GET** /api/Application/ShutDown |  |

<a id="apiapplicationshutdownget"></a>
# **ApiApplicationShutDownGet**
> void ApiApplicationShutDownGet ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Photobox.LocalServer.RestApi.Api;
using Photobox.LocalServer.RestApi.Client;
using Photobox.LocalServer.RestApi.Model;

namespace Example
{
    public class ApiApplicationShutDownGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new ApplicationApi(config);

            try
            {
                apiInstance.ApiApplicationShutDownGet();
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ApplicationApi.ApiApplicationShutDownGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ApiApplicationShutDownGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    apiInstance.ApiApplicationShutDownGetWithHttpInfo();
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ApplicationApi.ApiApplicationShutDownGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
### Return type

void (empty response body)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: Not defined


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)


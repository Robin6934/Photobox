# Photobox.LocalServer.RestApi.Api.CameraApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**ApiCameraStartGet**](CameraApi.md#apicamerastartget) | **GET** /api/Camera/Start |  |
| [**ApiCameraStopGet**](CameraApi.md#apicamerastopget) | **GET** /api/Camera/Stop |  |
| [**ApiCameraTakePictureGet**](CameraApi.md#apicameratakepictureget) | **GET** /api/Camera/TakePicture |  |

<a id="apicamerastartget"></a>
# **ApiCameraStartGet**
> void ApiCameraStartGet ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Photobox.LocalServer.RestApi.Api;
using Photobox.LocalServer.RestApi.Client;
using Photobox.LocalServer.RestApi.Model;

namespace Example
{
    public class ApiCameraStartGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new CameraApi(config);

            try
            {
                apiInstance.ApiCameraStartGet();
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling CameraApi.ApiCameraStartGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ApiCameraStartGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    apiInstance.ApiCameraStartGetWithHttpInfo();
}
catch (ApiException e)
{
    Debug.Print("Exception when calling CameraApi.ApiCameraStartGetWithHttpInfo: " + e.Message);
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

<a id="apicamerastopget"></a>
# **ApiCameraStopGet**
> void ApiCameraStopGet ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Photobox.LocalServer.RestApi.Api;
using Photobox.LocalServer.RestApi.Client;
using Photobox.LocalServer.RestApi.Model;

namespace Example
{
    public class ApiCameraStopGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new CameraApi(config);

            try
            {
                apiInstance.ApiCameraStopGet();
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling CameraApi.ApiCameraStopGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ApiCameraStopGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    apiInstance.ApiCameraStopGetWithHttpInfo();
}
catch (ApiException e)
{
    Debug.Print("Exception when calling CameraApi.ApiCameraStopGetWithHttpInfo: " + e.Message);
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

<a id="apicameratakepictureget"></a>
# **ApiCameraTakePictureGet**
> TakePictureResultModel ApiCameraTakePictureGet ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Photobox.LocalServer.RestApi.Api;
using Photobox.LocalServer.RestApi.Client;
using Photobox.LocalServer.RestApi.Model;

namespace Example
{
    public class ApiCameraTakePictureGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new CameraApi(config);

            try
            {
                TakePictureResultModel result = apiInstance.ApiCameraTakePictureGet();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling CameraApi.ApiCameraTakePictureGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ApiCameraTakePictureGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<TakePictureResultModel> response = apiInstance.ApiCameraTakePictureGetWithHttpInfo();
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling CameraApi.ApiCameraTakePictureGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
### Return type

[**TakePictureResultModel**](TakePictureResultModel.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: text/plain, application/json, text/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)


# Photobox.LocalServer.RestApi.Api.PhotoboxApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**ApiPhotoboxDeleteImagePathGet**](PhotoboxApi.md#apiphotoboxdeleteimagepathget) | **GET** /api/Photobox/Delete/{imagePath} |  |
| [**ApiPhotoboxPrintImagePathGet**](PhotoboxApi.md#apiphotoboxprintimagepathget) | **GET** /api/Photobox/Print/{imagePath} |  |
| [**ApiPhotoboxSaveImagePathGet**](PhotoboxApi.md#apiphotoboxsaveimagepathget) | **GET** /api/Photobox/Save/{imagePath} |  |

<a id="apiphotoboxdeleteimagepathget"></a>
# **ApiPhotoboxDeleteImagePathGet**
> void ApiPhotoboxDeleteImagePathGet (string imagePath)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Photobox.LocalServer.RestApi.Api;
using Photobox.LocalServer.RestApi.Client;
using Photobox.LocalServer.RestApi.Model;

namespace Example
{
    public class ApiPhotoboxDeleteImagePathGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new PhotoboxApi(config);
            var imagePath = "imagePath_example";  // string | 

            try
            {
                apiInstance.ApiPhotoboxDeleteImagePathGet(imagePath);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling PhotoboxApi.ApiPhotoboxDeleteImagePathGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ApiPhotoboxDeleteImagePathGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    apiInstance.ApiPhotoboxDeleteImagePathGetWithHttpInfo(imagePath);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling PhotoboxApi.ApiPhotoboxDeleteImagePathGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **imagePath** | **string** |  |  |

### Return type

void (empty response body)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: text/plain, application/json, text/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | OK |  -  |
| **404** | Not Found |  -  |
| **403** | Forbidden |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="apiphotoboxprintimagepathget"></a>
# **ApiPhotoboxPrintImagePathGet**
> void ApiPhotoboxPrintImagePathGet (string imagePath)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Photobox.LocalServer.RestApi.Api;
using Photobox.LocalServer.RestApi.Client;
using Photobox.LocalServer.RestApi.Model;

namespace Example
{
    public class ApiPhotoboxPrintImagePathGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new PhotoboxApi(config);
            var imagePath = "imagePath_example";  // string | 

            try
            {
                apiInstance.ApiPhotoboxPrintImagePathGet(imagePath);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling PhotoboxApi.ApiPhotoboxPrintImagePathGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ApiPhotoboxPrintImagePathGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    apiInstance.ApiPhotoboxPrintImagePathGetWithHttpInfo(imagePath);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling PhotoboxApi.ApiPhotoboxPrintImagePathGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **imagePath** | **string** |  |  |

### Return type

void (empty response body)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: text/plain, application/json, text/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | OK |  -  |
| **404** | Not Found |  -  |
| **403** | Forbidden |  -  |
| **415** | Unsupported Media Type |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="apiphotoboxsaveimagepathget"></a>
# **ApiPhotoboxSaveImagePathGet**
> void ApiPhotoboxSaveImagePathGet (string imagePath)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Photobox.LocalServer.RestApi.Api;
using Photobox.LocalServer.RestApi.Client;
using Photobox.LocalServer.RestApi.Model;

namespace Example
{
    public class ApiPhotoboxSaveImagePathGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new PhotoboxApi(config);
            var imagePath = "imagePath_example";  // string | 

            try
            {
                apiInstance.ApiPhotoboxSaveImagePathGet(imagePath);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling PhotoboxApi.ApiPhotoboxSaveImagePathGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ApiPhotoboxSaveImagePathGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    apiInstance.ApiPhotoboxSaveImagePathGetWithHttpInfo(imagePath);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling PhotoboxApi.ApiPhotoboxSaveImagePathGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **imagePath** | **string** |  |  |

### Return type

void (empty response body)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: text/plain, application/json, text/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | OK |  -  |
| **404** | Not Found |  -  |
| **403** | Forbidden |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)


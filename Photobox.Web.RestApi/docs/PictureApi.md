# Photobox.Web.RestApi.Api.PictureApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**ApiPictureListPicturesGet**](PictureApi.md#apipicturelistpicturesget) | **GET** /api/Picture/ListPictures |  |
| [**ApiPictureTestGet**](PictureApi.md#apipicturetestget) | **GET** /api/Picture/Test |  |
| [**ApiPictureUploadPicturePost**](PictureApi.md#apipictureuploadpicturepost) | **POST** /api/Picture/UploadPicture |  |

<a id="apipicturelistpicturesget"></a>
# **ApiPictureListPicturesGet**
> List&lt;string&gt; ApiPictureListPicturesGet ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Photobox.Web.RestApi.Api;
using Photobox.Web.RestApi.Client;
using Photobox.Web.RestApi.Model;

namespace Example
{
    public class ApiPictureListPicturesGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new PictureApi(config);

            try
            {
                List<string> result = apiInstance.ApiPictureListPicturesGet();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling PictureApi.ApiPictureListPicturesGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ApiPictureListPicturesGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<string>> response = apiInstance.ApiPictureListPicturesGetWithHttpInfo();
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling PictureApi.ApiPictureListPicturesGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
### Return type

**List<string>**

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

<a id="apipicturetestget"></a>
# **ApiPictureTestGet**
> void ApiPictureTestGet (string filePath = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Photobox.Web.RestApi.Api;
using Photobox.Web.RestApi.Client;
using Photobox.Web.RestApi.Model;

namespace Example
{
    public class ApiPictureTestGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new PictureApi(config);
            var filePath = "filePath_example";  // string |  (optional) 

            try
            {
                apiInstance.ApiPictureTestGet(filePath);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling PictureApi.ApiPictureTestGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ApiPictureTestGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    apiInstance.ApiPictureTestGetWithHttpInfo(filePath);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling PictureApi.ApiPictureTestGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **filePath** | **string** |  | [optional]  |

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

<a id="apipictureuploadpicturepost"></a>
# **ApiPictureUploadPicturePost**
> void ApiPictureUploadPicturePost (System.IO.Stream formFile = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Photobox.Web.RestApi.Api;
using Photobox.Web.RestApi.Client;
using Photobox.Web.RestApi.Model;

namespace Example
{
    public class ApiPictureUploadPicturePostExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new PictureApi(config);
            var formFile = new System.IO.MemoryStream(System.IO.File.ReadAllBytes("/path/to/file.txt"));  // System.IO.Stream |  (optional) 

            try
            {
                apiInstance.ApiPictureUploadPicturePost(formFile);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling PictureApi.ApiPictureUploadPicturePost: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ApiPictureUploadPicturePostWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    apiInstance.ApiPictureUploadPicturePostWithHttpInfo(formFile);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling PictureApi.ApiPictureUploadPicturePostWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **formFile** | **System.IO.Stream****System.IO.Stream** |  | [optional]  |

### Return type

void (empty response body)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: multipart/form-data
 - **Accept**: Not defined


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)


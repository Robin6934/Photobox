# Photobox.Web.RestApi.Api.ImageApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**ApiImageDeleteImageDelete**](ImageApi.md#apiimagedeleteimagedelete) | **DELETE** /api/Image/DeleteImage |  |
| [**ApiImageDeleteImagesDelete**](ImageApi.md#apiimagedeleteimagesdelete) | **DELETE** /api/Image/DeleteImages |  |
| [**ApiImageGetImageImageNameGet**](ImageApi.md#apiimagegetimageimagenameget) | **GET** /api/Image/GetImage/{imageName} |  |
| [**ApiImageGetPreviewImageImageNameGet**](ImageApi.md#apiimagegetpreviewimageimagenameget) | **GET** /api/Image/GetPreviewImage/{imageName} |  |
| [**ApiImageListImagesGet**](ImageApi.md#apiimagelistimagesget) | **GET** /api/Image/ListImages |  |
| [**ApiImageUploadImagePost**](ImageApi.md#apiimageuploadimagepost) | **POST** /api/Image/UploadImage |  |

<a id="apiimagedeleteimagedelete"></a>
# **ApiImageDeleteImageDelete**
> void ApiImageDeleteImageDelete (string imageName = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Photobox.Web.RestApi.Api;
using Photobox.Web.RestApi.Client;
using Photobox.Web.RestApi.Model;

namespace Example
{
    public class ApiImageDeleteImageDeleteExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new ImageApi(config);
            var imageName = "imageName_example";  // string |  (optional) 

            try
            {
                apiInstance.ApiImageDeleteImageDelete(imageName);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ImageApi.ApiImageDeleteImageDelete: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ApiImageDeleteImageDeleteWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    apiInstance.ApiImageDeleteImageDeleteWithHttpInfo(imageName);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ImageApi.ApiImageDeleteImageDeleteWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **imageName** | **string** |  | [optional]  |

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

<a id="apiimagedeleteimagesdelete"></a>
# **ApiImageDeleteImagesDelete**
> void ApiImageDeleteImagesDelete ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Photobox.Web.RestApi.Api;
using Photobox.Web.RestApi.Client;
using Photobox.Web.RestApi.Model;

namespace Example
{
    public class ApiImageDeleteImagesDeleteExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new ImageApi(config);

            try
            {
                apiInstance.ApiImageDeleteImagesDelete();
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ImageApi.ApiImageDeleteImagesDelete: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ApiImageDeleteImagesDeleteWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    apiInstance.ApiImageDeleteImagesDeleteWithHttpInfo();
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ImageApi.ApiImageDeleteImagesDeleteWithHttpInfo: " + e.Message);
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

<a id="apiimagegetimageimagenameget"></a>
# **ApiImageGetImageImageNameGet**
> System.IO.Stream ApiImageGetImageImageNameGet (string imageName)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Photobox.Web.RestApi.Api;
using Photobox.Web.RestApi.Client;
using Photobox.Web.RestApi.Model;

namespace Example
{
    public class ApiImageGetImageImageNameGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new ImageApi(config);
            var imageName = "imageName_example";  // string | 

            try
            {
                System.IO.Stream result = apiInstance.ApiImageGetImageImageNameGet(imageName);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ImageApi.ApiImageGetImageImageNameGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ApiImageGetImageImageNameGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<System.IO.Stream> response = apiInstance.ApiImageGetImageImageNameGetWithHttpInfo(imageName);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ImageApi.ApiImageGetImageImageNameGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **imageName** | **string** |  |  |

### Return type

**System.IO.Stream**

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

<a id="apiimagegetpreviewimageimagenameget"></a>
# **ApiImageGetPreviewImageImageNameGet**
> System.IO.Stream ApiImageGetPreviewImageImageNameGet (string imageName)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Photobox.Web.RestApi.Api;
using Photobox.Web.RestApi.Client;
using Photobox.Web.RestApi.Model;

namespace Example
{
    public class ApiImageGetPreviewImageImageNameGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new ImageApi(config);
            var imageName = "imageName_example";  // string | 

            try
            {
                System.IO.Stream result = apiInstance.ApiImageGetPreviewImageImageNameGet(imageName);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ImageApi.ApiImageGetPreviewImageImageNameGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ApiImageGetPreviewImageImageNameGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<System.IO.Stream> response = apiInstance.ApiImageGetPreviewImageImageNameGetWithHttpInfo(imageName);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ImageApi.ApiImageGetPreviewImageImageNameGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **imageName** | **string** |  |  |

### Return type

**System.IO.Stream**

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

<a id="apiimagelistimagesget"></a>
# **ApiImageListImagesGet**
> List&lt;string&gt; ApiImageListImagesGet ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Photobox.Web.RestApi.Api;
using Photobox.Web.RestApi.Client;
using Photobox.Web.RestApi.Model;

namespace Example
{
    public class ApiImageListImagesGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new ImageApi(config);

            try
            {
                List<string> result = apiInstance.ApiImageListImagesGet();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ImageApi.ApiImageListImagesGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ApiImageListImagesGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<string>> response = apiInstance.ApiImageListImagesGetWithHttpInfo();
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ImageApi.ApiImageListImagesGetWithHttpInfo: " + e.Message);
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

<a id="apiimageuploadimagepost"></a>
# **ApiImageUploadImagePost**
> void ApiImageUploadImagePost (string imageName = null, System.IO.Stream formFile = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Photobox.Web.RestApi.Api;
using Photobox.Web.RestApi.Client;
using Photobox.Web.RestApi.Model;

namespace Example
{
    public class ApiImageUploadImagePostExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new ImageApi(config);
            var imageName = "imageName_example";  // string |  (optional) 
            var formFile = new System.IO.MemoryStream(System.IO.File.ReadAllBytes("/path/to/file.txt"));  // System.IO.Stream |  (optional) 

            try
            {
                apiInstance.ApiImageUploadImagePost(imageName, formFile);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ImageApi.ApiImageUploadImagePost: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ApiImageUploadImagePostWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    apiInstance.ApiImageUploadImagePostWithHttpInfo(imageName, formFile);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ImageApi.ApiImageUploadImagePostWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **imageName** | **string** |  | [optional]  |
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


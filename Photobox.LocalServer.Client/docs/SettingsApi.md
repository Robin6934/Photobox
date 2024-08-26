# Photobox.LocalServer.RestApi.Api.SettingsApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**ApiSettingsGetPhotoboxSettingsGet**](SettingsApi.md#apisettingsgetphotoboxsettingsget) | **GET** /api/Settings/GetPhotoboxSettings |  |
| [**ApiSettingsListPrintersGet**](SettingsApi.md#apisettingslistprintersget) | **GET** /api/Settings/ListPrinters |  |
| [**ApiSettingsPrintingEnabledGet**](SettingsApi.md#apisettingsprintingenabledget) | **GET** /api/Settings/PrintingEnabled |  |
| [**ApiSettingsSetPrinterEnabledPrinterEnabledGet**](SettingsApi.md#apisettingssetprinterenabledprinterenabledget) | **GET** /api/Settings/SetPrinterEnabled/{printerEnabled} |  |
| [**ApiSettingsSetPrinterPrinterNameGet**](SettingsApi.md#apisettingssetprinterprinternameget) | **GET** /api/Settings/SetPrinter/{printerName} |  |

<a id="apisettingsgetphotoboxsettingsget"></a>
# **ApiSettingsGetPhotoboxSettingsGet**
> PhotoboxConfig ApiSettingsGetPhotoboxSettingsGet ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Photobox.LocalServer.RestApi.Api;
using Photobox.LocalServer.RestApi.Client;
using Photobox.LocalServer.RestApi.Model;

namespace Example
{
    public class ApiSettingsGetPhotoboxSettingsGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new SettingsApi(config);

            try
            {
                PhotoboxConfig result = apiInstance.ApiSettingsGetPhotoboxSettingsGet();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling SettingsApi.ApiSettingsGetPhotoboxSettingsGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ApiSettingsGetPhotoboxSettingsGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<PhotoboxConfig> response = apiInstance.ApiSettingsGetPhotoboxSettingsGetWithHttpInfo();
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling SettingsApi.ApiSettingsGetPhotoboxSettingsGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
### Return type

[**PhotoboxConfig**](PhotoboxConfig.md)

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

<a id="apisettingslistprintersget"></a>
# **ApiSettingsListPrintersGet**
> ListPrintersResultModel ApiSettingsListPrintersGet ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Photobox.LocalServer.RestApi.Api;
using Photobox.LocalServer.RestApi.Client;
using Photobox.LocalServer.RestApi.Model;

namespace Example
{
    public class ApiSettingsListPrintersGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new SettingsApi(config);

            try
            {
                ListPrintersResultModel result = apiInstance.ApiSettingsListPrintersGet();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling SettingsApi.ApiSettingsListPrintersGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ApiSettingsListPrintersGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<ListPrintersResultModel> response = apiInstance.ApiSettingsListPrintersGetWithHttpInfo();
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling SettingsApi.ApiSettingsListPrintersGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
### Return type

[**ListPrintersResultModel**](ListPrintersResultModel.md)

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

<a id="apisettingsprintingenabledget"></a>
# **ApiSettingsPrintingEnabledGet**
> PrintingEnabledResultModel ApiSettingsPrintingEnabledGet ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Photobox.LocalServer.RestApi.Api;
using Photobox.LocalServer.RestApi.Client;
using Photobox.LocalServer.RestApi.Model;

namespace Example
{
    public class ApiSettingsPrintingEnabledGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new SettingsApi(config);

            try
            {
                PrintingEnabledResultModel result = apiInstance.ApiSettingsPrintingEnabledGet();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling SettingsApi.ApiSettingsPrintingEnabledGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ApiSettingsPrintingEnabledGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<PrintingEnabledResultModel> response = apiInstance.ApiSettingsPrintingEnabledGetWithHttpInfo();
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling SettingsApi.ApiSettingsPrintingEnabledGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
### Return type

[**PrintingEnabledResultModel**](PrintingEnabledResultModel.md)

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

<a id="apisettingssetprinterenabledprinterenabledget"></a>
# **ApiSettingsSetPrinterEnabledPrinterEnabledGet**
> void ApiSettingsSetPrinterEnabledPrinterEnabledGet (PrinterEnabledOptions printerEnabled)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Photobox.LocalServer.RestApi.Api;
using Photobox.LocalServer.RestApi.Client;
using Photobox.LocalServer.RestApi.Model;

namespace Example
{
    public class ApiSettingsSetPrinterEnabledPrinterEnabledGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new SettingsApi(config);
            var printerEnabled = (PrinterEnabledOptions) "True";  // PrinterEnabledOptions | 

            try
            {
                apiInstance.ApiSettingsSetPrinterEnabledPrinterEnabledGet(printerEnabled);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling SettingsApi.ApiSettingsSetPrinterEnabledPrinterEnabledGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ApiSettingsSetPrinterEnabledPrinterEnabledGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    apiInstance.ApiSettingsSetPrinterEnabledPrinterEnabledGetWithHttpInfo(printerEnabled);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling SettingsApi.ApiSettingsSetPrinterEnabledPrinterEnabledGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **printerEnabled** | **PrinterEnabledOptions** |  |  |

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

<a id="apisettingssetprinterprinternameget"></a>
# **ApiSettingsSetPrinterPrinterNameGet**
> void ApiSettingsSetPrinterPrinterNameGet (string printerName)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Photobox.LocalServer.RestApi.Api;
using Photobox.LocalServer.RestApi.Client;
using Photobox.LocalServer.RestApi.Model;

namespace Example
{
    public class ApiSettingsSetPrinterPrinterNameGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            var apiInstance = new SettingsApi(config);
            var printerName = "printerName_example";  // string | 

            try
            {
                apiInstance.ApiSettingsSetPrinterPrinterNameGet(printerName);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling SettingsApi.ApiSettingsSetPrinterPrinterNameGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ApiSettingsSetPrinterPrinterNameGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    apiInstance.ApiSettingsSetPrinterPrinterNameGetWithHttpInfo(printerName);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling SettingsApi.ApiSettingsSetPrinterPrinterNameGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **printerName** | **string** |  |  |

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


[System.Environment]::SetEnvironmentVariable(
    'JAVA_OPTS',
    '-Dio.swagger.parser.util.RemoteUrl.trustAll=true -Dio.swagger.v3.parser.util.RemoteUrl.trustAll=true'
)

openapi-generator-cli generate `
    -g csharp `
    -i https://localhost:59836/swagger/v1/swagger.json `
    --additional-properties "packageName=Photobox.Web.RestApi,targetFramework=netstandard2.1" `
    -o "Photobox.Web.RestApi"

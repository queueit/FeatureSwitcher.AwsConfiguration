# FeatureSwitcher.AwsConfiguration
Configuration plugin for FeatureSwitcher based on AWS services. Feature configuration is stored in DynamoDB and made available through the API Gateway. 

# Usage
```csharp
// Synchronous configuration
var config = AwsConfig.Configure("https://id.execute-api.eu-west-1.amazonaws.com/test");
Features.Are.ConfiguredBy.Custom(config.IsEnabled);
```

```csharp
// Asynchronous configuration
var configTask = AwsConfig.ConfigureAsync("https://id.execute-api.eu-west-1.amazonaws.com/test");
//Do other stuff
var config = await configTask;
Features.Are.ConfiguredBy.Custom(config.IsEnabled);
```
# Architecture
![Diagram](https://www.websequencediagrams.com/cgi-bin/cdraw?lz=QXBwbGljYXRpb24tPgACCzogU3RhcnQKCm9wdCBGb3IgZWFjaCBmZWF0dXJlCiAgICBGAAYGU3dpdGNoLkF3c0NvbmZpZ3VyAEYHAAIeOiBGaW5kIGFsbABNCHMAMSUgQVdTIEFQSSBHYXRld2F5OiBSZWdpc3RlcgCBEA0AFw8tPkFXUyBEeW5hbW86IFB1dEl0ZW0gKGNvbmRpdGlvbmFsbHkpACQVAIEmIgCBcQYAgU5FQ2FjaGUgYwCCQwUgNSBtaW51dGVzCmVuZAoKbm90ZSBvdmVyIACDGA5vbWUgdGltZSBwYXNzZXMKCgCDRQ0AgmQhAINLBiBlbmFibGVkPwCDagZJZiBjAIEGBWV4cGlyZWQgcmVsb2FkIGFzeW5jAINYJQCDFRFMb2FkAIQlDgCDBiJHZQCDIwUAgXqBHgCFR0BEZXRlcm1pbmUgaWYAgn8IADUgAIccDkUAgzYGL0RpcwA4Bgo&s=napkin, "Diagram")

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
![Diagram](https://www.websequencediagrams.com/cgi-bin/cdraw?lz=QXBwbGljYXRpb24tPgACCzogU3RhcnQKRmVhdHVyZVN3aXRjaC5Bd3NDb25maWd1cgAsBwACHjogRmluZCBhbGwgZgBDBnMKCm9wdCBGb3IgZWFjaAAPCAogICAgCiAgICAAUSAgQVdTIEFQSSBHYXRld2F5OiBSZWdpc3RlcgA_DQAXDy0-QVdTIER5bmFtbzogUHV0SXRlbSAoY29uZGl0aW9uYWxseSkAJBUAgUEiAIIMBgCBGCUAKiFhY2hlIGMAgl4FIDUgbWludXRlcwplbmQKCm5vdGUgb3ZlciAAgxkOb21lIHRpbWUgcGFzc2VzCgoAg0YNAIJ_IQCDWgYgZW5hYmxlZD8AgxkGSWYgYwCBBgVleHBpcmVkIHJlbG9hZCBhc3luYwCDAiUAgxURTG9hZACDTw4AgwYiR2UAgyMFAIF6gR4AhWJARGV0ZXJtaW5lIGlmAIJ_CACGVyAAhx0ORQCDNgYvRGlzADgGCg&s=napkin, "Diagram")

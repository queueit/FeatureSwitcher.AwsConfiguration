# Setup

## Setup AWS backend
The backend is a highly available REST API implemented using the AWS API Gateway and AWS DynamoDB. No code needs to be deployed anywhere.

The setup consist of 2 steps. 
- A cloudFormation script which will setup DynamoDB and create an IAM Role and policy to grant access to the API
- A manual configuration of API Gateway (as ColudFormation does not yet have support for the API Gateway)

### Create the CloudFormation Stack
The CloudFormation template is located [here](https://github.com/queueit/FeatureSwitcher.AwsConfiguration/blob/master/config/CloudFormation.template). Go to the AWS colsole -> CloudFormation -> CreateStack. Choose the file complete the guide. You can pick any name for the stack.

![Stack Resources](https://raw.githubusercontent.com/queueit/FeatureSwitcher.AwsConfiguration/master/docs/img/StackResources.PNG "Stack Resources")

The stack have created 3 resources: A DynamoDB table, a Role and a Policy. Note the Role Arn as we will use that in the next step.

### Create the API Gateway REST interface
While we wait for CloudFormation support we have provided this step-by-step guide to setting up the API Gateway. There is also a Swagger definition [here](https://github.com/queueit/FeatureSwitcher.AwsConfiguration/blob/master/config/FeatureSwitcher.AwsConfiguration-test-swagger-integrations.json).

- Go to the AWS Console -> API Gateway and create a new API.

![Setup Step](https://raw.githubusercontent.com/queueit/FeatureSwitcher.AwsConfiguration/master/docs/img/1.PNG "Setup Step")
- Create a new resource by the name 'feature'
- Add a GET method on the feature resource
- Add a PUP method on the feature resource

![Setup Step](https://raw.githubusercontent.com/queueit/FeatureSwitcher.AwsConfiguration/master/docs/img/2.PNG "Setup Step")
- Click the GET method and configure it as shown here. Select your Region (the same as where you created the CloudFormation stack) and put the Role Arn you created in through the CloudFormation stack. Note that you need the full Arn of the role which can be located in AIM -> Roles -> details of the created role.

![Setup Step](https://raw.githubusercontent.com/queueit/FeatureSwitcher.AwsConfiguration/master/docs/img/3.PNG "Setup Step")
- On the GET method, click Method Request
- Expand "URL Query String Parameters"
- Add the "FeatureName" parameter

![Setup Step](https://raw.githubusercontent.com/queueit/FeatureSwitcher.AwsConfiguration/master/docs/img/4.PNG "Setup Step")
- On the GET method, click Integration Request
- Expand "Mapping Templates"
- Add "application/json"
- Enter the following as Mapping Template:
```
{
    "ConsistentRead": false,
    "Key": 
        {
            "FeatureName" :
                {
                    "S": "$input.params('FeatureName')"
                }
        },
    "TableName": "FeatureSwitcherConfig"
}
```

![Setup Step](https://raw.githubusercontent.com/queueit/FeatureSwitcher.AwsConfiguration/master/docs/img/5.PNG "Setup Step")
- On the GET method, click Method Response
- Add "400"
- Add "500"

![Setup Step](https://raw.githubusercontent.com/queueit/FeatureSwitcher.AwsConfiguration/master/docs/img/6.PNG "Setup Step")
- On the GET method, click Integration Response
- Expand the existing response
- Set HTTP status regex to "2\d{2}"
- Expand "Mapping Templates"
- Add "application/json"
- Enter the following as Mapping Template:
```
#set($inputRoot = $input.path('$'))
{
   "type": "$inputRoot.Item.Type.S",
   "value": $input.json('$.Item.Value')
}
```
- Hit the "check" button (next to the "Mapping template" headline and then Save

![Setup Step](https://raw.githubusercontent.com/queueit/FeatureSwitcher.AwsConfiguration/master/docs/img/7.PNG "Setup Step")
- Add a new integration response
- Set HTTP status regex to "4\d{2}"
- Set Response status to "400"
- Save
- Expand "Mapping Templates"
- Add "application/json"
- Enter the following as Mapping Template:
```
#set($inputRoot = $input.path('$'))
{
   "type": $input.json('$.__type'),
   "message": $input.json('$.Message')
}
```
- Hit the "check" button (next to the "Mapping template" headline and then Save

![Setup Step](https://raw.githubusercontent.com/queueit/FeatureSwitcher.AwsConfiguration/master/docs/img/8.PNG "Setup Step")
- Add a new integration response
- Set HTTP status regex to "5\d{2}"
- Set Response status to "500"
- Save
- Expand "Mapping Templates"
- Add "application/json"
- Enter the following as Mapping Template:
```
#set($inputRoot = $input.path('$'))
{
   "type": $input.json('$.__type'),
   "message": $input.json('$.Message')
}
```
- Hit the "check" button (next to the "Mapping template" headline and then Save
![Setup Step](https://raw.githubusercontent.com/queueit/FeatureSwitcher.AwsConfiguration/master/docs/img/8-1.PNG "Setup Step")

- Click the PUT method and configure it as shown here. Select your Region (the same as where you created the CloudFormation stack) and put the Role Arn you created in through the CloudFormation stack. Note that you need the full Arn of the role which can be located in AIM -> Roles -> details of the created role.

![Setup Step](https://raw.githubusercontent.com/queueit/FeatureSwitcher.AwsConfiguration/master/docs/img/9.PNG "Setup Step")

- On the PUT method, click Method Request
- Expand "URL Query String Parameters"
- Add the "FeatureName" parameter

![Setup Step](https://raw.githubusercontent.com/queueit/FeatureSwitcher.AwsConfiguration/master/docs/img/10.PNG "Setup Step")
- On the PUT method, click Integration Request
- Expand "Mapping Templates"
- Add "application/json"
- Enter the following as Mapping Template:
```
{
    "TableName": "FeatureSwitcherConfig",
    "Item": {
        "FeatureName": {
            "S": "$input.params('FeatureName')"
        },
        "Type": {
            "S": "FeatureSwitcher.AwsConfiguration.Behaviours.BooleanBehaviour"
        },
        "Value": {
            "BOOL": false
        }
    },
    "ConditionExpression": "attribute_not_exists (FeatureName)"
}
```

![Setup Step](https://raw.githubusercontent.com/queueit/FeatureSwitcher.AwsConfiguration/master/docs/img/11.PNG "Setup Step")
- On the PUT method, click Method Response
- Add "400"
- Add "500"

![Setup Step](https://raw.githubusercontent.com/queueit/FeatureSwitcher.AwsConfiguration/master/docs/img/12.PNG "Setup Step")
- On the PUT method, click Integration Response
- Expand the existing response
- Set HTTP status regex to "2\d{2}"
- Expand "Mapping Templates"
- Add "application/json"
- Enter the following as Mapping Template:
```
{}
```
- Hit the "check" button (next to the "Mapping template" headline and then Save

![Setup Step](https://raw.githubusercontent.com/queueit/FeatureSwitcher.AwsConfiguration/master/docs/img/13.PNG "Setup Step")
- Add a new integration response
- Set HTTP status regex to "4\d{2}"
- Set Response status to "400"
- Save
- Expand "Mapping Templates"
- Add "application/json"
- Enter the following as Mapping Template:
```
#set($inputRoot = $input.path('$'))
{
   "type": $input.json('$.__type'),
   "message": $input.json('$.Message')
}
```
- Hit the "check" button (next to the "Mapping template" headline and then Save

![Setup Step](https://raw.githubusercontent.com/queueit/FeatureSwitcher.AwsConfiguration/master/docs/img/14.PNG "Setup Step")
- Add a new integration response
- Set HTTP status regex to "5\d{2}"
- Set Response status to "500"
- Save
- Expand "Mapping Templates"
- Add "application/json"
- Enter the following as Mapping Template:
```
#set($inputRoot = $input.path('$'))
{
   "type": $input.json('$.__type'),
   "message": $input.json('$.Message')
}
```
- Hit the "check" button (next to the "Mapping template" headline and then Save

![Setup Step](https://raw.githubusercontent.com/queueit/FeatureSwitcher.AwsConfiguration/master/docs/img/15.PNG "Setup Step")

- Verify all the settings are correct (The UI does not always work the way you would expect, and stuff is missing)
- Click the PUT method -> Test
- Enter "MyTestFeature" as FeatureName

![Setup Step](https://raw.githubusercontent.com/queueit/FeatureSwitcher.AwsConfiguration/master/docs/img/16.PNG "Setup Step")
- Verify that you get a 200 response code. If not expand Log to look for the error.
- You can also look into the Dynamo table "FeatureSwitcherConfig" and verify that there is an entry for your feature

![Setup Step](https://raw.githubusercontent.com/queueit/FeatureSwitcher.AwsConfiguration/master/docs/img/17.PNG "Setup Step")
- Click the GET method -> Test
- Enter "MyTestFeature" as FeatureName

![Setup Step](https://raw.githubusercontent.com/queueit/FeatureSwitcher.AwsConfiguration/master/docs/img/18.PNG "Setup Step")
- Verify that you get a 200 response code. If not expand Log to look for the error.

![Setup Step](https://raw.githubusercontent.com/queueit/FeatureSwitcher.AwsConfiguration/master/docs/img/19.PNG "Setup Step")
- Deploy the API to you environment

![Setup Step](https://raw.githubusercontent.com/queueit/FeatureSwitcher.AwsConfiguration/master/docs/img/20.PNG "Setup Step")
![Setup Step](https://raw.githubusercontent.com/queueit/FeatureSwitcher.AwsConfiguration/master/docs/img/21.PNG "Setup Step")
- And you are done! take note of the Invoke Url which you will need when you configure the .Net client code.
- 
![Setup Step](https://raw.githubusercontent.com/queueit/FeatureSwitcher.AwsConfiguration/master/docs/img/22.PNG "Setup Step")

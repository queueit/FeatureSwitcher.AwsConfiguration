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
![Setup Step](https://raw.githubusercontent.com/queueit/FeatureSwitcher.AwsConfiguration/master/docs/img/2.PNG "Setup Step")

![Setup Step](https://raw.githubusercontent.com/queueit/FeatureSwitcher.AwsConfiguration/master/docs/img/3.PNG "Setup Step")
![Setup Step](https://raw.githubusercontent.com/queueit/FeatureSwitcher.AwsConfiguration/master/docs/img/4.PNG "Setup Step")
![Setup Step](https://raw.githubusercontent.com/queueit/FeatureSwitcher.AwsConfiguration/master/docs/img/5.PNG "Setup Step")
![Setup Step](https://raw.githubusercontent.com/queueit/FeatureSwitcher.AwsConfiguration/master/docs/img/6.PNG "Setup Step")
![Setup Step](https://raw.githubusercontent.com/queueit/FeatureSwitcher.AwsConfiguration/master/docs/img/7.PNG "Setup Step")
![Setup Step](https://raw.githubusercontent.com/queueit/FeatureSwitcher.AwsConfiguration/master/docs/img/8.PNG "Setup Step")
![Setup Step](https://raw.githubusercontent.com/queueit/FeatureSwitcher.AwsConfiguration/master/docs/img/8-1.PNG "Setup Step")
![Setup Step](https://raw.githubusercontent.com/queueit/FeatureSwitcher.AwsConfiguration/master/docs/img/9.PNG "Setup Step")
![Setup Step](https://raw.githubusercontent.com/queueit/FeatureSwitcher.AwsConfiguration/master/docs/img/10.PNG "Setup Step")
![Setup Step](https://raw.githubusercontent.com/queueit/FeatureSwitcher.AwsConfiguration/master/docs/img/11.PNG "Setup Step")
![Setup Step](https://raw.githubusercontent.com/queueit/FeatureSwitcher.AwsConfiguration/master/docs/img/12.PNG "Setup Step")
![Setup Step](https://raw.githubusercontent.com/queueit/FeatureSwitcher.AwsConfiguration/master/docs/img/13.PNG "Setup Step")
![Setup Step](https://raw.githubusercontent.com/queueit/FeatureSwitcher.AwsConfiguration/master/docs/img/14.PNG "Setup Step")
![Setup Step](https://raw.githubusercontent.com/queueit/FeatureSwitcher.AwsConfiguration/master/docs/img/15.PNG "Setup Step")
![Setup Step](https://raw.githubusercontent.com/queueit/FeatureSwitcher.AwsConfiguration/master/docs/img/16.PNG "Setup Step")
![Setup Step](https://raw.githubusercontent.com/queueit/FeatureSwitcher.AwsConfiguration/master/docs/img/17.PNG "Setup Step")
![Setup Step](https://raw.githubusercontent.com/queueit/FeatureSwitcher.AwsConfiguration/master/docs/img/18.PNG "Setup Step")
![Setup Step](https://raw.githubusercontent.com/queueit/FeatureSwitcher.AwsConfiguration/master/docs/img/19.PNG "Setup Step")
![Setup Step](https://raw.githubusercontent.com/queueit/FeatureSwitcher.AwsConfiguration/master/docs/img/20.PNG "Setup Step")
![Setup Step](https://raw.githubusercontent.com/queueit/FeatureSwitcher.AwsConfiguration/master/docs/img/21.PNG "Setup Step")
![Setup Step](https://raw.githubusercontent.com/queueit/FeatureSwitcher.AwsConfiguration/master/docs/img/22.PNG "Setup Step")

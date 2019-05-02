# Setup

## Setup AWS backend
The backend is a highly available REST API implemented using the AWS API Gateway and AWS DynamoDB. No code needs to be deployed anywhere.

The setup consist of 1 steps. 
- A cloudFormation script which will setup DynamoDB, create an IAM Role and policy to grant access to the API and configure the API

### Create the CloudFormation Stack
The CloudFormation template is located [here](https://github.com/queueit/FeatureSwitcher.AwsConfiguration/blob/master/config/CloudFormation.template). Go to the AWS colsole -> CloudFormation -> CreateStack. Choose the file complete the guide. You can pick any name for the stack.
- Install AWS SAM ![What is SAM?](https://docs.aws.amazon.com/serverless-application-model/latest/developerguide/what-is-sam.html)
- Create deployment package: ```sam package --template-file CloudFormation.yaml --output-template-file package.template --region [REGION] --s3-bucket [S3PACKAGEBUCKET]```
- Deploy pacakge: ```sam deploy --template-file package.template --region [REGION] --stack-name FeatureSwitcher --capabilities CAPABILITY_IAM  --parameter-overrides StageName=[APISTAGENAME]```

The stack have created the following resources: 
- DynamoDB table
- IAM Role and Policy
- API Gateway
- API endpoint url in an EC2 System Parameter `/feature-switcher/api-endpoint-url`

And you are done! take note of the Invoke Url which you will need when you configure the .Net client code.
![Setup Step](https://raw.githubusercontent.com/queueit/FeatureSwitcher.AwsConfiguration/master/docs/img/22.PNG "Setup Step")

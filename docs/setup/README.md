# Setup

## Setup AWS backend
The backend is a highly available REST API implemented using the AWS API Gateway and AWS DynamoDB. No code needs to be deployed anywhere.

The setup consist of 2 steps. 
- A cloudFormation script which will setup DynamoDB and create an IAM Role and policy to grant access to the API
- A manual configuration of API Gateway (as ColudFormation does not yet have support for the API Gateway)

### Create the CloudFormation Stack
The CloudFormation template is located at '/config/CloudFormation.template' in this project. Go to the AWS colsole -> CloudFormation -> CreateStack. Choose the file complete the guide. You can pick any name for the stack.

The stack have created 3 resources: A DynamoDB table, a Role and a Policy. Note the Role Arn as we will use that in the next step.

### Create the API Gateway REST interface

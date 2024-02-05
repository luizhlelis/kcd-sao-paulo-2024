# Infra as code with AWS CDK

This is a project to create an infrastructure as code using AWS CDK. It creates an K8S cluster in your AWS account.

The `cdk.json` file tells the CDK Toolkit how to execute your app.

## Useful commands

* `npm run build`   compile typescript to js
* `npm run watch`   watch for changes and compile
* `npm run test`    perform the jest unit tests
* `npx cdk deploy`  deploy this stack to your default AWS account/region
* `npx cdk diff`    compare deployed stack with current state
* `npx cdk synth`   emits the synthesized CloudFormation template

## Pre requisites

Create an user on AWS IAM with the following policies:

- AdministratorAccess
- AmazonEC2FullAccess
- AmazonEKSClusterPolicy
- AmazonEKSWorkerNodePolicy
- AmazonS3FullAccess
- AmazonVPCFullAccess
- AWSCloudFormationFullAccess

Set the following environment variables, replacing the values with your own:

```bash
export PUBLIC_ACCESS_ALLOWED_ONLY_FROM_CIDR="<public-ip-from-your-machine>/32"
```

Run the following commands:

- To bootstrap the CDK:

```bash
cdk bootstrap
```

- To synthesize the CDK:

```bash
cdk synth
```

- To see the changes that will be applied:

```bash
cdk diff
```

- To deploy the CDK:

```bash
cdk deploy
```

## Services

Read the [README.md](../src/README.md) file inside the `src` folder to know how to run the services locally.

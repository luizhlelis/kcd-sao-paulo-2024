import * as cdk from 'aws-cdk-lib';
import {Construct} from 'constructs';
import * as eks from 'aws-cdk-lib/aws-eks';
import {StackProps} from "aws-cdk-lib";
import {InfraResourceUniqueNames} from "./infra-resource-unique-names";
import {FargateCluster} from "aws-cdk-lib/aws-eks";
import * as iam from "aws-cdk-lib/aws-iam";

export interface KubernetesRbacAwsAuthStackProps extends StackProps {
    fargateCluster: FargateCluster,
}

export class KubernetesRbacAwsAuthStackStack extends cdk.Stack {

    constructor(scope: Construct, id: string, props: KubernetesRbacAwsAuthStackProps) {
        super(scope, id, props);

        const awsAuth = new eks.AwsAuth(this, InfraResourceUniqueNames.awsAuthStack, {
            cluster: props.fargateCluster,
        });

        // awsAuth.addMastersRole(props.fargateCluster.adminRole);
        awsAuth.addUserMapping(iam.User.fromUserName(this, InfraResourceUniqueNames.awsMasterUserName, InfraResourceUniqueNames.awsMasterUserName), {
            groups: [InfraResourceUniqueNames.k8sRbacMasterGroupName],
            username: InfraResourceUniqueNames.k8sMasterUserName
        })
    }
}

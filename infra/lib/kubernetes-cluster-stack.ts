import * as cdk from 'aws-cdk-lib';
import {Construct} from 'constructs';
import * as eks from 'aws-cdk-lib/aws-eks';
import {StackProps} from "aws-cdk-lib";
import {InfraResourceUniqueNames} from "./infra-resource-unique-names";
import {KubectlV28Layer} from '@aws-cdk/lambda-layer-kubectl-v28';
import {FargateCluster} from "aws-cdk-lib/aws-eks";

export interface KubernetesClusterStackProps extends StackProps {
    publicAccessAllowedOnlyFromCIDR: string,
}

export class KubernetesClusterStack extends cdk.Stack {

    readonly fargateCluster: FargateCluster;

    constructor(scope: Construct, id: string, props: KubernetesClusterStackProps) {
        super(scope, id, props);

        const kubectl = new KubectlV28Layer(this, InfraResourceUniqueNames.kubectlLayer);

        this.fargateCluster = new FargateCluster(this, InfraResourceUniqueNames.fargateKubernetesCluster, {
            version: eks.KubernetesVersion.V1_28,
            clusterName: InfraResourceUniqueNames.fargateKubernetesCluster,
            endpointAccess: eks.EndpointAccess.PUBLIC_AND_PRIVATE.onlyFrom(props.publicAccessAllowedOnlyFromCIDR),
            kubectlLayer: kubectl
        });
    }
}

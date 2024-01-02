#!/usr/bin/env node

// Entrypoint for the CDK app

import "source-map-support/register";
import * as cdk from 'aws-cdk-lib';
import {KubernetesClusterStack} from './lib/kubernetes-cluster-stack';
import {InfraResourceUniqueNames} from "./lib/infra-resource-unique-names";

const app = new cdk.App();

if (!process.env.PUBLIC_ACCESS_ALLOWED_ONLY_FROM_CIDR) {
    throw new Error("PUBLIC_ACCESS_ALLOWED_ONLY_FROM_CIDR environment variable must be set");
}

new KubernetesClusterStack(app, InfraResourceUniqueNames.kubernetesClusterStack, {
    publicAccessAllowedOnlyFromCIDR: process.env.PUBLIC_ACCESS_ALLOWED_ONLY_FROM_CIDR
});
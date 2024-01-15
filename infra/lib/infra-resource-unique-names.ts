export {
    InfraResourceUniqueNames
};

class InfraResourceUniqueNames {
    static fargateKubernetesCluster: string = "kcd-sao-paulo-fargate-cluster";
    static kubernetesClusterStack: string = "kcd-sao-paulo-cluster-stack";
    static kubectlLayer: string = "kcd-sao-paulo-cluster-kubectl-layer";
    static awsAuthStack: string = "aws-auth-stack";
    static awsMasterUserName: string = "kcd-sao-paulo-2024";
    static k8sMasterUserName: string = "kcd-sao-paulo-2024";
    static k8sRbacMasterGroupName: string = "system:masters";
}

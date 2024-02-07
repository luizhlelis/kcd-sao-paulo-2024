# kcd-sao-paulo-2024

## Pre requisites

- For the OTEL example with docker, you'll need to have docker installed in your machine. You can follow the instructions [here](https://docs.docker.com/get-docker/).
- For the OTEL example with operator, you'll need a kubernetes cluster, you can use [minikube](https://minikube.sigs.k8s.io/docs/handbook/config/)
or your own kubernetes cluster. [Here](./infra/README.md) you can find how to create an AWS EKS cluster using CDK.

## Useful commands

To start the mini-kube cluster, run:

```bash
minikube start
```

The OTEL operator requires a TLS certificate manager to be installed in the cluster. To install [cert-manager](https://cert-manager.io/docs/installation/)
in your cluster, run:

```bash
kubectl apply -f https://github.com/cert-manager/cert-manager/releases/download/v1.14.1/cert-manager.yaml
```

To install the OTEL operator, run:

```bash
kubectl apply -f https://github.com/open-telemetry/opentelemetry-operator/releases/latest/download/opentelemetry-operator.yaml
```

The command above will install the latest version of the OTEL operator, create a new namespace called `opentelemetry-operator-system` and deploy the operator in it.

Create a new namespace for the applications:

```bash
kubectl create namespace all-applications
```

Build the docker images for the services and push them to minikube's local registry:

```bash
eval $(minikube docker-env)
docker-compose -f src/docker-compose.yml build order-api
```

Deploy the services to the kubernetes cluster:

```bash
kubectl apply -f infra/recipes/order/web-deployment.yaml
kubectl apply -f infra/recipes/order/configmap.yaml
kubectl apply -f infra/recipes/order/secret.yaml
```

## Services

Read the [README.md](./src/README.md) file inside the `src` folder to know how to run the services locally outside of docker or kubernetes.

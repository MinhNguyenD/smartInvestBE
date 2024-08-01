Write-Host "Running cluster setup..."

aws eks --region us-east-1 update-kubeconfig --name smartinvest-cluster

helm upgrade --install ingress-nginx ingress-nginx --repo https://kubernetes.github.io/ingress-nginx --namespace ingress-nginx --create-namespace
kubectl apply -f ../kubernetes/
# Get the Ingress Controller IP and perform actions
$KubernetesServiceName = "ingress-nginx-controller"
$Namespace = "ingress-nginx"
$IngressIP = kubectl get svc ingress-nginx-controller -n ingress-nginx -o jsonpath='{.status.loadBalancer.ingress[0].hostname}'

Write-Host "Ingress IP: $IngressIP"
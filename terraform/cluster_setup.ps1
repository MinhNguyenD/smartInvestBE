Write-Host "Running cluster setup..."

aws eks --region us-east-1 update-kubeconfig --name smartinvest-cluster

helm upgrade --install ingress-nginx ingress-nginx --repo https://kubernetes.github.io/ingress-nginx --namespace ingress-nginx --create-namespace --set controller.service.annotations."service\.beta\.kubernetes\.io/aws-load-balancer-type"="nlb" --set controller.service.annotations."service\.beta\.kubernetes\.io/aws-load-balancer-eip-allocations"="eipalloc-0c361d8069471bb0d"

# Retrieve load balancer endpoint 
$domain = kubectl get svc ingress-nginx-controller -n ingress-nginx -o jsonpath='{.status.loadBalancer.ingress[0].hostname}'
Write-Host "Domain: $domain"
$lbName = $domain -split "-" | Select-Object -First 1
Write-Host "Load Balancer name: $lbName"
$lbStatus = ""

# Wait for load balancer to be done provisioning before starting kubertnete workload
while ($lbStatus -ne "active") {
    $lbStatus = (aws elbv2 describe-load-balancers --region us-east-1 --names $lbName --query 'LoadBalancers[0].State.Code' --output text)
    Write-Host "Current Load Balancer Status: $lbStatus"
    if ($lbStatus -eq "active") {
        Write-Host "Load Balancer is active."
    } else {
        Write-Host "Waiting for Load Balancer to become active..."
        Start-Sleep -Seconds 60
    }
}

# Start the workload with deployments and services
kubectl apply -f ../kubernetes/
resource "aws_eks_cluster" "smartinvest-cluster" {
  name     = "smartinvest-cluster"
  role_arn = data.aws_iam_role.LabRole.arn

  vpc_config {
    subnet_ids = [aws_subnet.smartinvest-private-subnet-1a.id, aws_subnet.smartinvest-private-subnet-1b.id]
  }
  tags = {
    Name="smartinvest-cluster"
  }
}

resource "null_resource" "cluster_setup" {
  # Ensure this resource runs after the cluster creation
  depends_on = [aws_eks_cluster.smartinvest-cluster, aws_eks_node_group.smartinvest-eks-ng]

  triggers = {
    always_run = timestamp()
  }
  provisioner "local-exec" {
    command = "powershell.exe -File ./cluster_setup.ps1"
  }
}
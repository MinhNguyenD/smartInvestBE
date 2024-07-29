resource "aws_eks_node_group" "example" {
  cluster_name    = aws_eks_cluster.smartinvest-cluster.name
  node_group_name = "smartinvest-wg"
  node_role_arn   = data.aws_iam_role.LabRole.arn
  subnet_ids      = [aws_subnet.smartinvest-private-subnet-1a.id, aws_subnet.smartinvest-private-subnet-1b.id]
  capacity_type = "SPOT"
  disk_size = 10
  ami_type = "AL2_x86_64"
  instance_types = [ var.instance_type ]
  scaling_config {
    desired_size = 2
    max_size     = 3
    min_size     = 1
  }

  update_config {
    max_unavailable = 1
  }
  tags = {
    Name = "smartinvest-wg"
  }
}
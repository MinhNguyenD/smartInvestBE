# module "eks" {
#   source  = "terraform-aws-modules/eks/aws"
#   version = "20.20.0"

#   cluster_name    = "smartinvest-cluster"
#   cluster_version = "1.30"

#   cluster_endpoint_public_access           = true
#   enable_cluster_creator_admin_permissions = true

#   vpc_id     = aws_vpc.main.id
#   subnet_ids = [aws_subnet.smartinvest-private-subnet-1a.id, aws_subnet.smartinvest-private-subnet-1b.id]
#   eks_managed_node_group_defaults = {
#     ami_type = "AL2_x86_64"
#     instance_types = [var.instance_type]

#   }

#   eks_managed_node_groups = {
#     smartinvest-cluster-wg = {
#       name = "smartinvest-cluster-wg"
#       capacity_type  = "SPOT"
#       instance_types = ["t3.small"]
#       min_size     = 1
#       max_size     = 3
#       desired_size = 2
#       tags = {
#         Name = "smartinvest-cluster-wg"
#       }
#     }
#   }
# }
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
data "aws_iam_role" "EC2FERole" {
  name = "EC2FERole"
}

data "aws_iam_role" "EC2NodeRole" {
  name = "EC2NodeRole"
}

data "aws_iam_role" "EKSClusterRole" {
  name = "EKSClusterRole"
}

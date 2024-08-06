resource "aws_ecr_repository" "smartinvest-ecr-fe" {
  name = "smartinvest-ecr-repo"
  tags = {
    name="smartinvest-ecr-repo"
  }
}

resource "aws_ecr_repository" "smartinvest-ecr-be" {
  name = "smartinvest-ecr-repo"
  tags = {
    name="smartinvest-ecr-repo"
  }
}
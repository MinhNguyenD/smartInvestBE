resource "aws_ecr_repository" "smartinvest-ecr-fe" {
  name = "smartinvest-ecr-fe"
  tags = {
    name="smartinvest-ecr-fe"
  }
}

resource "aws_ecr_repository" "smartinvest-ecr-be" {
  name = "smartinvest-ecr-be"
  tags = {
    name="smartinvest-ecr-be"
  }
}
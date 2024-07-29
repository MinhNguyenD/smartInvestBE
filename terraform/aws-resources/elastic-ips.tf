resource "aws_eip" "ec2-fe_eip" {
  domain = "vpc"
  tags = {
    Name = "smartinvest-vpc-eip"
  }
}

resource "aws_eip" "natgw-ip" {
  domain           = "vpc"
  tags = {
    Name = "smartinvest-vpc-eip"
  }
}
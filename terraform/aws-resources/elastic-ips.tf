resource "aws_eip" "natgw-ip" {
  domain           = "vpc"
  tags = {
    Name = "smartinvest-vpc-eip"
  }
}
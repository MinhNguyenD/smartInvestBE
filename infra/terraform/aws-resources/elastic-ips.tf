resource "aws_eip" "smartinvest-natgw-eip" {
  domain           = "vpc"
  tags = {
    Name = "smartinvest-natgw-eip"
  }
}
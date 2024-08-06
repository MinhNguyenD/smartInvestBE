resource "aws_nat_gateway" "smartinvest-natgw" {
  # elastic ip 
  allocation_id = aws_eip.smartinvest-natgw-eip.id
  subnet_id     = aws_subnet.smartinvest-public-subnet-1a.id

  tags = {
    Name = "smartinvest-natgw"
  }

  # To ensure proper ordering, add an explicit dependency on the Internet Gateway for the VPC.
  depends_on = [aws_eip.smartinvest-natgw-eip, aws_internet_gateway.smartinvest-ig]
}
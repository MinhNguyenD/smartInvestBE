resource "aws_internet_gateway" "smartinvest-ig" {
  vpc_id = aws_vpc.main.id
  tags = {
    Name = "smartinvest-ig"
  }
}
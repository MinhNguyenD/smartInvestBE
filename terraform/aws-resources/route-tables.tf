resource "aws_route_table" "smartinvest-public-rtb" {
  vpc_id = aws_vpc.main.id

  route {
    cidr_block = "0.0.0.0/0"
    gateway_id = aws_internet_gateway.smartinvest-ig.id
  }

  route {
    cidr_block = "10.0.0.0/16"
    gateway_id = "local"
  }

  tags = {
    Name = "smartinvest-public-rtb"
  }
}

resource "aws_route_table" "smartinvest-private-rtb" {
  vpc_id = aws_vpc.main.id

  route {
    cidr_block = "0.0.0.0/0"
    gateway_id = aws_internet_gateway.smartinvest-ig.id
  }

  route {
    cidr_block = "10.0.0.0/16"
    gateway_id = "local"
  }

  tags = {
    Name = "smartinvest-private-rtb"
  }
}

# Associate the route table with the subnet
resource "aws_route_table_association" "associate_public_1a" {
  subnet_id      = aws_subnet.smartinvest-public-subnet-1a.id
  route_table_id = aws_route_table.smartinvest-public-rtb.id
}

resource "aws_route_table_association" "associate_private_1a" {
  subnet_id      = aws_subnet.smartinvest-private-subnet-1a.id
  route_table_id = aws_route_table.smartinvest-private-rtb.id
}

resource "aws_route_table_association" "associate_private_1b" {
  subnet_id      = aws_subnet.smartinvest-private-subnet-1b.id
  route_table_id = aws_route_table.smartinvest-private-rtb.id
}
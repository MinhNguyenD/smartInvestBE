resource "aws_subnet" "smartinvest-public-subnet-1a" {
  vpc_id     = aws_vpc.main.id
  cidr_block = "10.0.0.0/24"
  availability_zone = "us-east-1a"
  tags = {
    Name = "smartinvest-public-subnet-1a"
  }
}

resource "aws_subnet" "smartinvest-private-subnet-1a" {
  vpc_id     = aws_vpc.main.id
  cidr_block = "10.0.1.0/24"
  availability_zone = "us-east-1a"
  tags = {
    Name = "smartinvest-private-subnet-1a"
  }
}


resource "aws_subnet" "smartinvest-private-subnet-1b" {
  vpc_id     = aws_vpc.main.id
  cidr_block =  "10.0.2.0/24"
  availability_zone = "us-east-1b"
  tags = {
    Name = "smartinvest-private-subnet-1b"
  }
}
resource "aws_subnet" "smartinvest-public-subnet-1a" {
  vpc_id     = aws_vpc.main.id
  cidr_block = "10.0.0.0/24"
  availability_zone = "us-east-1a"
  map_public_ip_on_launch = true 
  tags = {
    Name = "smartinvest-public-subnet-1a"
  }
}

resource "aws_subnet" "smartinvest-private-subnet-1a" {
  vpc_id     = aws_vpc.main.id
  cidr_block = "10.0.1.0/24"
  availability_zone = "us-east-1a"
  map_public_ip_on_launch = false
  tags = {
    Name = "smartinvest-private-subnet-1a"
    "kubernetes.io/role/internal-elb" = 1
  }
}


resource "aws_subnet" "smartinvest-private-subnet-1b" {
  vpc_id     = aws_vpc.main.id
  cidr_block =  "10.0.2.0/24"
  availability_zone = "us-east-1b"
  map_public_ip_on_launch = false
  tags = {
    Name = "smartinvest-private-subnet-1b"
    "kubernetes.io/role/internal-elb" = 1
  }
}
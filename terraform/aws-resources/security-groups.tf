resource "aws_security_group" "smartinvest-ec2-fe-sg" {
  name        = "smartinvest-ec2-fe"
  description = "Allow HTTPs traffic to EC2 front end"
  vpc_id      = aws_vpc.main.id
  ingress {
    from_port        = 80
    to_port          = 80
    protocol         = "tcp"
    cidr_blocks      = ["0.0.0.0/0"]
  }
  ingress {
    from_port        = 443
    to_port          = 443
    protocol         = "tcp"
    cidr_blocks      = ["0.0.0.0/0"]
  }
  ingress {
    from_port        = 22
    to_port          = 22
    protocol         = "tcp"
    cidr_blocks      = ["0.0.0.0/0"]
  }
  egress {
    from_port        = 0
    to_port          = 0
    protocol         = "-1"
    cidr_blocks      = ["0.0.0.0/0"] # all main vpc traffic
  }
  tags = {
    Name = "smartinvest-ec2-fe"
  }
}
resource "aws_instance" "smartinvest-fe" {
  ami           = "ami-0427090fd1714168b"
  instance_type = "t2.micro"
  user_data = file("user-data-ec2-fe.sh")
  subnet_id = aws_subnet.smartinvest-public-subnet-1a.id
  security_groups = [aws_security_group.smartinvest-ec2-fe.id]
  iam_instance_profile = aws_iam_instance_profile.ec2_instance_profile.name
  tags = {
    Name = "smartinvest-fe"
  }
}

resource "aws_eip_association" "eip_assoc" {
  instance_id = aws_instance.smartinvest-fe.id
  allocation_id = aws_eip.ec2-fe_eip.id
}

data "aws_iam_role" "LabRole" {
  name = "LabRole"
}

resource "aws_iam_instance_profile" "ec2_instance_profile" {
  name = "ec2_instance_profile"
  role = data.aws_iam_role.LabRole.name
}
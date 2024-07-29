resource "aws_instance" "smartinvest-fe" {
  ami           = var.instance_ami
  instance_type = var.instance_type
  user_data = file("user-data-ec2-fe.sh")
  subnet_id = aws_subnet.smartinvest-public-subnet-1a.id
  security_groups = [aws_security_group.smartinvest-ec2-fe-sg.id]
  iam_instance_profile = aws_iam_instance_profile.ec2_instance_profile.name
  tags = {
    Name = "smartinvest-fe"
  }
}

resource "aws_eip_association" "eip_assoc" {
  instance_id = aws_instance.smartinvest-fe.id
  allocation_id = aws_eip.ec2-fe_eip.id
}

resource "aws_iam_instance_profile" "ec2_instance_profile" {
  name = "ec2_instance_profile"
  role = data.aws_iam_role.LabRole.name
}
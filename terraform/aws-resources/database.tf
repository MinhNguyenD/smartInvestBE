resource "aws_db_instance" "smartinvest-rds-user" {
  identifier = "smartinvest-rds-user"
  allocated_storage    = 10
  db_name              = "user"
  engine               = "mysql"
  port                 = 3306
  engine_version       = "8.0.35"
  instance_class       = "db.t3.micro"
  username             = "minh"
  password             = ""
  db_subnet_group_name = aws_db_subnet_group.rds-subnet-group.name
  skip_final_snapshot  = true
  parameter_group_name = "default.mysql8.0"
  vpc_security_group_ids = [aws_security_group.smartinvest-rds-sg.id]
  tags = {
    Name="smartinvest-rds-user"
  }
}

resource "aws_db_instance" "smartinvest-rds-portfolio" {
  identifier = "smartinvest-rds-portfolio"
  allocated_storage    = 10
  db_name              = "mydb"
  engine               = "mysql"
  port                 = 3306
  engine_version       = "8.0.35"
  instance_class       = "db.t3.micro"
  username             = "minh"
  password             = ""
  db_subnet_group_name = aws_db_subnet_group.rds-subnet-group.name
  vpc_security_group_ids = [aws_security_group.smartinvest-rds-sg.id]
  skip_final_snapshot  = true
  parameter_group_name = "default.mysql8.0"
  tags = {
    Name="smartinvest-rds-portfolio"
  }
}
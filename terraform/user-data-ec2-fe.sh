#!/bin/bash
sudo yum update -y
sudo yum install docker -y
echo "Starting Docker installation" >> /var/log/user-data.log
sudo systemctl start docker
sudo systemctl enable docker
$(aws ecr get-login-password --region us-east-1 | sudo docker login --username AWS --password-stdin 309238126949.dkr.ecr.us-east-1.amazonaws.com)
echo "Starting Docker installation" >> /var/log/user-data.log
sudo docker pull 309238126949.dkr.ecr.us-east-1.amazonaws.com/smartinvestfe:smartinvestfe
sudo docker run -d -p 80:80 309238126949.dkr.ecr.us-east-1.amazonaws.com/smartinvestfe:smartinvestfe
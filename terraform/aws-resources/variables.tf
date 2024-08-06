variable "vpc_cidr_block" {
  type = string
  default = "10.0.0.0/16"
}

variable "subnet_cidr_block" {
  type = string
  default = "10.0.0.0/24"
}

variable "region" {
  type = string
  default = "us-east1"
}

variable "az" {
  type = set(string)
  default = ["us-east-1a","us-east-1b"]
}

variable "instance_type" {
  type = string
  default = "t2.micro"
}

variable "instance_ami" {
  type = string
  default = "ami-0427090fd1714168b"
}
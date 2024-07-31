terraform {
 required_providers {
    aws = {
      source = "hashicorp/aws"
      version = "5.60.0"
    }
    null = {
      source = "hashicorp/null"
      version = "3.2.2"
    }
  }
}
provider "aws" {
    region = "us-east-1"
}
provider "null" {
  # Configuration options
}
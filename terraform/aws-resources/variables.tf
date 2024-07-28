variable "vpc_cidr_block" {
  type = string
  default = "10.0.0.0/16"
}

variable "subnet_cidr_block" {
  type = string
  default = "10.0.0.0/24"
}

variable "registry_credential" {
  type = string
  default = "registry_credential"
}

variable "registry_credential_provider" {
  type = string
  default = "registry_credential_provider"
}

variable "region" {
  type = string
  default = "us-east1"
}
variable "az" {
  type = set(string)
  default = ["us-east-1a","us-east-1b","us-east-1c"]
}
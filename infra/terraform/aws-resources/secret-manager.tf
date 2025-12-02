### Name
data "aws_secretsmanager_secret" "smartinvest-user-rds-secret" {
  arn = "arn:aws:secretsmanager:us-east-1:309238126949:secret:Production_User_ConnectionStrings__DbConnectionString-LUGkoJ"
}

data "aws_secretsmanager_secret" "smartinvest-portfolio-rds-secret" {
  arn = "arn:aws:secretsmanager:us-east-1:309238126949:secret:Production_Portfolio_ConnectionStrings__DbConnectionString-oUMRhI"
}

data "aws_secretsmanager_secret" "smartinvest-user-jwt-secret" {
  arn = "arn:aws:secretsmanager:us-east-1:309238126949:secret:Production_User_JWT__Key-dww8OH"
}

data "aws_secretsmanager_secret" "smartinvest-portfolio-jwt-secret" {
  arn = "arn:aws:secretsmanager:us-east-1:309238126949:secret:Production_Portfolio_JWT__Key-6r9ZGF"
}

data "aws_secretsmanager_secret" "smartinvest-portfolio-marketdata-secret" {
  arn = "arn:aws:secretsmanager:us-east-1:309238126949:secret:Production_Portfolio_MarketData__Key-xCmygR"
}

data "aws_secretsmanager_secret" "smartinvest-portfolio-OpenAI-secret" {
  arn = "arn:aws:secretsmanager:us-east-1:309238126949:secret:Production_Portfolio_OpenAI__Key-hp5dfE"
}


# resource "aws_secretsmanager_secret" "smartinvest-user-rds-secret" {
#   name = "Production_User_ConnectionStrings__DbConnectionString"
#   description = "Connection string for user database"
#   lifecycle {
#     prevent_destroy = true
#   }
# }

# resource "aws_secretsmanager_secret" "smartinvest-portfolio-rds-secret" {
#   name = "Production_Portfolio_ConnectionStrings__DbConnectionString"
#   description = "Connection string for portfolio database"
#   lifecycle {
#     prevent_destroy = true
#   }
# }

# resource "aws_secretsmanager_secret" "smartinvest-user-jwt-secret" {
#   name = "Production_User_JWT__Key"
#   description = "JWT key for user service"
#   lifecycle {
#     prevent_destroy = true
#   }
# }

# resource "aws_secretsmanager_secret" "smartinvest-portfolio-jwt-secret" {
#   name = "Production_Portfolio_JWT__Key"
#   description = "JWT key for portfolio service"
#   lifecycle {
#     prevent_destroy = true
#   }
# }

# resource "aws_secretsmanager_secret" "smartinvest-portfolio-marketdata-secret" {
#   name = "Production_Portfolio_MarketData__Key"
#   description = "Market data api key"
#   lifecycle {
#     prevent_destroy = true
#   }
# }

# resource "aws_secretsmanager_secret" "smartinvest-portfolio-OpenAI-secret" {
#   name = "Production_Portfolio_OpenAI__Key"
#   description = "OpenAI api key"
#   lifecycle {
#     prevent_destroy = true
#   }
# }

### Value
resource "aws_secretsmanager_secret_version" "smartinvest-user-rds-secret-version" {
  secret_id     = data.aws_secretsmanager_secret.smartinvest-user-rds-secret.id
  secret_string = "server=${aws_db_instance.smartinvest-rds-user.address};port=3306;database=${aws_db_instance.smartinvest-rds-user.db_name};uid=${aws_db_instance.smartinvest-rds-user.username};password=${aws_db_instance.smartinvest-rds-user.password}"
}

resource "aws_secretsmanager_secret_version" "smartinvest-portfolio-rds-secret-version" {
  secret_id     = data.aws_secretsmanager_secret.smartinvest-portfolio-rds-secret.id
  secret_string = "server=${aws_db_instance.smartinvest-rds-portfolio.address};port=3306;database=${aws_db_instance.smartinvest-rds-portfolio.db_name};uid=${aws_db_instance.smartinvest-rds-portfolio.username};password=${aws_db_instance.smartinvest-rds-portfolio.password}"
}

resource "aws_secretsmanager_secret_version" "smartinvest-user-jwt-secret-version" {
  secret_id     = data.aws_secretsmanager_secret.smartinvest-user-jwt-secret.id
  secret_string = ""
}

resource "aws_secretsmanager_secret_version" "smartinvest-portfolio-jwt-secret-version" {
  secret_id     = data.aws_secretsmanager_secret.smartinvest-portfolio-jwt-secret.id
  secret_string = ""
}

resource "aws_secretsmanager_secret_version" "smartinvest-portfolio-marketdata-secret-version" {
  secret_id     = data.aws_secretsmanager_secret.smartinvest-portfolio-marketdata-secret.id
  secret_string = ""
}

resource "aws_secretsmanager_secret_version" "smartinvest-portfolio-OpenAI-secret-version" {
  secret_id     = data.aws_secretsmanager_secret.smartinvest-portfolio-OpenAI-secret.id
  secret_string = ""
}
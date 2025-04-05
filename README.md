# Overview
For this project, I developed a cloud-native, microservice, AI-powered stock analyzer SaaS app that allows users to browse stocks, manage stocks with portfolios, and perform detailed stock analysis using GPT 4.0-mini with the retrieved data from Market Data APIs. The application aims to help users have easier and more convenient access to investing with a detailed explanation of the stock performance of AI from the data. Users can browse any stocks, ETFs, and companies… on the market. Users can also keep their favorite stocks on their watchlist, saving them to their portfolio. The users can request to have detailed analyses provided by GPT 4.0-mini from the stock data such as income statements, key metrics, and real-time tradings...  Finally, when the analyses are completed, users can email these analyses to their account emails. The application will add more features and optimize the architecture in the future.
Please view the front end of this project at [SmartInvestFE](https://github.com/MinhNguyenD/SmartInvestFE)


# Architecture
## Architecture Diagram
![image](https://github.com/user-attachments/assets/563e9d4b-f85f-415a-a4eb-ca7a1ddc5b2e)

## Cloud Mechanism
### Networking 
The architecture is hosted on a VPC where it has 1 public subnet and 2 private subnets, spanning 2 Availability Zones us-east-1a and us-east-1b. The VPC attaches an AWS Internet Gateway that allows the subnet to communicate with the public internet using the route table. The private subnets use a NAT gateway that is placed on the public subnet to get access to the internet to install necessary dependencies. Additionally, there are 2 route tables for public and private subnets which 1) route traffic from the public subnet to the Internet to the Internet Gateway and 2) route traffic from private subnets to the Internet to the NAT Gateway.
### Front end
The front-end application is deployed on an EC2 instance which runs the containerized React application with the Nginx web server. The EC2 is placed in the public subnet that allows internet access. More than that, the EC2 is configured with the security group to allow only HTTP, HTTPS, and SSH traffic from the public. It also contains a user data script to install docker and run the front-end application image. The front-end browser communicates with the back end through the public IP address of the Load Balancer put in the public subnet which will distribute traffic to the back-end EKS cluster. 
### Back end
The microservice back-end application is deployed on the AWS EKS cluster placed in the private subnets. The ASP.NET Core microservices are containerized and stored in ECR. Then the EKS cluster runs microservices application pulled from ECR with deployment workloads and corresponding ClusterIP services. The EKS cluster is also configured with a public-facing nginx-ingress-controller which includes a Load Balancer. The Kubernetes cluster communicates with AWS API to create an AWS Network Load Balancer to distribute traffic from the internet to the cluster. The traffic then is routed to the destinated running pods with attached services by an ingress that defines the routing rules. The deployed microservices communicate with the database by using the RDS database connection string that is stored in AWS Secret Manager. More than that, other sensitive data used by the back end such as the OpenAI key, Market Data API Key, JWT key, and RDS credentials are also stored in Secret Manager and accessed using AWS SDK. Finally, the back-end feature that supports email analysis to users uses AWS SNS to create topics and notify subscribers/users via email through AWS SDK.
### Database
The database solution for this application is AWS RDS MySQL which is located in the private subnets where no public traffic can directly access the database. Additionally, the databases are also configured with security groups that only allow internal VPC traffic with a 3306 port for inbound. Data returned from the back end (outbound) is limited to only internal VPC IPs.   There are 2 RDS databases dedicated to 2 microservices from the back end where each service owns an independent database. Each service accesses the database by using the connection string stored in AWS Secret Manager.

## Data Store
Data is securely stored in 2 AWS RDS MySQL databases, where each microservice has an independent database. The databases are placed in the private subnet in the us-east-1a and us-east-1b availability zones, preventing any access from the public internet. In addition to private subnets, the databases are also secured with security groups that only accept traffic with port 3306 from the internal VPC. The databases’ outbounds are also limited to internal VPC. 

## Deployment
The application is a full-stack web app with the front end and back end deployed differently on AWS. The front-end React application is containerized with an Nginx web server and stored in AWS ECR. The front end is hosted on an EC2 instance, where it pulls and runs the application image. Finally, the front-end content is served with the Nginx web server to users that access the public IP address of the EC2 instance. The back end follows microservice architecture where it has 2 microservices. Each microservice is containerized and the images are also stored in AWS ECR. The microservice back end is deployed on AWS EKS where it uses the Kubernetes deployment file to start the application workload. 

# Services 
## Application Services
As mentioned above, the application is hosted and maintained completely on AWS Cloud. The application utilized 6 main AWS services which are categorized into 4 types Computing, Database, Network, and General.
Computing: 
AWS EC2: EC2 is used to host containerized front-end React applications with the Nginx web server. 
AWS EKS: The service is utilized to host a microservice back end ASP.NET Core APIs. The EKS cluster contains one worker node group with auto-scaled EC2 instances. The cluster also contains pods that run the containerized microservices which have ClusterIP for internal communication. More than that, for effective routing and traffic control, the cluster has an nginx-ingress controller (with Network Load Balancer) with ingress rules.     

## Database: 
AWS RDS: The application choice of database is AWS RDS with MySQL engine. There are 2 separate AWS RDS for 2 independent microservices. The user database is used for user management service, and the Portfolio database is used for portfolio management service.

## Network:
AWS VPC: For security purposes, the application architecture uses a private isolated virtual network. The network contains 1 public subnet for front-end applications and 2 private subnets for the EKS cluster and databases.

## General:
AWS SNS: The application utilizes AWS SNS as an email server to send emails when users request email stock analyses.  
AWS Secret Manager: Secret Manager is used to securely store sensitive data such as Open AI key, RDS credentials, JWT key, and Market Data API key as secrets. 
AWS ECR: ECR is a fully managed Docker container registry that is used to store and manage container images.


# Security at all layers
## VPC layer
The application is hosted entirely on the isolated VPC within AWS Public Cloud. This layer of security ensures that users’ clouds are separated from each other and the Internet, adding more security for the application. 

## Subnet layer
### Public Subnet (Front end EC2)
A part of the VPC, through the use of Internet Gateway and route table, allows exposure to the Internet, which is suitable for resources that need direct access to the Internet. The front end of the application requires traffic from the users on the Internet to access the front-end content and the ability to respond to the corresponding request. Therefore, placing the EC2 instance that hosted the front end is justified because it only exposes front-end content with no internal data included. More than that, although the front-end EC2 is exposed to the Internet, another security layer of the security group is added which will be discussed below.
### Private Subnet (Back end, Database)
The private subnet is part of the VPC that prevents exposure to the Internet. With the NAT gateway, resources in a private subnet allow communications to the Internet only for outbound, meaning no direct access from the Internet to the private subnet is allowed. With this mechanism, the back end and database resources can install any dependencies or tools from the Internet while keeping them protected from direct inbound Internet traffic. However, the back-end API of this application is exposed to the public by the Nginx ingress network load balancer. The reason is that the front end uses Client-side rendering (CSR) meaning the requests to the back-end APIs are from the users’ browsers on the Internet. Although back-end APIs are exposed to the public, a security group layer is added to ensure only certain types of access are allowed.

## Security Group Layer (Front end, Back end, Database)
This layer of security is the last level to access IT resources. These security groups act as a virtual firewall to gatekeep the inbound and outbound traffic, providing fine-grained control of what traffic is allowed to access the instances. Security groups are added to a front-end EC2 instance, back-end cluster nodes, and RDS instances to protect these resources and their data. 
### Front End EC2 security group
Inbound: Only allows HTTP, HTTPS, and SSH traffic 
Outbound: Allows any traffic including public
### EKS cluster nodes security group
Inbound: Only allows internal VPC HTTP, HTTPS, and SSH traffic 
Outbound: Only allows internal VPC traffic
### RDS security group
Inbound: Only allows internal VPC traffic with port 3306
Outbound: Only allows internal VPC traffic


# Future Consideration
In the future, the plan is to continue development and implement the following features
1) Stock trading simulation: Allows users to buy/sell stock, and manage profit/loss of portfolio with real-time trading prices. 
2) Trading social: Create a social platform for investors to discuss stocks, share portfolios, and chat with others. 
3) CI/CD: Automate the integration and delivery of the product development process on AWS Cloud. 

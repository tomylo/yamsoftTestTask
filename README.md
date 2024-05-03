# Documentation

## About
In this project implemented API service for Login and Client console application to consume API .

## Projects

API - REST API server. 
Contracts - Models for API
Client- console application client of API
DAL - Database layer
DBRepo - Repositories for DB 
Shared- logic, which shared between projects ( for example configurations )


## Configuration
In Project Shared /Configuration/ there 2 config files  ysconfiguration.json and ysconfiguration_dev.json  
ysconfiguration_dev.json for Dev env, ysconfiguration.json - for productions. This configurations used in API and in DB projects (for migration)
In config file can be configurable Database connection string, JWT settings, Settings for Logger (Serilog) and configuraiton for checking and Seeding data.


## How to run
Project can be runned from visual studio. Database which used for dev purposes are accesible for everyone. 
In case if Database will be changed. Needs to apply migrations and Seed Data (for user and roles).

## Default user
By default for login can be used "admin@example.com" as login and "AdminPassword123!" as password ( both of them used without quotes).






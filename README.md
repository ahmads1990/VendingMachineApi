# Vending machine REST api 🏧➡️🍬

Short description here

## Quick start

Link here if deployed on a external service

## Table of Contents

-   [Introduction](#introduction)
-   [Technologies](#technologies)
-   [Features](#features)
    -   [Future features](#future-features)
-   [Getting Started](#getting-started)
    -   [Prerequisites](#prerequisites)
    -   [Installation](#installation)
-   [Project walkthrough](#project-walkthrough)
-   [Contributing](#contributing)
    -   [Suggesting ideas](#suggesting-ideas)
    -   [How to Contribute](#how-to-contribute)
    -   [Reporting Bugs](#reporting-bugs)

## Introduction

This project is a vending machine rest api done as a FlapKap’s Backend challenge

## Technologies

<a href="https://skillicons.dev">
<img src="https://skillicons.dev/icons?i=cs,dotnet,postman,git,github" />
</a>

Additional used

-   Nunit
-   Serilog
-   Entity framework
-   Sql server
-   Identity

## Outline

For better quality check the file ["Outline.jpg"](Outline.jpg)
![Project Outline](Outline.jpg)

## Features [🔼](#table-of-contents)

Highlight the key features of the project

-   CRUD for users
    -   Login\Register using jwt bearer token
-   CRUD for product
-   Buy products
-   Deposit
-   Reset

## Getting Started [🔼](#table-of-contents)

### Prerequisites

    -   Git
    -   DotNet 8
    -   SqlServer

### Postman

I have created a Postman collection with available endpoints. You can use it as a reference on how to use the API. Find the documentation [here](https://documenter.getpostman.com/view/29637594/2s9Yyy9ea4).

### Installation

Steps to install and run this project locally

-   Step 1 clone project in terminal paste:
    ```bash
    git clone github.com/ahmads1990/VendingMachineApi
    ```
-   Step 2 setup configuration

    ```json
    {
    	"ConnectionStrings": {
    		"DefaultConnection": "Your connection string"
    	}
    }
    ```

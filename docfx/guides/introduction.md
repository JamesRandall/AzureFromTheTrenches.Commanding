# Introduction

AzureFromTheTrenches.Commanding is a configuration based asynchronous command mediator framework with a number of key design goals:

* To provide a highly performant mediator for simple usage
* To support evolution across a projects lifecycle allowing for easy decomposition from a modular-monolith to a fully distributed micro-service architecture
* To provide a none-leaking abstraction over command dispatch and execution semantics
* To reduce boilerplate code - simplistically less code means less errors and less to maintain

To support these goals the framework supports .NET Standard 2.0 (and higher) and so can be used in a wide variety of scenarios and a number of fully optional extension packages are available to enable:

* Building a REST API directly from commands using a configuration based approach
* Dispatching commands to queues (Service Bus Queues and Topics, and Azure Storage)
* Dispatching commands to event hubs
* Using queues as a source for executing commands 
* Caching commands based on signatures in local memory caches or Redis caches

For an introduction on taking the Commanding and Mediator approach to application architecture using this framework [please see this series of posts here](https://www.azurefromthetrenches.com/c-cloud-application-architecture-commanding-with-a-mediator-the-full-series/).

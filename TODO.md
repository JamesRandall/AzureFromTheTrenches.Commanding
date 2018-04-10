# vNext - work in progress

## ASP.Net Core REST commanding

* Documentation

## General

* Ensure storage queues can handle commands with no response

* Ensure all extensions for storage have "hygiene" work done - various factories as per service bus

## Done

* Allow a mediator decorator to influence the respones type of the controller

* Allow additional assemblies to be routed through to the SyntaxTreeCompiler for inclusion as MetaDataReferences

* Register attributes such as Authorize at the controller or Action level



# vNext + 1

* Allow Polly to be used in the command pipeline - e.g. place a circuit breaker in front of a command handler. Probably through an abstract policy wrapper and allowing for policies to be registered against command types.

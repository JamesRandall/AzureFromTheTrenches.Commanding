# Philosophy - Why Develop this Framework?

I began work on the framework when I found myself facing some challenging constraints on projects with limited budgets, very small teams, and an initial need to keep things flexible while establishing market fit but knowing I would need to tease the systems apart into distributed micro-service architectures connected typically via events, brokered messages and REST APIs, without the budget to significantly rewrite system components.

Given that operations in a distributed system are essentially expressed as state then adopting that approach for in-process operations seemed to be a good starting point if I wanted to enable and that led me to the mediator pattern.

I looked at the popular [Mediatr](https://github.com/jbogard/MediatR) framework however given my own goals I was concerned that it did not pull apart dispatch and execution - something I felt would be important if I wanted to execute commands over HTTP connections and queues.

All that being the case I began work on this framework. Of course I didn't get all this implemented in version 1 - it's rather been a steady progression based on experience with the framework and feedback that has got it to it's current state. Lots of things have been added and changed and a few ideas dropped but the core has been stable for a while now and recent work has been about adding additional extensions.
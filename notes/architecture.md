# Architecture

> Notes around how to architect Orleans applications

## Suitable Apps

Consider Orleans for:

- Lots of entities in the system where a subset of them are active at any point of time
- Entities small enough to be single threaded
- Workload is interactive

More reading:

- [Best practises in Orleans](https://learn.microsoft.com/en-us/dotnet/orleans/resources/best-practices)

## Patterns

- Registry Grain: Keeps track of a set of other grains
  - Its state is just a list of references to other grains

[OrleansContrib/DesignPatterns](https://github.com/OrleansContrib/DesignPatterns)

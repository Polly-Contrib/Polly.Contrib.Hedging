# Polly.Contrib.Hedging

The Polly.Contrib.Hedging exposes a custom policy, i.e. Hedging.
The hedging policy allows the execution of multiple tasks with different degrees of concurrency, until one of them completes with success or until all of them are failed.

First task that is successfully completed triggers the cancellation and disposal of all other tasks and/or adjacent allocated resources.

# Usage

## Asynchronous execution

## Synchronous executions

# Further reading
- [Changelog](docs/CHANGELOG.md)
- [Code of conduct](docs/CODE_OF_CONDUCT.md)
- [How to contribute](docs/CONTRIBUTING.md)
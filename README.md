# Introduction 
The *Operations* was built to cancel orders using event driven architecture.

It is based on Azure Functions, with triggers for Azure Service Bus messages (published by the Scheduler component which is not part of this repo).


# Getting Started
**1.	Installation process**
  - Ensure the *Azure development* Workload is installed via the Visual Studio Installer. This will include the core SDKs and project templates for creating and running Azure Functions.
  - Optionally install v2.x of the [*Azure Functions Core Tools*](https://docs.microsoft.com/en-us/azure/azure-functions/functions-run-local#v2). This includes CLI commands and a local runtime for easier development, testing and management of Azure Functions on your local machine.
  - Ensure you have the SDK for .Net Core 3.1 installed. It can be downloaded from https://dotnet.microsoft.com/download

# Build
To build the solution, use the normal _Build -> Build Solution_ option. This should download all required Nuget packages. As long as the .Net Core SDK is installed, the solution should build successfully.

### Versioning
Builds are versioned using GitVersion, using the rules defined in the `GitVersion.yml` file in the solution. In short:
- Development builds (branches, pull requests, main builds) are suffixed with an approparite label, e.g. `-dev` or `-pr`. 
- Bug fix branches should be named `/bugfix<suffix>` 
- Release branches must be named `/release/Major.Minor.Patch`

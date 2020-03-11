# PC Polo Game Example!

This is example project of PC polo game. 

Technology used:
- .Net Core 3.1/ C# 8.0
- Communication via TCP protocol
- Usage of the async/await construct

Use [Mermaid](https://mermaidjs.github.io/) to display simple diagram :)

```mermaid
graph LR
A[Server] -- Server Push notifications --> B((Client))
A -- Server Push notifications --> C((Player2))
B -- TCP communication --> A{Server}
C -- TCP communication --> A
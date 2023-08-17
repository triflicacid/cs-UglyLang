# Execution

To start executing a program, run `Program.cs` and enter the filename. The file will be read and the sources parsed then executed. Parsing is carries out according to `Syntax.md`, and execution is done line-by-line starting from the top of the file.

## Importing

To include additional files in the program, use the `IMPORT` keyword. This will fetch, read, parse then execute the file. There are two sytaxes for the keyword:
- Without a symbol: any symbols defined in this file will appear in the same scope that the `IMPORT` keyword was invoked in.
- With a symbol: any symbols defined in thie file will be bundles into a `NAMESPACE` type which will be bound to the provided symbol.

Once a file is imported, it cannot be imported again until that file is exited.

## Scope

The language uses a stack to store variables. Each time a function is called a new context is pushed onto the stack.

When encountered, symbols will be looked up from the topmost stack context going down. If the symbol is a function, the function will be called with the provided arguments.

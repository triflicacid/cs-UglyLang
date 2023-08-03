# Scope

The language uses a stack to store variables. Each time a new file is entered or a function is called a new context is pushed onto the stack.

When encountered, symbols will be looked up from the topmost stack context going down. If the symbol is a function, the function will be called with the provided arguments.

Arguments are listed inside angular brackets `<...>` and are comma-seperated. If none are encountered then the function is called with no arguments.

Functions may be defined as stated above. The following functions listed in thie section are built-in.

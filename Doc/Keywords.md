
# Keywords
The following document will describe each keyword, split up into appropriate sections. The syntax of a line of source code is `<Keyword> <Before>: <After>`, with `Before` and `After` indicating what type of entity is expected in each position. Listed below are the possible values of the aforementioned fields:

- none: nothing is expected in this position.
- `expr`: an expression.
- `symbol`: a symbol is expected (no member access).
- `symbol_chain`: a symbol is expected. Member access is permitted, in which case the chain will reference a property.
- `type`: a type name is expected.

## General

| Keyword | Before | After | Description |
| - | - | - | - |
| `CAST` | `symbol_chain` | `type` | Change the type of the given symbol/property. |
| `DO` | none | `expr` | Evaluate the given expression. |
| `END` | none | none | Marks the end of the previous loop/function block. |
| `ERROR` | none | none | Raises an error with a default message. |
| `ERROR` | none | `expr` | Raises an error with the evaluated expression as the message. |
| `EXIT` | none | none | Exits the current loop structure. |
| `IMPORT` | none | `expr`| Imports the filepath provided and exports all symbols into the current scope. See `Runtime.md`. |
| `IMPORT` | `symbol` | `expr`| Same as the above, but bundles all symbols into a namespace with the name of `symbol`. |
| `INPUT` | `symbol_chain` | none | Prompts the user for input, storing the entered value in the given symbol/property. |
| `LET` | `symbol` | `expr` | Creates a new symbol and initialises it to the given value. |
| `PRINT` | none | `expr` | Prints the expression to the screen. Subsequent invocations of `PRINT[LN]` will print on the same line. |
| `PRINTLN` | none | `expr` | Prints the expression to the screen, followed by a line break. |
| `SET` | `symbol_chain` | `expr` | Sets the symbol/property to the new value. |
| `STOP` | none | none | Terminates program execution |

## Conditional Statement
| Keyword | Before | After | Description |
| - | - | - | - |
| `IF` | none | `expr` | Opens a new conditional block. The block is executed if the condition is met. |
| `ELSEIF` | none | `expr` | Adds a new branch to the current conditional block. The branch will execute if the condition is met and no previous branch has been executed. |
| `ELSE` | none | `expr` | Terminates a conditional block. This branch is executed if no other branch inside the block has been executed. |

## Loop Statement

| Keyword | Before | After | Description |
| - | - | - | - |
| `LOOP` | none* | none | Opens a loop block. The contents of said block are executed indefinitely. |
| `LOOP` | none* | `expr`| Opens a loop block. The contents of said block are executed for as long as the condition is met. |

\* - `Before` may be a `symbol`. If the symbol is defined, it must be numeric, else it is defined as an `INT`. This symbol acts as a loop counter. It is initially set to zero and is incremented at the end of each iteration.

## Procedures/Functions

| Keyword | Before | After | Description |
| - | - | - | - |
| `DEF` | `symbol` | *custom* | Defines a new function with the given name. See `Functions.md` for further details. |
| `FINISH` | none | none | Exits the current function, yielding the return value of `EMPTY`. |
| `FINISH` | none | `expr` | Exits the current function, yielding the return value of the result of `expr`. |


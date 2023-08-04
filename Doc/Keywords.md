# Keywords

## General

- `CAST <symbol>: <type>`
Change the type of the given variable.
- `DO: <expr>`
Execute the expression.
- `END`
Marks the end of a loop/function.
- `EXIT`
Exits the current loop.
- `FINISH` / `FINISH: <expr>`
Exits the current function and rteurns the stated value, or none.
- `INPUT <symbol>`
Prompts the user for input, setting the symbol to the given value.
- `LET <symbol>: <expr>`
Creates a new symbol.
- `PRINT: <expr>`
Prints the given value to the screen. Subsequent invokations of `PRINT` will print on the same line.
- `PRINTLN: <expr>`
Prints the given value to the screen, followed by a newline.
- `SET <symbol>: <expr>`
Sets the given symbol to the given value. Note that the type of the variable and the type of `expr` must match.
- `STOP`
Halt execution of the program

## Conditional Statement

- `IF: <expr>`
Start a new conditional block. Execute the block if the condition is met.
- `ELSEIF: <expr>`
Used after an IF keyword to introduce a new conditional. Block will be executed if the condition is true and other if/elseif conditions in the block before it have not been executed.
- `ELSE`
Used after an `IF`/`ELSEIF` keyword. Contents will be executed if none of the chained if/elseif blocks were executed.

## Loop Statement

- `LOOP`
Repeat the code block indefinitely.
- `LOOP: <expr>`
Loop the current block while the condition is truthy.
~~If the symbol is provided, this will be the loop counter. It will be defined in the outermost scope and must be numeric starting at 0 and incrementing each iteration.~~

## Procedures/Functions

- `DEF <symbol>: [<type>] <<arg1: type1>, <arg2: type2>, ...>`
Defines a new procedyre which accepts the stated arguments. If `type` is present, the function returns a value of that type, else a prcedyre is created which returns no value.

Functions may be overloaded, given that their return types (or lack thereof) match. Built-in functions **cannot** be overloaded.

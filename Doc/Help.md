# Syntax
Each line of the source file is considered to be a new statement. A line may either a comment if the line begins with `;`, or in the form:

`<KEYWORD> [arg][: <value> [(<type>)]]`

Depending on the keyword, neither, one or both of `arg` and `value` must be provided.
- `arg` : generally a single symbol, will be specified otherwise.
- `value` : generally an expression to be evaluated, will be specified otherwise.
- `type` : if present, the type to cast the expression to *before* it is passed into the keyword

There is the general pattern that `arg` acts as the input symbol/the symbol to affect, and `value` is the output value/value to use. For example, `INPUT x` places user input into `x` whilst `PRINT: x` outputs the value of `x`.

# Keywords

## General

- CAST <symbol>: <type>
Change the type of the given variable
- DO: <expr>
Execute the expression
- END
Marks the end of a loop/function
- EXIT / EXIT: <expr>
Exits the current loop/function. The second form is used to return a value from a function
- INPUT <symbol>
Prompts the user for input, setting the symbol to the given value.
- LET <symbol>: <expr>
Creates a new symbol
- PRINT: <expr>
Prints the given value to the screen
- SET <symbol>: <expr>
Sets the given symbol to the given value. Note that the type of the variable and the type of `expr` must match.

## Conditional Statement

- IF: <expr>
Start a new conditional block. Execute the block if the condition is met
- ELSEIF: <expr>
Used after an IF keyword to introduce a new conditional. Block will be executed if the condition is true and other if/elseif conditions in the block before it have not been executed.

## Conditional Loop

- WHILE [<symbol>]: <expr>
Loop the current block while the condition is met.
If the symbol is provided, this will be the loop counter. It will be defined in the outermost scope and must be numeric starting at 0 and incrementing each iteration.

## Infinite Loop

- LOOP
Repeat the code block indefintatly

## Functions

- DEF <symbol>: <type> <<arg1: type1>, <arg2: type2>, ...>
Defines a new function which returns <type> with the given arguments each with the resepective type.

# Symbols

The language uses a stack to store variables. Each time a new file is entered or a function is called a new context is pushed onto the stack.

When encountered, symbols will be looked up from the topmost stack context going down. If the symbol is a function, the function will be called with the provided arguments.

Arguments are listed inside angular brackets `<...>` and are comma-seperated. If none are encountered then the function is called with no arguments.

Functions may be defined as stated above. The following functions listed in thie section are built-in.

## Comparative Functions

- EQ<a,b>
Returns a number, 0 or 1, depending on whether the given objects are equal.
- GT<a,b>
Returns a number, 0 or 1, depending on if a > b
- GE<a,b>
Returns a number, 0 or 1, depending on if a >= b
- LT<a,b>
Returns a number, 0 or 1, depending on if a < b
- LE<a,b>
Returns a number, 0 or 1, depending on if a <= b

## Mathmatical Functions

- ADD<a,b>
Returns a + b
- SUB<a,b>
Returns a - b
- NEG<a>
Returns -a
- DIV<a,b>
Returns a / b
- MOD<a,b>
Returns a % b
- MUL<a,b>
Returns a * b
- EXP<a,b>
Returns a ^ b

## Generic Functions

- CAST<a,t>
Returns a casted to type t
Note that this is different to the CAST keyword
- RANDOM / RANDOM<max> / Random<min,max>
Returns random number in the range: [0,1) / [0,max) / [min,max)
- SLEEP<t>
Suspend execution for t milliseconds


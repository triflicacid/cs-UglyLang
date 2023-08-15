# Syntax
Each line of the source file is considered to be a new statement. A line is typically in the form:

`<KEYWORD> [arg][: <expr>]`

Depending on the keyword, neither, one or both of `arg` and `expr` must be provided.
- `arg` : generally a single symbol, will be specified otherwise.
- `expr` : generally an expression to be evaluated, will be specified otherwise.

There is the general pattern that `arg` acts as the input symbol/the symbol to affect, and `value` is the output value/value to use. For example, `INPUT x` places user input into `x` whilst `PRINT: x` outputs the value of `x`.

Single line comments begin with `;`. Block comments are started with `;:` and are closed with `:;`.

## Expressions

An expression may contain one or more units, and terminated with an optional type. Illustrated:

`<unit 1> <unit 2> ... <unit n> [<type>]`

Each unit is one of:
- A string literal, which is enclosed by quotation marks `" ... "`.
- A number, a string of digits optionally followed by a decimal point and another string of digits. If there is a decimal point, the type is `FLOAT`, else it is assumed to be `INTEGER`.
- A type name followed by arguments in brace `{ ... }` will attempts to construct said type with the given arguments.
- An at symbol followed by a type name is called a type literal and may be used to represent that type.
- A symbol name. The symbol may be followed by angled brackets `< ... >`. If present, these are passed to the symbol as arguments to a function. The arguments are seperated by commas. If the symbol is function, it is called with provided arguments, or called with none if no arguments are provided.

If a symbol name is followed by a `.`, a property name is expected. This may be a symbol or a numeric literal, and may be used as a function, in which case it will be called before the next property is accessed - e.g., `numbers.Reverse<>.Get<1>`. The topic of properties is covered in `Types.md`.

If there is a singular unit, this unit is evaluated and is the result of the expression. If there are multiple units, each unit will be evaluated, cast into a string, and concatenated together. Finally, if `type` is present, the entire result will be cast into the specified type.

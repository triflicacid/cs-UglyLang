# Built-In Functions

## Comparative Functions

Note: all these functions, except `EQ`, have the signature `<FLOAT,FLOAT> -> INT`.

- `EQ<a,b>` signature `<ANY,ANY> -> INT`
Returns a number, 0 or 1, depending on whether the given objects are equal.
- `GT<a,b>`
Returns a number, 0 or 1, depending on if a > b.
- `GE<a,b>`
Returns a number, 0 or 1, depending on if a >= b.
- `LT<a,b>`
Returns a number, 0 or 1, depending on if a < b.
- `LE<a,b>`
Returns a number, 0 or 1, depending on if a <= b.

## Logical Functions

Note: all these functions have the signature `<ANY,ANY> -> INTEGER`.

- `AND<a,b>`
Returns true if a and b are both true.
- `NOR<a>`
Returns the logical inverse of a.
- `OR<a,b>`
Returns true if a or b are true.
- `XOR<a,b>`
Returns true if a or b are true, but not both.

## Mathematical Functions

Note: all these functions have the signature `<FLOAT,FLOAT> -> FLOAT` or `<FLOAT> -> FLOAT` or `<INTEGER> -> INTEGER`.

- `ADD<a,b>`
Returns a + b.
- `DIV<a,b>`
Returns a / b.
- `EXP<a,b>`
Returns a ^ b.
- `MOD<a,b>`
Returns a % b.
- `MUL<a,b>`
Returns a * b.
- `NEG<a>`
Returns -a.
- `PRED<a>`
Returns the predecessor of integer a.
- `SUB<a,b>`
Returns a - b.
- `SUCC<a>`
Returns the successor of integer a.

## Generic Functions

- ~~`CAST<a,t>`
Returns a casted to type t.
Note that this is different to the CAST keyword.~~
- `CONCAT<a,b>` signature `<ANY,ANY> -> STRING`
Convert a and b to strings and concatenate them.
- `ID<a>` signature `<a> -> a`
Returns the provided argument.
- `RANDOM` / `RANDOM<max>` / `RANDOM<min,max>` signatures `(<> | <FLOAT> | <FLOAT,FLOAT>) -> FLOAT`
Returns random number in the range: [0,1) / [0,max) / [min,max).
- `SLEEP<t>` signature `<FLOAT> -> EMPTY`
Suspend execution for t milliseconds.
- `TYPE<a>` signature `<ANY> -> STRING`
Returns the type of a after evaluation as a string.


# User-Defined Functions

Functions may be defined with the following syntax:

```DEF <symbol>: [<type>] <<arg1: type1>, <arg2: type2>, ...>```

This will create a symbol `symbol` in the topmost scope and bind it to the defined function.

If the return type is present, the function **must** return an instance of said type using the `FINISH` keyword. The type may be omitted, in which case the function returns an instance of `EMPTY`.

Functions are called by referencing their name. If the function accepts arguments they must be present in the defined order inside angular brackets, otherwise they are called with no arguments.

# Built-In Functions

## Comparative Functions

- `EQ<a: ANY, b: ANY> -> INT`
Returns a number, 0 or 1, depending on whether the given objects are equal.
- `GT<a: FLOAT, b: FLOAT> -> INT`
Returns a number, 0 or 1, depending on if a > b.
- `GE<a: FLOAT, b: FLOAT> -> INT`
Returns a number, 0 or 1, depending on if a >= b.
- `LT<a: FLOAT, b: FLOAT> -> INT`
Returns a number, 0 or 1, depending on if a < b.
- `LE<a: FLOAT, b: FLOAT> -> INT`
Returns a number, 0 or 1, depending on if a <= b.

## Generic Functions

- ~~`CAST<a,t>`
Returns a casted to type t.
Note that this is different to the CAST keyword.~~
- `CONCAT<a: ANY, b: ANY> -> STRING`
Convert a and b to strings and concatenate them.
- `ID<x: a> -> a`
Returns the provided argument.
- `LIST<a: TYPE> -> ANY[]`
Creates a new list instance containing type a.
- `NEW<a: TYPE> -> ANY`
Create a new instance of type a and return it. Note, that a must be able to be created with no arguments.
- `RANDOM -> FLOAT` / `RANDOM<max: FLOAT> -> FLOAT` / `RANDOM<min: FLOAT, max: FLOAT> -> FLOAT`
Returns random number in the range: [0,1) / [0,max) / [min,max).
- `SLEEP<t: FLOAT> -> EMPTY`
Suspend execution for t milliseconds.
- `TYPE<a: ANY> -> TYPE`
Returns the type of a after evaluation.

## List Functions

These functions are all available on lists. The list type is assumed to be `a[]`.

- `Add<e: a> -> EMPTY / Add<e: a, idx: INT> -> EMPTY`
Either adds an item to the end of the list, or inserts an item at the given index.
- `Contains<e: a> -> INT`
Returns whether or not the list contains said item.
- `Get<idx: INT> -> a`
Returns item at the given index.
- `IndexOf<e: a> -> INT`
Returns the index of the given item in the list, or returns -1.
- `Join<glue: STRING> -> STRING`
Joins each item in the list by glue.
- `Length -> INT`
Returns the length of the list. Note that this is a function, not a property.
- `Remove<e: a> -> INT`
Removes *every* instance of the given item from the list. Returns how many items were removed.
- `RemoveAt<idx: INT> -> INT`
Removes the item at the given index, returns is this was done (i.e. if the index was in-bounds).
- `Reverse -> a[]`
Reverses the list.
- `Set<idx: INT, v: a> -> EMPTY`
Sets the item at the given index to the given value.
- `Slice<start: INT> -> a[] / Slice<start: INT, end: INT> -> a[]`
Returns a slice of the list, starting from `start` andending either at the end of the list or at `end`.

## Logical Functions

- `AND<a: ANY, b: ANY> -> INT`
Returns true if a and b are both true.
- `NOT<a: ANY> -> INT`
Returns the logical inverse of a.
- `OR<a: ANY, b: ANY> -> INT`
Returns true if a or b are true.
- `XOR<a: ANY, b: ANY> -> INT`
Returns true if a or b are true, but not both.

## Mathematical Functions

- `ADD<a: FLOAT, b: FLOAT> -> FLOAT`
Returns a + b.
- `DIV<a: FLOAT, b: FLOAT> -> FLOAT`
Returns a / b.
- `EXP<a: FLOAT, b: FLOAT> -> FLOAT`
Returns a ^ b.
- `MOD<a: FLOAT, b: FLOAT> -> FLOAT`
Returns a % b.
- `MUL<a: FLOAT, b: FLOAT> -> FLOAT`
Returns a * b.
- `NEG<a: FLOAT> -> FLOAT`
Returns -a.
- `PRED<a: INT> -> INT`
Returns the predecessor of integer a.
- `SUB<a: FLOAT, b: FLOAT> -> FLOAT`
Returns a - b.
- `SUCC<a: INT> -> INT`
Returns the successor of integer a.

## String Functions

- `Length -> INT`
Returns the length of the string.
- `Lower -> STRING`
Returns the string in lowercase.
- `Reverse -> STRING`
Reverses the string
- `Slice<start: INT> -> STRING / Slice<start: INT, end: INT> -> STRING`
Returns a sliced potrion of the starting starting from `start` and ending at either the end of the string or at `end`.
- `Split<sep: STRING> -> STRING[]`
Splits the string by the given seperator. If the seperator if `""`, split by each character.
- `Title -> STRING`
Returns the string in title case.
- `Upper -> STRING`
Returns the string in uppercase.

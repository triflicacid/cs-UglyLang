# User-Defined Functions

Functions may be defined with the following syntax:

```DEF <symbol>: [<type>] <<arg1: type1>, <arg2: type2>, ...>```

This will create a symbol `symbol` in the topmost scope and bind it to the defined function.
- If a return type is given, the function must return an instance of this type (using the `FINISH` keyword). If there is no return type, the function returns `EMPTY`.
- If the function takes no arguments, `< ... >` may be omitted.
- If the function takes no arguments and returns no value, the colon and everything after it may be omitted.

Functions are called by referencing their name. If the function accepts arguments they must be present in the defined order inside angular brackets, otherwise they are called with no arguments.

# Built-In Functions

## Bitwise Functions

- `BITAND<a: INT, b: INT> -> INT`
Calculate the bitwise and of a and b.
- `BITNOT<a: INT> -> INT`
Calculate the bitwise inverse of a.
- `BITOR<a: INT, b: INT> -> INT`
Calculate the bitwise or of a and b.
- `SHL<a: INT> -> INT`
Shift a left by one places.
- `SHL<a: INT, b: INT> -> INT`
Shift a left by b places.
- `SHR<a: INT> -> INT`
Shift a right by one places.
- `SHR<a: INT, b: INT> -> INT`
Shift a right by b places.
- `BITXOR<a: INT, b: INT> -> INT`
Calculate the bitwise exclusive-or of a and b.

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

- `CONCAT<a: ANY, b: ANY> -> STRING`
Convert a and b to strings and concatenate them.
- `CreateVirtualFile<name: STRING, contents: STRING>`
Creates a virtual file at runtime. Say that one creates a virtual file `eg.txt`. Then `IMPORT: "eg.txt"` will still fail as the keyword operated at compile-time, but `LET x: IMPORT<"eg.txt">` will work as the function operates at runtime.
- `ID<x: a> -> a`
Returns the provided argument.
- `IF<p: ANY, t: a, f: a> -> a`
Returns t of f depending on whether p is tru of false, respectively.
- `IMPORT<path: STRING> -> NAMESPACE`
Imports the given file and returns the bundled namespace. Behaviour of `LET name: IMPORT<path>` is similar to `IMPORT name: path`.
- `IMPORT<path: STRING, reEval: INT> -> NAMESPACE`
Imports the given file and returns the bundled namespace. If reEval is truthy, forces re-evaluation even if the namespace is cached.
- `RANDOM -> FLOAT` / `RANDOM<max: FLOAT> -> FLOAT` / `RANDOM<min: FLOAT, max: FLOAT> -> FLOAT`
Returns random number in the range: [0,1) / [0,max) / [min,max).
- `SLEEP<t: FLOAT>`
Suspend execution for t milliseconds.

## List Functions

These functions are all available on lists. The list type is assumed to be `a[]`.

- `Add<e: a> / Add<e: a, idx: INT>`
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
- `Set<idx: INT, v: a>`
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

## Map Functions
These functions are all available on maps. The map type is assumed to be `MAP[a]`.

- `Delete<key: STRING> -> a`
Deletes the key and its value from the map, or raises an error if the key does not exist. Returns the old value associated with the key.
- `Get<key: STRING> -> a`
Returns the value associated with the given key, or raises an error.
- `Has<key: STRING> -> INT`
Returns whether or not the map contains the given key.
- `Keys -> STRING[]`
Returns a list of all the keys.
- `Size -> INT`
Returns the number of items registered in the map.
- `Set<key: STRING, value: a>`
Sets the key to the given value. Creates the key if does not exist in the map.

## Mathematical Functions

- `ABS<a: FLOAT> -> FLOAT`
Returns the absolute value of a (removes negative sign).
- `ADD<a: FLOAT, b: FLOAT> -> FLOAT`
Returns a + b.
- `CEIL<a: FLOAT> -> INT`
Returns the ceiling of a.
- `DIV<a: FLOAT, b: FLOAT> -> FLOAT`
Returns a / b.
- `EXP<a: FLOAT, b: FLOAT> -> FLOAT`
Returns a ^ b.
- `FLOOR<a: FLOAT> -> INT`
Returns the floor of a.
- `MOD<a: FLOAT, b: FLOAT> -> FLOAT`
Returns a % b.
- `MUL<a: FLOAT, b: FLOAT> -> FLOAT`
Returns a * b.
- `NEG<a: FLOAT> -> FLOAT`
Returns -a.
- `PRED<a: INT> -> INT`
Returns the predecessor of integer a.
- `ROUND<a: FLOAT> -> INT`
Rounds the given decimal number
- `SUB<a: FLOAT, b: FLOAT> -> FLOAT`
Returns a - b.
- `SUCC<a: INT> -> INT`
Returns the successor of integer a.
- `SQRT<a: FLOAT> -> FLOAT`
Calculates the square root of a.

## String Functions
These functions are properties of any `STRING` instance.

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
 
## Type Functions

- `IS<x: ANY, t: TYPE> -> INT`
Returns whether the value x is an instance of the given type t.
- `TYPE -> TYPE`
Returns the type type.
- `TYPE<a: ANY> -> TYPE`
Returns the type of a after evaluation.

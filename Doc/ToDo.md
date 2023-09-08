# Bugs

- Duplicate overload definitions with same signature permitted.
- `Examples/Rule110.txt`: Function `LIST` no longer exists. Add way to create lists of an initial size.

# Ideas

The following section contains ideas for the language. The bullets are not listed in any particular order.

- ~~Add user-defined types.~~
	- Add inheritance
	- Type constraint arguments

- Allow multi-word keywords (key*phrases*)
	- Add keyphrase `LOOP OVER <symbol>: <symbol>` where the first symbol is iterated over, each member being set to the second symbol.
	- Namespaces and Types: read-only modifier.

- Differentiate between read-only and constants. Say that a symbol contains a vector. Then
	- `LET`: both the symbol and the vector's fields can be changed.
	- `READ-ONLY`: the symbol cannot be changed, but the fields of the vector can.
	- `CONSTANT`: both the symbol nor the vector's fields cannot be changed.

- Make errors an internal type. Create synonyms to the `try .. catch` blocks.

- Create a basic text editor for this language with syntax highlighting. Either in a new repository or in this repository. If it is the latter, propose placing them in `Language` and `Editor` directories.

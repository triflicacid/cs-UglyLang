# Bugs

- Duplicate overload definitions with same signature permitted.

# Ideas

The following section contains ideas for the language. The bullets are not listed in any particular order.

- Record a symbol's definition location

- ~~Add user-defined types.~~
	- Add inheritance
	- Type constraint arguments

- Allow multi-word keywords (key*phrases*)
	- Add keyphrase `LOOP OVER <symbol>: <symbol>` where the first symbol is iterated over, each member being set to the second symbol.
	- Namespaces and Types: read-only modifier.

- Make errors an internal type. Create synonyms to the `try .. catch` blocks.

- Create a basic text editor for this language with syntax highlighting. Either in a new repository or in this repository. If it is the latter, propose placing them in `Language` and `Editor` directories.

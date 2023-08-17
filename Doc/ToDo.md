# Bugs

- Duplicate overload definitions with same signature permitted.

# Ideas

The following section contains ideas for the language. The bullets are not listed in any particular order.

- Record a value's definition location

- Add user-defined types. Example syntax:

```
TYPE Person [WHERE ..]
	FIELD name: STRING
	FIELD age: INT

	DEF Say: STRING <msg: STRING>
		FINISH: name " says '" msg "'."
	END
END

LET joe: Person { "Joe", 34 }
PRINTLN: joe.Say<"Hello, world"> ; => Joe says 'Hello, world'.
PRINTLN: joe.age ; => 34
SET joe.age: SUCC<joe.age> ; joe.age = 35
```

The arguments of the constructor would correspond to each `FIELD` field, in order of definition.

- Type constraints: add option to allow an instance of a type. Either make this the behaviour by default (equl to or instance of), or add a new symbol such as `'`.

- Allow multi-word keywords (key*phrases*)
	- Add keyphrase `LOOP OVER <symbol>: <symbol>` where the first symbol is iterated over, each member being set to the second symbol.

- Create a basic text editor for this language with syntax highlighting. Either in a new repository or in this repository. If it is the latter, propose placing them in `Language` and `Editor` directories.

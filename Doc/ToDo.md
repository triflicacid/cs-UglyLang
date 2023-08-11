# Bugs

- Duplicate overload definitions with same signature permitted.

# Ideas

The following section contains ideas for the language.

- Record a value's definition location

- Add user-defined types. Example syntax:

```
TYPE Person
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

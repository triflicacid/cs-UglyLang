# Bugs

- Duplicate overload definitions with same signature permitted.

# Ideas

The following section contains ideas for the language.

- Record a value's definition location

- More flexible property access. At the moment, names are parsed which include periods which are then recognised as properties. Propse adding a MemberAccessNode which has a parent Symbol and a child Symbol.

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

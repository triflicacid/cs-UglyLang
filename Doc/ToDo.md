# Bugs

# Ideas

The following section contains ideas for the language.

- Record a value's definition location

- Lists: dynamic get/set such that the properties can be calculated at runtime. Propose creating methods so that the `.<index>` way is kept as constant indexes only.

- Add list constructors which omit the type, assuming that at least one member is provided. `INT[] {1,2} --> {1,2}`. This can be done as the types of the members can be inferred.

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

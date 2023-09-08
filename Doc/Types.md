Everything in this language has a type, even types themselves.

# Built-In Types

Every value in this language has a type. The type of a value may be obtained using the `TYPE` function.

**Special Types**

- `ANY`. This is a special type that matches any value. It is only used internally, and cannot be used nor constructed by the user.
- `EMPTY`. This is a special type that represents an absence of a value. It is returned by functions with no return type, but cannot be constructed nor refernced by the user.

	Note that the `EMPTY` values does not necessarily have the `EMPTY` type. `EMPTY` values may have another type and simply portrays a placeholder for any other value of that type.

**Primitive Types**
- `INT`. Represents an integer in the range -9,223,372,036,854,775,808 to 9,223,372,036,854,775,807 (same as `long` in C#).

	Note that integers are used to represent Booleans, with 0 representing False and any other number representing True.

	**Constructors**:
	- `<>` - Returns a new int with value 0.
	- `<v: ANY>` - Casts the provided value to `INT`.

- `FLOAT`. Represents a floating point number in the range +-1.5e-45 to +-3.4e38 (same as `float` in C#).

	**Constructors**:
	- `<>` - Returns a new float with value 0.
	- `<v: ANY>` - Casts the provided value to `FLOAT`.

- `STRING`. Represents a sequence of characters.

	**Constructors**:
	- `<>` - Returns a new string of length 0.
	- `<v: ANY>` - Casts the provided value to `STRING`.

- `TYPE`. Represents a type itself. The value of such a variable may be `INT`, `FLOAT`, or even `TYPE` itself.

**Compound Types**
- `a[]`. Represents a list of type `a`.
	- Property `.MemberType: TYPE` contains the type of the members (`a`).
	- A list of any type may be referenced using `@LIST`.
	
	**Constructors**:
	- `<>` - Returns a new list of length 0.
	- `<len: INT>` - Returns a new list of length `len` populated with empty instances.

- `MAP[a]`. Represents a map between `STRING` keys and values of type `a`.
	- Property `.ValueType: TYPE` contains the type of the values (`a`).
	- A map of any type may be referenced using `@MAP`.
	
	**Constructors**:
	- `<>` - Returns a new map with no entries.

- `NAMESPACE`. Represents a collection of symbols. Primarily used when importing a file.
	- It **cannot** be constructed.
	- It contains symbols which can be accessed as properties.

# User-Defined Types

Custom types may be created using the `TYPE` keyword. A type consists of fields, introduced using the `FIELD` keyword, and methods, defined using the `DEF` keyword. Constructor(s) are defined using the `NEW` keyword like functions but with no return type For example,

```
TYPE Vec
	FIELD x: INT
	FIELD y: INT

	NEW
		SET x: 0
		SET y: 0
	END

	NEW <ax: INT, ay: INT>
		SET x: ax
		SET y: ay
	END

	DEF Repr: STRING <>
		FINISH: "<" x "," y ">"
	END

	LET i: Vec<1, 0>
	LET j: Vec<0, 1>
END
```

This defines a new type, `Vec`, which contains two fields and one method. An instance of this type may be constructed by calling the type like a function, which is matched against one of the defined constructors. For example, the result of calling `Vec:Repr` on the following values:
- `Vec<>` -> `<0,0>`
- `Vec<1, 2>` -> `<1,2>`

When methods are called, the containing type instance resides in the stack. As such, all fields are accessible seemingly as normal variables and may be updated as such.

`LET` and `CONST` statements can be used to create static properties. Such properties exist on the type itself, so `Vec.i`, not one any one instance.

# Constructing Types

Most types have a constructor. These can be invoked by "calling" the type e.g. `@STRING<>`.

Note that, as long as the value is resolved to a type, it may be called, so `SET t: @STRING \n ... t<>` is also valid.

## Baked Constructors

Common types have a baked-in construction syntax.

- `INT`. Numeric literals lacking a decimal point, or where the decimal portion is zero, construct an `INT` instance. For example, `13` or `-1.0`.
- `FLOAT`. Numeric literals with a non-zero decimal portion construct a `FLOAT` instance. For example, `3.14` or `-2.7`.
- `LIST`. The type of a list is assumed when the `type` is omitted in type constructors, with the type of the list equal to the type of the first member (`{ a: t, b: t, ... } => t[]`). All member types must be equal (no simply matching). For example, `{ 0, 1 }` is assumed `INT[]` but ` { 0, 1.1 }` is an error as `INT != FLOAT`. Note that this may be nested, e.g., `{{1},{1,1}}` is `INT[][]`.
- `STRING`. Sequences of characters enclosed in quotation marks construct a `STRING` instance. For example `"Hello, world"` or `""`. Note that escape characters are **not** supported.
- `TYPE`. Types may be constructed using braces, but to reference a type itself one uses `@` followed by the type name. For example, `@INT`. Note, that type literals **cannot** be parameterised, but it can reference parameters already bound in the context.

# Type Parameters
Symbols may be used to represent types. For example,

```
DEF Id: a <arg: a>
	FINISH: x
END
```

Defines a function which returns the value given to it. Its argument is defined as `a`, a type parameter acting as a placeholder for the type of the argument. It returns `a`, meaning that the argument and the return value are the same type.

Type parameters are treated the same as any type and thus may be used in compound types. For example,

```
DEF First: a <list: a[]>
	FINISH: list.0
END
```

This returns the first item in any list. The parameterised type `a[]` is matched against the argument type and the type of `a` is resolved. Therefore, the return type of the function must be the same as the member type of the input list.

Once inside the scope of a function, `a` may be used as:
- A variable with type `TYPE` with the value of the type of the members of the argument list.
- A type. No longer parameterised, however, as it references the type of the members of the argument list.

## Type Constraints
Basic constraints may be added to type parameters which limit which types the type parameter may match. For example,

```
DEF Fn: a <n1: a, n2: a> WHERE a: INT|FLOAT
	FINISH: ADD<MUL<a,2>,b>
END
```

At the moment, this must be direct equality between types, not simply matching.

# Properties
Properties may exist on types, as well as on individual values which support it.

Properties may be retrieved using the `.` syntax. It is important to note:

- Some properties may be read-only, which prohibits updating a property's value.
- Most types are rigid in their property types, meaning that changing the type of a defined property is not permitted.

When functions are accessed using the `.` syntax, the function is wrapped in a function context. When a function context is called, the parent is passed in as the first argument. That is why the `LIST.Get` function has a signatue of `<a[],INT>` despite the fact that, visually, only one argument of type `INT` is passed.

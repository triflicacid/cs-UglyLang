Everything in this language has a type, even types themselves.

# Built-In Types

Every value in this language has a type. The type of a value may be obtained using the `TYPE` function.

**Special Types**

- `ANY`. This is a special type that matches any value. It is only used internally, and cannot be used nor constructed by the user.
- `EMPTY`. This is a special type that represents an absence of a value. It is returned by functions with no return type, but cannot be constructed nor refernced by the user.

**Primitive Types**
- `INT`. Represents an integer in the range -9,223,372,036,854,775,808 to 9,223,372,036,854,775,807 (same as `long` in C#).

Note that integers are used to represent Booleans, with 0 representing False and any other number representing True.
- `FLOAT`. Represents a floating point number in the range +-1.5e-45 to +-3.4e38 (same as `float` in C#).
- `STRING`. Represents a sequence of characters.
- `TYPE`. Represents a type itself. The value of such a variable may be `INT`, `FLOAT`, or even `TYPE` itself.

**Compound Types**
- `a[]`. Represents a list of type `a`.

# User-Defined Types

*In progress*

# Constructing Types

Each type is constructed differently, each requiring different arguments or none at all. Types which require no arguments may be constructed using the `NEW<t: TYPE>` function.

Every type may be constructed using the following syntax:

``` <type> { <arg1>, <arg2>, ... } ```

## Baked Constructors

Common types have a baked-in construction syntax.

- `INT`. Numeric literals lacking a decimal point, or where the decimal portion is zero, construct an `INT` instance. For example, `13` or `-1.0`.
- `FLOAT`. Numeric literals with a non-zero decimal portion construct a `FLOAT` instance. For example, `3.14` or `-2.7`.
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

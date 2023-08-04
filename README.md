# Ugly Lang

This is an interpreted, simplistic keyword-driven language written in C#. As for the name, the origins came from quick sketches of a simple yet useable language with a not-so-pleasing syntax (namely the use of angled brackets).

The syntax is extremely simple, with the program being split into lines and each line carrying out a single function as dictated by the leading keyword. Operators do not exist with expressions being constituted of literals and function calls. More information can be found in `Doc/Syntax.md`.

I have previously written an expression-driven language called TriflicScript wherein the source is parsed as one expression and converted to RPN before being executed on a stack. In contrast, this language has a strict structure with each line representing a statement and expressions being data passed into these statements which is parsed into an abstract syntax tree before being executed in a waterfall pattern.

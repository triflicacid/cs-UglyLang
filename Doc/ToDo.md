# Bugs
- `Value.To` - program crashes if there is a casting error, noticeably with `Convert.ToDouble`

# Ideas

The following document contains ideas for the language

- Concatenation: currently expressions contains only one top-level items e.g. `name`. If multiple items should appear, they are concatenated.

E.g, `"Hello " "world"` ==> `Hello world`

- Add command to print on the same line. Either introduce a new print command, or rename `PRINT` to `PRINTLN`.
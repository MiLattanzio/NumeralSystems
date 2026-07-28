# Contributing to NumeralSystems.Net

Thank you for improving NumeralSystems.Net. Contributions can include bug fixes,
tests, documentation, performance work, and focused feature proposals.

By participating, you agree to follow the
[Code of Conduct](CODE_OF_CONDUCT.md). Do not disclose vulnerabilities in a
public issue; use the process in [SECURITY.md](SECURITY.md).

## Before opening an issue

Search the [existing issues](https://github.com/MiLattanzio/NumeralSystems/issues)
and read the [documentation](NumeralSystems.Net/docs/index.md).

For a bug, include:

- the NumeralSystems.Net version or commit;
- your .NET SDK/runtime and operating system;
- the smallest code sample that reproduces the behavior;
- the expected result and the actual result;
- the full exception and stack trace, when applicable.

For a feature, describe the use case before proposing an API. Explain why the
existing types cannot solve it and identify compatibility or performance
constraints.

## Development setup

Install the .NET 8 SDK, then clone and restore the solution:

```bash
git clone https://github.com/MiLattanzio/NumeralSystems.git
cd NumeralSystems/NumeralSystems.Net
dotnet restore
```

Build and run the tests:

```bash
dotnet build --configuration Release --no-restore
dotnet test --configuration Release --no-build
```

The library targets .NET Standard 2.1. The test project targets .NET 8 and uses
NUnit.

## Making a change

1. Create a short-lived branch from `master`.
2. Keep the change focused on one problem.
3. Add or update NUnit tests for observable behavior.
4. Update the relevant guide and XML API comments when public behavior changes.
5. Run the build and tests, then review the rendered Markdown on GitHub.
6. Review the complete diff before committing.

Follow the existing C# style:

- four spaces for indentation;
- braces on their own lines;
- descriptive names over abbreviations;
- explicit aliases when a library type conflicts with a `System` type;
- no unrelated formatting in a focused pull request.

Public APIs should validate arguments consistently and include valid XML
documentation for public types, members, parameters, return values, and thrown
exceptions.

## Tests

Place tests in `NumeralSystems.Net/NumeralSystem.Net.NUnit` next to the closest
existing fixture. A useful test is deterministic and covers:

- the successful path;
- invalid input or an impossible reverse operation;
- boundary values for the primitive width or numeral base;
- round trips when adding a conversion or encoding.

Avoid random-only assertions. If randomness is required, use a fixed seed and
include the failing input in the assertion message.

## Documentation

Documentation is in `NumeralSystems.Net/docs` and consists entirely of Markdown
files. Update `docs/index.md` and the navigation block in affected pages when
adding, renaming, or removing a guide. Keep `docs/api-reference.md` aligned with
the public surface of the C# project.

Preview the changed files in a Markdown renderer and check headings, tables,
code blocks, and relative links.

When editing examples:

- include all required `using` directives;
- avoid culture-dependent expected strings unless culture is the subject;
- say when binary arrays are least-significant-bit first;
- keep the base, alphabet, separators, and encoded width explicit.

## Pull requests

Open a pull request against `master` and complete the repository template. A
reviewer should be able to determine:

- what changed and why;
- which public behavior is affected;
- how the change was tested;
- whether compatibility or documentation changes;
- which issue is fixed, when applicable.

Keep generated build output, IDE files, credentials, and unrelated changes out
of the commit. Maintainers may request changes before merging.

## License

By submitting a contribution, you confirm that you have the right to provide it
under the repository's [MIT License](LICENSE.txt).

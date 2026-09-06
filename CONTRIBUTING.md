# Contributing to X.PagedList

Thanks for taking the time to contribute. X.PagedList is maintained by one
person in his spare time, so clear, focused contributions are the ones that
get merged fastest.

## Before you start

- **Bugs and features** — open an issue first for anything beyond a small fix.
  Describe the package (`X.PagedList`, `X.PagedList.Mvc.Core`, `X.PagedList.EF`,
  `X.PagedList.EntityFramework`, `X.PagedList.Serialization.*`), the package
  version, the target framework and a minimal reproduction.
- **Security issues** — do not open a public issue. Follow [SECURITY.md](SECURITY.md).
- **Questions about usage** — check the [Wiki](https://github.com/udndc/X.PagedList/wiki) first.

## Building and testing

Prerequisites: .NET SDK 10.0 or later (the solution uses the `.slnx` format)
and the .NET 8 runtime for the test project.

```bash
dotnet restore X.PagedList.slnx
dotnet build X.PagedList.slnx -c Release
dotnet test tests/X.PagedList.Tests/X.PagedList.Tests.csproj -c Release
```

If you only have the .NET 10 runtime installed, run the tests with
`DOTNET_ROLL_FORWARD=Major dotnet test ...`.

The same steps run in CI (`.github/workflows/ci.yml`) on every pull request.

## Pull requests

- Branch from `master` and keep the pull request focused on one change.
- Add or update tests in `tests/X.PagedList.Tests` for any behaviour change.
- Do not change the public API in a patch or minor release. Breaking changes
  go into the next major version; open an issue to discuss them first.
- Do not bump the version. The package version is set from the release tag
  (`v10.6.0` → `10.6.0`) by `.github/workflows/release.yml`; the default in
  `src/Directory.Build.props` is updated by the maintainer.
- Add a line to `CHANGELOG.md` under the unreleased version.
- Follow the style of the surrounding code: 4-space indentation, nullable
  reference types enabled, XML documentation on public members.

## Licensing of contributions

X.PagedList is licensed under the [MIT License](LICENSE.md). By submitting a
contribution (code, documentation or anything else) to this repository you
agree that it is provided under the same MIT License, without any additional
terms or conditions. This is the same "inbound = outbound" rule described in
[GitHub's Terms of Service, section D.6](https://docs.github.com/en/site-policy/github-terms/github-terms-of-service#6-contributions-under-repository-license).

You confirm that you have the right to grant this license for your contribution,
for example that it is your own work or that your employer permits you to
contribute it.

## Releases

Releases are made by the maintainer by pushing a `v*` tag; see
`.github/workflows/release.yml`. Contributors do not need to do anything for
a release beyond getting their pull request merged.

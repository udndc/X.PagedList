# Changelog

All notable changes to this project are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning 2.0](https://semver.org/spec/v2.0.0.html).

## [10.6.0] - Unreleased

Metadata and packaging release. No changes to the public API.

### Added

- `SECURITY.md` with the supported versions and the private vulnerability
  reporting process (GitHub Security Advisories).
- `SPONSORS.md` and the Tidelift entry in `.github/FUNDING.yml`.
- `CONTRIBUTING.md` with build/test instructions, pull request rules and the
  MIT inbound = outbound rule for contributions.
- SourceLink (`Microsoft.SourceLink.GitHub`), deterministic builds and symbol
  packages (`.snupkg`) for all packages.
- GitHub Actions: CI (build and test on push and pull request) and release
  (pack and publish to NuGet.org on a `v*` tag, version taken from the tag,
  NuGet Trusted Publishing via OIDC instead of a stored API key).
- `net10.0` target for `X.PagedList`, `X.PagedList.Mvc.Core`, `X.PagedList.EF`
  (with EF Core 10), `X.PagedList.Serialization.SystemTextJson` and
  `X.PagedList.Serialization.JsonNet`. Existing targets are unchanged.
- Issue forms (YAML) for bug reports and feature requests with package,
  version, target framework and minimal reproduction fields.

### Changed

- All repository links point to `https://github.com/udndc/X.PagedList`
  (the repository moved from the `dncuug` organization).
- `PackageLicenseExpression` is now `MIT` instead of a packaged license file.
- Package READMEs refreshed: sponsorship badges, "Support this project"
  section, package titles aligned with their NuGet package ids
  (`X.PagedList.Mvc.Core`, `X.PagedList.EF`, `X.PagedList.EntityFramework`).
- The Serialization packages ship the repository `LICENSE.md` instead of their
  own copies; Oleksandr Tsvirkun is listed in `Authors` of both packages.
- The test project runs on `net8.0` and `net10.0`; the example web site and
  its data layer target `net10.0` with EF Core 10.
- `GeneratePackageOnBuild` disabled; packages are produced by `dotnet pack`
  in the release workflow only.

### Removed

- `.github/stale.yml` (configuration for the discontinued probot-stale app).
- Markdown issue templates, replaced by issue forms.

[10.6.0]: https://github.com/udndc/X.PagedList/compare/v10.5.9...v10.6.0

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
- SourceLink (`Microsoft.SourceLink.GitHub`), deterministic builds and symbol
  packages (`.snupkg`) for all packages.
- GitHub Actions: CI (build and test on push and pull request) and release
  (pack and publish to NuGet.org on a `v*` tag, version taken from the tag).

### Changed

- All repository links point to `https://github.com/udndc/X.PagedList`
  (the repository moved from the `dncuug` organization).
- `PackageLicenseExpression` is now `MIT` instead of a packaged license file.
- Package READMEs refreshed: sponsorship badges, "Support this project"
  section, package titles aligned with their NuGet package ids
  (`X.PagedList.Mvc.Core`, `X.PagedList.EF`, `X.PagedList.EntityFramework`).

[10.6.0]: https://github.com/udndc/X.PagedList/compare/v10.5.9...v10.6.0

# Releasing and publishing the NuGet package

[Documentation home](index.md) ·
[Getting started](getting-started.md) ·
[Troubleshooting](troubleshooting.md) ·
[API reference](api-reference.md)

The `.github/workflows/dotnet.yml` workflow builds and tests every push and pull
request targeting `master` or `develop`. It also packages and publishes
`NumeralSystems.Net` when a GitHub Release is published.

## One-time repository setup

NuGet publication uses Trusted Publishing and GitHub OpenID Connect (OIDC).
There is no long-lived API key to create, store, or rotate.

1. Sign in to NuGet.org with the account or organization that owns
   `NumeralSystems.Net`.
2. Open **Trusted Publishing** and create a GitHub Actions policy with these
   values:

   | Field | Value |
   | --- | --- |
   | Repository owner | `MiLattanzio` |
   | Repository | `NumeralSystems` |
   | Workflow file | `dotnet.yml` |
   | Environment | `nuget-release` |

3. In the GitHub repository, create an Actions variable named `NUGET_USER`.
   Set it to the NuGet.org profile name used by the policy, not an email
   address.
4. In **Settings > Environments**, review the automatically referenced
   `nuget-release` environment and add any desired deployment protection rules.

The publish job requests `id-token: write` only for the OIDC exchange.
`NuGet/login@v1` obtains a short-lived credential immediately before the push;
no GitHub Actions secret is required.

## Tag format

Use one of these forms:

```text
vMAJOR.MINOR.PATCH
vMAJOR.MINOR.PATCH-prerelease
```

Examples:

```text
v4.7.0
v4.7.0-beta.1
```

The leading `v` is removed before setting `PackageVersion`. A prerelease GitHub
Release should also use a prerelease suffix in its tag; the GitHub prerelease
checkbox alone does not change the NuGet version.

Tags with a fourth numeric component, build metadata, spaces, or other formats
are rejected by the workflow.

## Publish a release

1. Merge the intended changes into `master`.
2. Confirm the `Build and test` job succeeds on `master`.
3. Update release notes and choose the next semantic version.
4. Create a GitHub Release from the `master` commit with a valid tag.
5. Publish the release.
6. Wait for both workflow jobs to finish.
7. Verify the new version on NuGet.org and install it in a clean sample project.

Publishing a draft release does not trigger deployment. The workflow starts on
the `release.published` event and checks out the exact release tag.

## Workflow output

For a release, the workflow:

1. builds the solution in `Release`;
2. runs the complete NUnit suite;
3. validates and extracts the package version from the tag;
4. creates `NumeralSystems.Net.<version>.nupkg`;
5. uploads the package as a GitHub Actions artifact for 30 days;
6. authenticates to NuGet.org through OIDC Trusted Publishing;
7. pushes it to `https://api.nuget.org/v3/index.json`.

`--skip-duplicate` makes a repeated run harmless when the same package version
already exists. NuGet packages are immutable, so code changes require a new
version rather than overwriting a published package.

## Recover from a failed publication

- **Invalid tag:** create a new release with a valid semantic-version tag.
- **Trusted Publishing rejected:** verify the NuGet.org policy fields, the
  `NUGET_USER` repository variable, and the `nuget-release` environment name,
  then rerun the failed job.
- **Version already exists:** choose a new version. Do not attempt to replace
  the existing package.
- **Build or test failure:** fix the failure on `master`, then create a new tag
  and release from the corrected commit.

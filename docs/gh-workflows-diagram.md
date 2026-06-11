# GitHub Workflows & Actions Diagram

```mermaid
flowchart TB
    subgraph Triggers
        T1["release: published"]
        T2["workflow_dispatch (manual)"]
    end

    subgraph Workflows
        WF1["publish.yml<br/><i>Publish ALL (VSIX, UI App, CLI App)</i>"]
        WF2["uiapp.yml<br/><i>Publish UI App</i>"]
    end

    T1 --> WF1
    T2 --> WF2

    subgraph Actions [".github/actions/"]
        A1["validate-tag<br/><i>Validate release tag vX.Y.Z</i>"]
        A2["update-version<br/><i>Update version in 5 project files</i>"]
        A3["uiapp<br/><i>Build and publish UI App</i>"]
        A4["cliapp<br/><i>Build portable CLI packages</i>"]
    end

    %% publish.yml relationships
    WF1 --> A1
    WF1 --> A3
    WF1 --> A4

    %% uiapp.yml relationships
    WF2 --> A1
    WF2 --> A2
    WF2 --> A3
    WF2 --> A4

    %% Version files updated by update-version
    subgraph VersionFiles ["Version files"]
        VF1["src/App/App.csproj"]
        VF2["ProductAssemblyInfo.cs"]
        VF3["VsExtension2022/Properties/AssemblyInfo.cs"]
        VF4["VsExtension2022/VsExtensionConstants.cs"]
        VF5["VsExtension2022/source.extension.vsixmanifest"]
    end

    A2 --> VF1 & VF2 & VF3 & VF4 & VF5
    WF1 -. "validates versions" .-> VF1 & VF2 & VF3 & VF4 & VF5
    WF2 -. "git commit + push" .-> VersionFiles
```

## Workflow Summary

| Workflow | Trigger | Custom Actions | Key Behavior |
|----------|---------|----------------|--------------|
| **publish.yml** | Release published | validate-tag, uiapp, cliapp | Validates all 5 version files match tag, builds VSIX + UI App + CLI, publishes to VS Marketplace and GitHub Releases |
| **uiapp.yml** | Manual dispatch | validate-tag, update-version, uiapp, cliapp (optional) | Updates all 5 version files, commits and pushes back to branch, builds and publishes UI App |

## Action Summary

| Action | Purpose | Key Inputs | Key Outputs |
|--------|---------|------------|-------------|
| **validate-tag** | Validates `vX.Y.Z` tag format, extracts version | `tag` | `tag_version`, `tag_valid` |
| **update-version** | Updates version in 5 project files | `new_version` | - |
| **uiapp** | Ensures release exists, installs vpk, builds/publishes UI app | `mode`, `vpk_version`, `channel` | - |
| **cliapp** | Builds portable Linux + Windows CLI packages | `new_version` | `linux_path`, `windows_path` |

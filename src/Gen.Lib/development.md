## Development Setup

### Github PAT Token

To install npm packages from GitHub Packages, you need to provide a GitHub Personal Access Token (PAT) with the appropriate permissions.

Create new PAT token:
1. Go to [GitHub Settings - Developer settings - Personal access tokens](https://github.com/settings/tokens)
2. Create new token (classic) with `read:packages` scope.
3. Copy the generated token.
4. Set the token as an environment variable named `GH_READ_PACKAGES_TOKEN`.

### Setting Environment Variable automatically

For powershell users, follow the instructions below to set the environment variable automatically from a `.env` file.

To apply .env file in windows/powershell (v7+), follow these steps:

1. Open PowerShell as Administrator.
2. Run the following command to install the pwsh-dotenv module:

    ```powershell
    Install-Module -Name pwsh-dotenv -Scope CurrentUser
    ```

4. Run the following command to open profile file in a text editor:

   ```bash
   notepad $PROFILE
   ```

6. Add the following line to the profile file to import the pwsh-dotenv module automatically:

    ```powershell
    Import-Module pwsh-dotenv
    if (Test-Path ".env") {
        Import-Dotenv ".env"
    }
    ```

7. Test in powershell by running:

    ```powershell
    $env:GH_READ_PACKAGES_TOKEN
    ```

   It should display the value of the token set in the `.env` file. 
$ErrorActionPreference = 'Stop'

Set-Location -Path "d:\TODO\backend"

if (-not (Test-Path .git)) {
    git init
}

# Only set if not set globally
$email = git config --global user.email
if (-not $email) {
    git config user.email "user@example.com"
    git config user.name "User"
}

# Create .gitignore
@"
[Bb]in/
[Oo]bj/
.vscode/
"@ | Out-File -FilePath .gitignore -Encoding utf8

# Add root files
git add .gitignore Dockerfile git_setup.ps1
# Also add base files in TodoApp.Api so the branches have a base
git add TodoApp.Api/*.cs TodoApp.Api/*.csproj TodoApp.Api/*.json TodoApp.Api/*.http
git commit -m "Initial commit: Base project files"
git branch -M main

git checkout -b development

# Get all valid folders inside TodoApp.Api
$folders = Get-ChildItem -Path "TodoApp.Api" -Directory | Where-Object { $_.Name -notin @('bin', 'obj') }

foreach ($folder in $folders) {
    $branchName = $folder.Name.ToLower()
    git checkout -b $branchName development
    git add "TodoApp.Api/$($folder.Name)"
    # check if there are files to commit
    $status = git status --porcelain
    if ($status) {
        git commit -m "Add $($folder.Name) features"
    }
    git checkout development
    git merge $branchName --no-ff --no-edit -m "Merge branch '$branchName' into development"
}

# create release branch
git checkout -b release/v1.0.0 development

# merge release/v1.0.0 into main
git checkout main
git merge release/v1.0.0 --no-ff --no-edit -m "Merge release/v1.0.0 into main"

# Setup remote and push
git remote add origin https://github.com/harikrishnan-ps/TODO-BACKEND.git
git push -u origin --all

# Git Repository Management Guide

How to change, switch, and manage Git repositories.

## Table of Contents
- [Change Remote Repository URL](#change-remote-repository-url)
- [Switch Between Local Repositories](#switch-between-local-repositories)
- [Add Multiple Remotes](#add-multiple-remotes)
- [Clone a Different Repository](#clone-a-different-repository)
- [Switch Branches](#switch-branches)

---

## Change Remote Repository URL

### View Current Remote
```powershell
cd C:\Users\rober\Documents\GitHub\IPManagementInterface
git remote -v
```

### Change Remote URL
```powershell
# Change the URL of existing remote
git remote set-url remote-push <new-url>

# Example: Change to a different GitHub repo
git remote set-url remote-push https://github.com/username/new-repo.git

# Example: Change to a different remote name
git remote set-url origin https://github.com/username/new-repo.git
```

### Remove and Add New Remote
```powershell
# Remove existing remote
git remote remove remote-push

# Add new remote
git remote add origin https://github.com/username/new-repo.git
```

---

## Switch Between Local Repositories

### Navigate to Different Repository
```powershell
# Switch to IPManagementInterface repo
cd C:\Users\rober\Documents\GitHub\IPManagementInterface

# Switch to remote-location repo
cd C:\Users\rober\Documents\GitHub\remote-location

# Check current location
pwd  # or Get-Location in PowerShell
```

### Check Which Repository You're In
```powershell
# Check current directory
Get-Location

# Check git status
git status

# Check remote URL
git remote -v
```

---

## Add Multiple Remotes

You can have multiple remotes for the same local repository:

```powershell
# Add multiple remotes
git remote add origin https://github.com/username/repo1.git
git remote add backup https://github.com/username/repo2.git
git remote add upstream https://github.com/original/repo.git

# View all remotes
git remote -v

# Push to specific remote
git push origin main
git push backup main
git push upstream main

# Pull from specific remote
git pull origin main
```

### Example: Multiple Remotes Setup
```powershell
# Primary remote (GitHub)
git remote add origin https://github.com/devildog5x5/IPManagementInterface.git

# Backup remote (different repo)
git remote add backup https://github.com/devildog5x5/remote-location.git

# Push to both
git push origin discovery-alerts
git push backup discovery-alerts
```

---

## Clone a Different Repository

### Clone a New Repository
```powershell
# Navigate to where you want the repo
cd C:\Users\rober\Documents\GitHub

# Clone a repository
git clone https://github.com/username/repository-name.git

# Clone into specific folder
git clone https://github.com/username/repository-name.git my-folder-name
```

### Example: Clone remote-location
```powershell
cd C:\Users\rober\Documents\GitHub
git clone https://github.com/devildog5x5/remote-location.git
```

---

## Switch Branches

### View All Branches
```powershell
# Local branches
git branch

# All branches (local + remote)
git branch -a

# Current branch (marked with *)
```

### Switch to Different Branch
```powershell
# Switch to existing branch
git checkout branch-name

# Or using newer syntax
git switch branch-name

# Create and switch to new branch
git checkout -b new-branch-name
git switch -c new-branch-name
```

### Example: Switch Branches
```powershell
# Current branch: discovery-alerts
git branch
# * discovery-alerts
#   master

# Switch to master
git checkout master
# or
git switch master
```

---

## Common Scenarios

### Scenario 1: Change Remote to Different GitHub Repo
```powershell
cd C:\Users\rober\Documents\GitHub\IPManagementInterface
git remote set-url remote-push https://github.com/username/new-repo.git
git push remote-push discovery-alerts
```

### Scenario 2: Push Same Code to Multiple Repos
```powershell
# Add multiple remotes
git remote add repo1 https://github.com/user/repo1.git
git remote add repo2 https://github.com/user/repo2.git

# Push to both
git push repo1 discovery-alerts
git push repo2 discovery-alerts
```

### Scenario 3: Switch Between Your Repositories
```powershell
# Work on IPManagementInterface
cd C:\Users\rober\Documents\GitHub\IPManagementInterface
git status

# Switch to remote-location
cd C:\Users\rober\Documents\GitHub\remote-location
git status
```

### Scenario 4: Copy Code to Different Repo
```powershell
# Option A: Add as new remote and push
cd C:\Users\rober\Documents\GitHub\IPManagementInterface
git remote add newrepo https://github.com/user/newrepo.git
git push newrepo discovery-alerts

# Option B: Clone and copy files
cd C:\Users\rober\Documents\GitHub
git clone https://github.com/user/newrepo.git
# Then copy files manually
```

---

## Quick Reference Commands

```powershell
# View remotes
git remote -v

# Change remote URL
git remote set-url <remote-name> <new-url>

# Add new remote
git remote add <name> <url>

# Remove remote
git remote remove <name>

# Rename remote
git remote rename <old-name> <new-name>

# Push to specific remote
git push <remote-name> <branch-name>

# Pull from specific remote
git pull <remote-name> <branch-name>

# Switch directories
cd <path-to-repo>

# Check current repo
git remote -v
git status
```

---

## Your Current Setup

### IPManagementInterface Repository
- **Location:** `C:\Users\rober\Documents\GitHub\IPManagementInterface\`
- **Current Remote:** `remote-push` → `https://github.com/devildog5x5/remote-location.git`
- **Current Branch:** `discovery-alerts`

### remote-location Repository
- **Location:** `C:\Users\rober\Documents\GitHub\remote-location\`
- **Status:** Empty repository (ready for content)

---

## Troubleshooting

### "Remote URL not found"
```powershell
# Check if remote exists
git remote -v

# If missing, add it
git remote add origin <url>
```

### "Permission denied"
- Check GitHub credentials
- Verify you have push access to the repository
- Use Personal Access Token if needed

### "Repository not found"
- Verify the repository URL is correct
- Check if repository exists on GitHub
- Ensure you have access to the repository

---

## Need Help?

- Check current remotes: `git remote -v`
- Check current branch: `git branch`
- Check current location: `Get-Location` (PowerShell) or `pwd` (bash)
- View git status: `git status`


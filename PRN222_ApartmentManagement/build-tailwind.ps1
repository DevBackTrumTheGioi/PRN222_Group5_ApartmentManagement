#!/usr/bin/env pwsh
# Build Tailwind CSS script

$inputCss = "wwwroot/css/input.css"
$outputCss = "wwwroot/css/output.css"

Write-Host "Building Tailwind CSS..." -ForegroundColor Green

# Try method 1: npm run
try {
    Write-Host "Method 1: Using npm run build:css"
    npm run build:css
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ Build successful!" -ForegroundColor Green
        exit 0
    }
} catch {
    Write-Host "✗ npm run failed" -ForegroundColor Yellow
}

# Try method 2: npx
try {
    Write-Host "Method 2: Using npx tailwindcss"
    npx tailwindcss -i $inputCss -o $outputCss
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ Build successful!" -ForegroundColor Green
        exit 0
    }
} catch {
    Write-Host "✗ npx failed" -ForegroundColor Yellow
}

# Try method 3: node_modules binary
try {
    Write-Host "Method 3: Using local node_modules"
    & node node_modules/tailwindcss/lib/cli.js -i $inputCss -o $outputCss
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ Build successful!" -ForegroundColor Green
        exit 0
    }
} catch {
    Write-Host "✗ node_modules failed" -ForegroundColor Yellow
}

# Try method 4: Check if tailwindcss.exe exists
$tailwindExe = "tailwindcss.exe"
if (Test-Path $tailwindExe) {
    try {
        Write-Host "Method 4: Using tailwindcss.exe"
        & .\$tailwindExe -i $inputCss -o $outputCss
        if ($LASTEXITCODE -eq 0) {
            Write-Host "✓ Build successful!" -ForegroundColor Green
            exit 0
        }
    } catch {
        Write-Host "✗ tailwindcss.exe failed" -ForegroundColor Yellow
    }
}

Write-Host "✗ All methods failed. Please build manually with: npm run build:css" -ForegroundColor Red
exit 1


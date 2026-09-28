@echo off
cd /d "%~dp0"
npx --yes jscpd . --pattern "**/*.cs" --min-lines 10 --min-tokens 50 --reporters console,json --output ./report

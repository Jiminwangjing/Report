@echo off
if "%1%" == "" (
	php build/build.all.php
) else (
	php build/build.%1.php
)
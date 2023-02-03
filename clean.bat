@echo off
cls
for /D /R %%f in (bin obj) do rmdir /s/q "%%f"
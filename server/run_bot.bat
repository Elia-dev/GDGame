@echo off
start /min python.exe main_bot_controller.py %1 %2 > output_run_bot.log 2>&1
echo %ERRORLEVEL%
@echo off
set game_id=%1
set bot_name=%2
start /B python main_bot_controller.py %game_id% %bot_name%
echo %ERRORLEVEL%

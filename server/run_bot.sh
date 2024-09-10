#!/bin/bash
game_id=$1
bot_name=$2
/usr/bin/python3 main_bot_controller.py "$game_id" "$bot_name" > /dev/null 2>&1 &
echo $!
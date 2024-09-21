#!/bin/bash
game_id=$1
bot_name=$2
log_file="bot_${game_id}_${bot_name}.log"
/usr/bin/python3 main_bot_controller.py "$game_id" "$bot_name" > "$log_file" 2>&1 &
echo $!
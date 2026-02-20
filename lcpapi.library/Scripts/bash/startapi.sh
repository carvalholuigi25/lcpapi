#!/bin/bash
#set -e

#pthproj="$HOME/Documents/projects/lcpapi/lcpapi"
pthproj=$(sudo find $HOME -type d -name "lcpapi" -prune 2>/dev/null)

# cd "$pthproj" || exit
# pocdnet=$(netstat -ano | findstr :5000 | findstr LISTENING | awk '{print $5}')
# pocdnet=$(ps aux | grep dotnet)
# kill -9 $pocdnet

cd "$pthproj/lcpapi" || { echo "Directory not found: $pthproj"; exit 1; }

# Find any process using port 5000 and kill it
pids=$(lsof -t -i:5000)
if [ -n "$pids" ]; then
    echo "Killing processes using port 5000: $pids"
    kill -9 $pids
fi

# Kill all 'dotnet' processes
pkill -f dotnet || true

dotnet watch

if [ $? -ne 0 ]
  then
    echo LCPAPI couldnt be started...
  else
    echo LCPAPI is now starting...
  fi



exit
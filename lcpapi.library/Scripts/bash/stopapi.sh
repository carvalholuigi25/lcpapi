#!/bin/bash
# set -e

# pthproj="$HOME/Documents/projects/lcpapi/lcpapi"
pthproj=$(sudo find $HOME -type d -name "lcpapi" -prune 2>/dev/null)

# cd "$pthproj" || exit
# pocdnet=$(netstat -ano | findstr :5000 | findstr LISTENING | awk '{print $5}')
# pocdnet=$(ps aux | grep dotnet)
# kill -9 $pocdnet

cd "$pthproj/lcpapi" || { echo "Directory not found: $pthproj"; exit 1; }

pids=$(lsof -t -i:5000)
if [ -n "$pids" ]; then
    echo "Killing processes using port 5000: $pids"
    kill -9 $pids
fi

pkill -f dotnet || true

if [ $? -ne 0 ]
  then
    echo LCPAPI couldnt be stopped...
  else
    echo LCPAPI is now stopped...
  fi


exit
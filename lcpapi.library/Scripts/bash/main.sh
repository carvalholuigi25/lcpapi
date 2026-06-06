#!/bin/bash
set -e

pthproj="$HOME/mydocs/lcpapi/lcpapi.library"
#pthproj=$(sudo find $HOME -type d -name "lcpapi/lcpapi.library" -prune 2>/dev/null | head -n 1)

cd "$pthproj" || exit

startAPI() {
    clear
    source $pthproj/Scripts/bash/startapi.sh
    exit
}

stopAPI() {
    clear
    source $pthproj/Scripts/bash/stopapi.sh
    exit
}

genDB() {
   clear
   source $pthproj/Scripts/bash/gendb.sh
   exit
}

invChoice() {
    clear
    echo "Invalid choice!"
    exit
}

main() {
    clear
    echo
    echo "LCPAPI Main Menu"
    echo
    echo "--------------------------------------"
    echo "Author Info:"
    echo "Name: Luis Carvalho"
    echo "Email: luiscarvalho239@gmail.com"
    echo "Date creation of script: 20/10/2025"
    echo "--------------------------------------"
    echo
    echo "Choose your option:"
    echo
    echo "A - Start API"
    echo "B - Stop API"
    echo "C - Generate DB"
    echo "E - Exit"
    echo "--------------------------------------"
    echo
    read -p "" chmmode

    case "$chmmode" in
        ""|A|a) startAPI ;;
        B|b) stopAPI ;;
 	    C|c) genDB ;;
        E|e) exit 0 ;;
        *) invChoice ;;
    esac
}

main
exit
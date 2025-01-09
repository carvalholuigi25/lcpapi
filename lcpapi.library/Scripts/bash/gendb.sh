#!/bin/bash

#pthproj="$HOME/Documents/projects/lcpapi/lcpapi"
pthproj=$(sudo find $HOME -type d -name "lcpapi" -prune 2>/dev/null)
pthmig="$pthproj/Migrations"
DefDBMode="SQLite"

cd "$pthproj" || exit

main() {
    clear
    echo
    echo "Generate DB for API"
    echo
    echo "--------------------------------------"
    echo "Author Info:"
    echo "Name: Luis Carvalho"
    echo "Email: luiscarvalho239@gmail.com"
    echo "Date creation of script: 30/09/2024"
    echo "--------------------------------------"
    echo
    echo "Choose your option:"
    echo
    echo "Note: The default option is $DefDBMode"
    echo
    echo "1 - SQLite"
    echo "2 - SQLServer"
    echo "3 - PostgresSQL"
    echo "4 - MySQL"
    echo "5 - All"
    echo
    read -p "" chdbmode

    case "$chdbmode" in
        ""|1) addSQLite ;;
        2) addSQLServer ;;
        3) addPostgresSQL ;;
        4) addMySQL ;;
        5) addAll ;;
        *) invChoice ;;
    esac
}

chkDNEFInstalled() {
  dotnet ef
  if [ $? -ne 0 ]
  then
    dotnet tool install --global dotnet-ef
  else
    echo Dotnet EF Core Tools installed
  fi
}

addDB() {
    chkDNEFInstalled

    if [ -d "$pthmig/$1" ]; then
        rm -rf "$pthmig/$1"
    fi

    if [ -d "$pthproj/Database/$1" ]; then
        rm -rf "$pthproj/Database/$1"
    fi

    if [ ! -d "$pthproj/Database/$1" ]; then
        mkdir -p "$pthproj/Database/$1"
    fi

    dotnet ef migrations remove --force --context "MyDBContext$1"
    dotnet ef database drop --force --context "MyDBContext$1"
    dotnet ef migrations add "InitialCreate$1" --context "MyDBContext$1" --output-dir "$pthmig/$1"
    dotnet ef database update "InitialCreate$1" --context "MyDBContext$1"
    dotnet ef migrations script --context "MyDBContext$1" --output "Scripts/sql/migscr$1.sql"
}

addSQLite() {
    clear
    DefDBMode="SQLite"
    echo "$DefDBMode"
    addDB "$DefDBMode"
}

addSQLServer() {
    clear
    DefDBMode="SQLServer"
    echo "$DefDBMode"
    addDB "$DefDBMode"
}

addPostgresSQL() {
    clear
    DefDBMode="PostgresSQL"
    echo "$DefDBMode"
    addDB "$DefDBMode"
}

addMySQL() {
    clear
    DefDBMode="MySQL"
    echo "$DefDBMode"
    addDB "$DefDBMode"
}

addAll() {
    addSQLite
    addSQLServer
    addPostgresSQL
    addMySQL
}

invChoice() {
    clear
    echo "Invalid choice!"
}

main

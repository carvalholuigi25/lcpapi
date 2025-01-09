#!/bin/bash

getNumDays() {
    ya=$(date +%Y)
    numy=10
    ((!($ya % 4) && ($ya % 100 || !($ya % 400)))) && echo $((366 * $numy)) || echo $((365 * $numy))
}

SERVERNAME="localhost"
NUMDAYS=$(getNumDays)

if [ ! -d "Keys" ]; then
    mkdir -p "Keys"
fi

if [ ! -d "Certs" ]; then
    mkdir -p "Certs"
fi

openssl req -x509 -newkey rsa:4096 -sha256 -days $NUMDAYS -nodes -keyout Keys/$SERVERNAME.key -out Certs/$SERVERNAME.crt -subj "//CN=${SERVERNAME}" -extensions v3_ca -extensions v3_req -config config.cnf && openssl pkcs12 -export -out Certs/$SERVERNAME.pfx -inkey Keys/$SERVERNAME.key -in Certs/$SERVERNAME.crt
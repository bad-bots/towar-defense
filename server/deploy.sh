#!/bin/bash
docker build -t "towar-defense" .
docker run \
	-p "4574:8080" \
	--env-file "./.env.production" \
	towar-defense